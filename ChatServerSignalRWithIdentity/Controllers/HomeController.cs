using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ChatServerSignalRWithIdentity.Data;
using ChatServerSignalRWithIdentity.Data.DTO;
using ChatServerSignalRWithIdentity.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChatServerSignalRWithIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Data.SqlClient;

namespace ChatServerSignalRWithIdentity.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public readonly ApplicationDbContext _context;
        public readonly UserManager<AppUser> _userManager;
        protected readonly IMapper _mapper;
        IWebHostEnvironment _appEnvironment;

        public HomeController(ApplicationDbContext context, UserManager<AppUser> userManager, IMapper mapper, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _appEnvironment = appEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            ViewBag.CurrentUserName = currentUser.UserName;

             //получаю все диалоги с моим участием
             var allDialogsWitMe = await _context
                 .Dialogs
                 .Include(x => x.Participants)
                 .Where(x => x.Participants.Any(y => y.AppUserId == currentUser.Id))
                 .Include(m=>m.Messages)
                 .OrderByDescending(x => x.LastActivityUtc)
                 .ToListAsync();

            //получаю список своих друзей
            var myFriends = await _context.UserRelationships
                .Where(c => (c.SmallerUserId == currentUser.Id ||
                             c.BiggerUserId == currentUser.Id)
                            && c.Status == RelationshipStatus.Friend).ToListAsync();

            
            var myDialogsWithFriends = new List<Dialog>();
            //в модель передать последние 10 диалогов с ДРУЗЬЯМИ (т.е. для каждого друга ищу диалог с собой, добавляю в список выводимых диалогов)
         
            foreach (var friend in myFriends)
            {
                var dialogWithFriend = new Dialog();

                if (currentUser.Id == friend.SmallerUserId)
                {
                    dialogWithFriend = allDialogsWitMe.Find(x =>
                        x.Participants.First().AppUserId == friend.BiggerUserId ||
                        x.Participants.Last().AppUserId == friend.BiggerUserId);

                }
                else
                {
                    dialogWithFriend = allDialogsWitMe.Find(x =>
                        x.Participants.First().AppUserId == friend.SmallerUserId ||
                        x.Participants.Last().AppUserId == friend.SmallerUserId);
                }

                myDialogsWithFriends.Add(dialogWithFriend);
            }

            var dialogsForModel = myDialogsWithFriends.OrderByDescending(l => l.LastActivityUtc).Take(10).ToList();

            var lastMessages = new List<Message>();
            var newMessagesCount = new List<int>();

            foreach (var dialog in dialogsForModel)
            {
                if (dialog.LastMessageId != 0)
                {
                    var m = dialog.Messages.FirstOrDefault(x => x.Id == dialog.LastMessageId);
                    lastMessages.Add(m);
                }
            }

            var response = new ChatModel
            {
                CallerId = currentUser.Id,
                MessagesList = _mapper.Map<List<Message>>(lastMessages),
                DialogsWithFriendsList = _mapper.Map<List<Dialog>>(dialogsForModel),
            };

            return View(response);
        }

        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            ViewBag.CurrentUserName = currentUser.UserName;

            var response = new AppUser()
            {
                UserName = currentUser.UserName,
                PhoneNumber = currentUser.PhoneNumber,
                Avatar = currentUser.Avatar
            };

            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(AppUserResponse model)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            ViewBag.CurrentUserName = currentUser.UserName;

             currentUser.PhoneNumber = model.PhoneNumber;
            if (model.Avatar != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(model.Avatar.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)model.Avatar.Length);
                }
                currentUser.MyAvatar = imageData;
            }

            //  var operationDetails = await _accountService.Update(currentUser);
            //  currentUser.Avatar= new FileResponse();

            await _context.SaveChangesAsync();

            //return Ok();


            var response = new AppUser()
            {
                UserName = currentUser.UserName,
                PhoneNumber = currentUser.PhoneNumber,
                Avatar = currentUser.Avatar
            };

            return View(response);
        }

        public async Task<IActionResult> Searching(string username)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUserName = currentUser.UserName;
           
            var otherUser = await _context.AspNetUsers.Where(i => i.UserName == username).ToListAsync();
          
            string[] values;
            var _otherUser = new AppUserResponse();
            var myUser = new AppUserResponse();
            var status = RelationshipStatus.Stranger;

            if (otherUser.Count != 0)
            {
                values = new[] { currentUser.Id, otherUser.First().Id }.OrderBy(x => x).ToArray();

                var relationship = await _context.UserRelationships.Where(f=> (f.BiggerUserId==values.Last()) && f.SmallerUserId==values.First()).ToListAsync();

                if (relationship.Count != 0)
                {
                    status = relationship.First().Status;
                }

                _otherUser = _mapper.Map<AppUserResponse>(otherUser.First());
                myUser = _mapper.Map<AppUserResponse>(currentUser);
            }

            var response = new Couple()
            {
                MyUser = myUser,
                OtherUser = _otherUser,
                Status = status
            };

            return View(response);
        }

        // [HttpGet("home/AddFriendAndCreateDialog/{anotherUserId}")]
        public async Task<IActionResult> AddFriendAndCreateDialog(string anotherUserId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUserName = currentUser.UserName;
            
            var anotherUser = await _context.AspNetUsers.FindAsync(anotherUserId);

            var values = new[] { currentUser.Id, anotherUserId }.OrderBy(x => x).ToArray();

            var relationship = _context.UserRelationships.FirstOrDefault(i =>
                i.SmallerUserId==values.First() &&
                i.BiggerUserId == values.Last());

            var dialog = new Dialog();

            if (relationship != null)
            {
                relationship.Status = RelationshipStatus.Friend;

                dialog = _context.Dialogs.Include(x => x.Participants).Where(x => x.Participants.Any(y => y.AppUserId == currentUser.Id))
                   .Where(q => q.Participants.Any(w => w.AppUserId == anotherUserId)).First();
            }

            else
            {
                _context.UserRelationships.Add(new UserRelationship()
                {
                    BiggerUserId = values.Last(),
                    SmallerUserId = values.First(),
                    Status = RelationshipStatus.Friend
                });


                dialog.Participants = new List<Participant>
                {
                    new Participant {AppUserId = currentUser.Id, AppUserName = currentUser.UserName},
                    new Participant {AppUserId = anotherUser.Id, AppUserName = anotherUser.UserName}
                };
                dialog.LastActivityUtc = DateTime.UtcNow;
            }

            await _context.Dialogs.AddAsync(dialog);
            await _context.SaveChangesAsync();

            return View(dialog);
        }

        [HttpPost("home/DeleteFriend/{anotherUserId}")]
        public async Task<IActionResult> DeleteFriend(string anotherUserId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            ViewBag.CurrentUserName = currentUser.UserName;
            
            var anotherUser = _context.AspNetUsers.ToList().Find(i => i.Id == anotherUserId);
            var values = new[] { currentUser.Id, anotherUserId }.OrderBy(x => x).ToArray();

            var relationship = _context.UserRelationships.FirstOrDefault(i =>
                i.SmallerUserId == values.First() &&
                i.BiggerUserId == values.Last());
            
            if (relationship != null)
            {
                relationship.Status = RelationshipStatus.Stranger;
            }
           
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("home/Block/{anotherUserId}")]
        public async Task<IActionResult> Block(string anotherUserId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            ViewBag.CurrentUserName = currentUser.UserName;
            
            var values = new[] { currentUser.Id, anotherUserId }.OrderBy(x => x).ToArray();

            var userForBlock = _context.UserRelationships.FirstOrDefault(i =>
                i.SmallerUserId == values.First() &&
                i.BiggerUserId == values.Last());

            if (userForBlock != null)
            {
                if (currentUser.Id == values.First())
                {
                    userForBlock.Status = RelationshipStatus.SmallBlockBigger;
                }

                else
                {
                    userForBlock.Status = RelationshipStatus.BigBlockSmaller;
                }

            }
            else
            {
                if (currentUser.Id == values.First())
                {
                    _context.UserRelationships.Add(new UserRelationship()
                    {
                        BiggerUserId = values.Last(),
                        SmallerUserId = values.First(),
                        Status = RelationshipStatus.SmallBlockBigger
                    });
                }
                else
                {
                    _context.UserRelationships.Add(new UserRelationship()
                    {
                        BiggerUserId = values.Last(),
                        SmallerUserId = values.First(),
                        Status = RelationshipStatus.BigBlockSmaller
                    });
                }

            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("home/Unblock/{anotherUserId}")]
        public async Task<IActionResult> Unblock(string anotherUserId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            ViewBag.CurrentUserName = currentUser.UserName;

            var values = new[] { currentUser.Id, anotherUserId }.OrderBy(x => x).ToArray();


            var userForUnblock = _context.UserRelationships.FirstOrDefault(i =>
                i.SmallerUserId == values.First() &&
                i.BiggerUserId == values.Last());


            if (userForUnblock != null)
            {
                userForUnblock.Status = RelationshipStatus.Stranger;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        public async Task<IActionResult> GetMessagesForDialog(int dialogId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUserName = currentUser.UserName;

                var dialog = await _context.Dialogs.Where(i => i.Id == dialogId).Include(x => x.Messages)
                .Include(p => p.Participants).ToArrayAsync();
          
            var messages = dialog.First().Messages.OrderByDescending(c => c.CreatedUtc).Take(10).ToList();
            foreach (var message in messages)
            {
                if (message.OwnerId==currentUser.Id)
                {
                    message.Read = true;
                }
            }

            await _context.SaveChangesAsync();

            return PartialView("_Messages", messages);
        }

        [HttpGet("home/GetNewMessageForIndex/{dialogId}")]
        public JsonResult GetNewMessageForIndex(int dialogId)
        {
            var currentUser =  _userManager.GetUserAsync(User);
            ViewBag.CurrentUserName = currentUser.Result.UserName;

            var dialog =  _context.Dialogs
                .Include(x => x.Participants).Include(m => m.Messages).FirstOrDefaultAsync(x =>
                    x.Id.Equals(dialogId)).Result;

            var messages = dialog.Messages.OrderByDescending(c => c.CreatedUtc).Where(x => x.Read == false).ToList();

            if (messages is null)
            {
                messages = dialog.Messages.OrderByDescending(c => c.CreatedUtc).Take(1).ToList();
            }

            //Получила ЛИСТ С НЕПРОЧИТАННЫМИ(чтобы взять их количество для отображения) СООБЩЕНИЯМИ ИЛИ ПРОСТО ПОСЛЕДНИМ
            return Json(messages);
        }

        [HttpPost]
        //public async Task<IActionResult> SaveInfo(string phoneNumber, string avatarPath, IFormFile upload, [FromServices] IHostingEnvironment env)
        public async Task<IActionResult> SaveInfo(AppUserResponse au)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            ViewBag.CurrentUserName = currentUser.UserName;

           // currentUser.PhoneNumber = phoneNumber;
            if (au.Avatar != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(au.Avatar.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)au.Avatar.Length);
                }
                currentUser.MyAvatar = imageData;
            }

            //if (upload != null && upload.Length > 0)
            //{
            //    using (var reader = new BinaryReader(upload.OpenReadStream()))
            //    {
            //     //   currentUser.HasAvatar = true;
            //        currentUser.Avatar = new Avatar()
            //        {
            //            FileName = Path.GetFileName(upload.FileName),
            //            ContentType = upload.ContentType,
            //            Content = reader.ReadBytes((int)upload.Length)
            //        };
            //    }
            //}
            //else
            //{
            //    var webRoot = env.WebRootPath;
            //    var p = Path.Combine(webRoot, "Images/pf.jpg");
            //    currentUser.Avatar = new Avatar()
            //    {
            //        FileName = "pf.jpg",
            //        ContentType = "image/jpeg",
            //        Content = System.IO.File.ReadAllBytes(p)
            //    };
            //}
            //  var operationDetails = await _accountService.Update(currentUser);
            //  currentUser.Avatar= new FileResponse();

            await _context.SaveChangesAsync();

            return Ok();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

    }
}
