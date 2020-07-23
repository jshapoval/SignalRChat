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

        public HomeController(ApplicationDbContext context, UserManager<AppUser> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

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

        [HttpPost("home/Send/")]
        public async Task<IActionResult> Send(string text, int dialogId)
        {
            if (ModelState.IsValid)
            {
                var dialog = await _context.Dialogs
                    .Include(x => x.Participants).Include(m => m.Messages).FirstOrDefaultAsync(x =>
                        x.Id.Equals(dialogId));

                var myUser = await _userManager.GetUserAsync(User);
                var anotherUser = dialog.Participants.FirstOrDefault(i => i.AppUserId != myUser.Id);

                if (dialog is null)
                {
                    dialog = new Dialog
                    {
                        LastActivityUtc = DateTime.UtcNow,
                        Participants = new List<Participant>
                            {new Participant {AppUserId = myUser.Id, AppUserName = myUser.UserName}, new Participant {AppUserId = anotherUser.AppUserId, AppUserName = anotherUser.AppUserId}}
                    };

                    await _context.Dialogs.AddAsync(dialog);
                    await _context.SaveChangesAsync();
                }

                var messageModel = new MessageModel
                {
                    DialogId = dialogId,
                    UserId = myUser.Id,
                    Text = text
                };

                var message = _mapper.Map<Message>(messageModel);
                message.UserName = myUser.UserName;
                message.SenderId = myUser.Id;
                message.OwnerId = anotherUser.AppUserId;
                message.DialogId = dialog.Id;
                //    добавляю поля еще те, что ниже
                message.Text = messageModel.Text;
                message.CreatedUtc = DateTime.UtcNow;

                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();

                dialog.LastActivityUtc = DateTime.UtcNow;
                dialog.Messages.Add(message);
                dialog.LastMessageId = message.Id;

                await _context.SaveChangesAsync();

                return Ok();
            }

            return Error();
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

            // добавляем в друзья без требования ответного подтверждения, т.е. статус отношений устанавливаем у юсера и создаем диалог

            var values = new[] { currentUser.Id, anotherUserId }.OrderBy(x => x).ToArray();

            //ПРОВЕРКА НА СУЩЕСТВОВАНИЕ ОТНОШЕНИЙ, ВДРУГ Я РАЗБЛОЧИДА ПОЛЬЗОВАТЕЛЯ И ХОЧУ ДОБАВИТЬ В ДРУЗЬЯ
            var relationship = _context.UserRelationships.FirstOrDefault(i =>
                i.SmallerUserId==values.First() &&
                i.BiggerUserId == values.Last());

            var dialog = new Dialog();

            if (relationship != null)
            {
                relationship.Status = RelationshipStatus.Friend;

                dialog = _context.Dialogs.Include(x => x.Participants).Where(x => x.Participants.Any(y => y.AppUserId == currentUser.Id))
                   .Where(q => q.Participants.Any(w => w.AppUserId == anotherUserId)).First();

                ////
                //(n.Participants.First().AppUserId == currentUser.Id) &&
                //    (n.Participants.Last().AppUserId == anotherUserId) ||
                //    (n.Participants.First().AppUserId == anotherUserId) &&
                //    (n.Participants.Last().AppUserId == currentUser.Id)).First();
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

            //////await _context.Entry(anotherUser).Collection(x => x.Relationships).LoadAsync();
            //////anotherUser.Relationships.Add(new UserRelationship() { BigUserId = values.Last(), SmallUserId = values.First(), Id = String.Concat(values.Last(), values.First()), Status = RelationshipStatus.Friend });
            //////не надо ли второму пользователю добавлять этот же диалог и статус???????????????????

            await _context.Dialogs.AddAsync(dialog);
            await _context.SaveChangesAsync();

            //  return Ok();\
            return View(dialog);
        }

        [HttpPost("home/DeleteFriend/{anotherUserId}")]
        public async Task<IActionResult> DeleteFriend(string anotherUserId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            ViewBag.CurrentUserName = currentUser.UserName;
            
            //статус отношений меняю на чужака у юсера, с диалогом наверное ничего не делаю, допустим в индексе перестает предлагаться друг с этим ником

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

            //  return View("Searching");
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

        //[HttpGet("home/GetMessagesForDialog/{dialogId}")]
        public async Task<IActionResult> GetMessagesForDialog(int dialogId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUserName = currentUser.UserName;

                var dialog = await _context.Dialogs.Where(i => i.Id == dialogId).Include(x => x.Messages)
                .Include(p => p.Participants).ToArrayAsync();
          
            var messages = dialog.First().Messages.OrderByDescending(c => c.CreatedUtc).Take(10).ToList();
           
            return PartialView("_Messages", messages);
        }

        public async Task<IActionResult> GetDialogByLogin(string login)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUserName = currentUser.UserName;

            var userFromDialog = _context.AspNetUsers.Where(x => x.UserName == login).ToListAsync();
            //!!!!!!!добавить загрузку вариантов, отличающихся немного, вдруг опечатка или пользователь забыл ник друга

           
            var friend = new UserRelationship();
            var dialogsWithFriend = new List<Dialog>();
            var lastMessages = new List<Message>();

            //проверяю друзья ли мы с ним по-прежнему
            foreach (var user in userFromDialog.Result)
            {
                var values = new[] { currentUser.Id, user.Id }.OrderBy(x => x).ToArray();
                
                friend =  _context.UserRelationships
                    .First(c => (c.SmallerUserId == values.First() &&
                                 c.BiggerUserId == values.Last())
                                && c.Status == RelationshipStatus.Friend);

                if (friend != null)
                {
                    //ищу диалог, где participants - я и введенный ник
                    var dialogWithFriend = _context.Dialogs
                        .Include(l => l.LastMessageId)
                        .Include(m => m.Messages)
                        .Include(x => x.Participants)
                        .Where(x => x.Participants.Any(y => y.AppUserId == currentUser.Id))
                        .Where(q => q.Participants.Any(w => w.AppUserId == user.Id)).First();

                    dialogsWithFriend.Add(dialogWithFriend);

                    if (dialogWithFriend.LastMessageId != 0)
                    {
                        var message = dialogWithFriend.Messages.FirstOrDefault(x => x.Id == dialogWithFriend.LastMessageId);
                        lastMessages.Add(message);
                    }
                }
            }

            var response = new DialogsModel
            {
                MessagesList = _mapper.Map<List<Message>>(lastMessages),
                DialogsWithFriendsList = _mapper.Map<List<Dialog>>(dialogsWithFriend),
            };

            return PartialView("_Dialogs", response);
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

     

        //public ActionResult GetAvatar()
        //{
        //    return View(_context.Avatars);
        //}

        //public ActionResult CreateAvatar()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult CreateAvatar(Avatar pic, HttpPostedFileBase uploadImage)
        //{
        //    if (ModelState.IsValid && uploadImage != null)
        //    {
        //        byte[] imageData = null;
        //        // считываем переданный файл в массив байтов
        //        using (var binaryReader = new BinaryReader(uploadImage.InputStream))
        //        {
        //            imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
        //        }
        //        // установка массива байтов
        //        pic.Image = imageData;

        //        _context.Avatars.Add(pic);
        //        _context.SaveChanges();

        //        return RedirectToAction("GetAvatar");
        //    }
        //    return View(pic);
        //}


        public async Task<IActionResult> CreatePrivate(Message message, AppUserResponse to)
        {
            if (ModelState.IsValid)
            {
                message.UserName = User.Identity.Name;
                var sender = await _userManager.GetUserAsync(User);
                message.SenderId = sender.Id;
                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();
                return Ok();
            }

            return Error();
        }
    }
}
