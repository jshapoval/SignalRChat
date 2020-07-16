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

            //var users = await _context.AspNetUsers.ToListAsync();
        
             //получаю все диалоги с моим участием
             var allDialogsWitMe = await _context
                 .Dialogs
                 .Include(x => x.Participants)
                 .Where(x => x.Participants.Any(y => y.AppUserId == currentUser.Id))
                 .OrderByDescending(x => x.LastActivityUtc)
                 .ToListAsync();

            //получаю список своих друзей
            var myFriends = await _context.UserRelationships
                .Where(c => (c.SmallerUserId == currentUser.Id ||
                             c.BiggerUserId == currentUser.Id)
                            && c.Status == RelationshipStatus.Friend).ToListAsync();

            
            var myDialogsWithFriends = new List<Dialog>();
            //в модель передать последние 10 диалогов с ДРУЗЬЯМИ (т.е. для каждого друга ищу диалог с собой, добавляю в список выводимых диалогов)
            if (myFriends!=null && myFriends.Count!=0)
            {
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
            }
         //   await _context.Entry(currentUser).Collection(x => x.Relationships).LoadAsync();

         var dialogsForModel = myDialogsWithFriends.OrderByDescending(l => l.LastActivityUtc).Take(10);


            var lastMessages = new List<Message>();

            foreach (var dialog in dialogsForModel)
            {
                if (dialog.Messages.Count != 0)
                {
                    var m = dialog.Messages?.OrderByDescending(c => c.CreatedUtc).First();
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

        //[HttpPost("home/Send/")]
        //public async Task<IActionResult> Send(string text, int dialogId)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var dialog = await _context.Dialogs
        //            .Include(x => x.Participants).Include(m => m.Messages).FirstOrDefaultAsync(x =>
        //                x.Id.Equals(dialogId));


        //        var myUser = await _userManager.GetUserAsync(User);
        //        var anotherUser = dialog.Participants.FirstOrDefault(i => i.AppUserId != myUser.Id);

        //        //if(dialog is null)
        //        //{
        //        //    dialog = new Dialog
        //        //    {
        //        //        LastActivityUtc = DateTime.UtcNow,
        //        //        Participants = new List<Participant>
        //        //            {new Participant {AppUserId = myUser.Id, AppUserName = myUser.UserName}, new Participant {AppUserId = anotherUser.Id, AppUserName = anotherUser.UserName}}
        //        //    };

        //        //    await _context.Dialogs.AddAsync(dialog);
        //        //    await _context.SaveChangesAsync();
        //        //}

        //        var messageModel = new MessageModel
        //        {
        //            DialogId = dialogId,
        //            UserId = myUser.Id,
        //            Text = text
        //        };

        //        var message = _mapper.Map<Message>(messageModel);

        //        message.UserName = myUser.UserName;
        //        message.SenderId = myUser.Id;
        //        message.OwnerId = anotherUser.AppUserId;
        //        message.DialogId = dialog.Id;

        //        //добавляю поля еще те, что ниже
        //        message.Text = messageModel.Text;
        //        message.CreatedUtc = DateTime.UtcNow;
        //        dialog.LastActivityUtc = DateTime.UtcNow;

        //        await _context.Messages.AddAsync(message);
        //        await _context.SaveChangesAsync();

        //        return Ok();
        //    }

        //    return Error();
        //}


        //public async Task<IActionResult> Searching(string username)
        //{
        //    var currentUser = await _userManager.GetUserAsync(User);
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        ViewBag.CurrentUserName = currentUser.UserName;
        //    }

        //    var otherUser = _context.AspNetUsers.ToList().Find(i => i.UserName == username);
        //    string[] values;
        //    var newFriend = new AppUserResponse();
        //    var myUser = new AppUserResponse();
        //    var status = new RelationshipStatus();

        //    if (otherUser != null)
        //    {
        //        await _context.Entry(currentUser).Collection(x => x.Relationships).LoadAsync();

        //        values = new[] {currentUser.Id, otherUser.Id}.OrderBy(x => x).ToArray();

        //        var relationship = currentUser.Relationships.ToList()
        //            .Find(i => i.Id == String.Concat(values.First(), values.Last()));

        //        if (relationship is null)
        //        {
        //            currentUser.Relationships.Add(new UserRelationship()
        //            {
        //                BigUserId = values.Last(), SmallUserId = values.First(),
        //                Id = String.Concat(values.First(), values.Last()), Status = RelationshipStatus.Stranger
        //            });
        //            status = RelationshipStatus.Stranger;
        //        }

        //        else
        //        {
        //            status = relationship.Status;

        //        }

        //        newFriend = _mapper.Map<AppUserResponse>(otherUser);
        //        myUser = _mapper.Map<AppUserResponse>(currentUser);
        //    }


        //    var response = new Couple()
        //    {
        //        MyUser = myUser,
        //        OtherUser = newFriend,
        //        Status = status
        //    };

        //    return View(response);
        //}


        //// [HttpGet("home/AddFriend/{anotherUs erId}")]
        //public async Task<IActionResult> AddFriend(string anotherUserId)
        //{
        //    var currentUser = await _userManager.GetUserAsync(User);

        //    if (User.Identity.IsAuthenticated)
        //    {
        //        ViewBag.CurrentUserName = currentUser.UserName;
        //    }

        //    var anotherUser = await _context.AspNetUsers.FindAsync(anotherUserId);

        //    //теперь добавляем в друзья (пока без требования ответного подтверждения), т.е. статус отношений устанавливаем у юсера и создаем диалог

        //    var values = new[] {currentUser.Id, anotherUserId}.OrderBy(x => x).ToArray();

        //    await _context.Entry(currentUser).Collection(x => x.Relationships).LoadAsync();

        //    //ПРОВЕРКА НА СУЩЕСТВОВАНИЕ ОТНОШЕНИЙ, ВДРУГ Я РАЗБЛОЧИДА ПОЛЬЗОВАТЕЛЯ И ХОЧУ ДОБАВИТЬ В ДРУЗЬЯ
        //    var relationship = currentUser.Relationships.FirstOrDefault(i =>
        //        i.SmallUserId.Equals(values.First(), StringComparison.OrdinalIgnoreCase) &&
        //        i.BigUserId == values.Last());

        //    var dialog = new Dialog();

        //    if (relationship != null && relationship.Status != RelationshipStatus.Friend)
        //    {
        //        relationship.Status = RelationshipStatus.Friend;
        //        //var dialogs = await _context.Dialogs.Include(x => x.Participants).ToListAsync();
        //        //dialog = dialogs.Find(n =>
        //        //    (n.Participants.First().AppUserId == currentUser.Id) &&
        //        //    (n.Participants.Last().AppUserId == anotherUserId) ||
        //        //    (n.Participants.First().AppUserId == anotherUserId) &&
        //        //    (n.Participants.Last().AppUserId == currentUser.Id));
        //    }
        //    else
        //    {
        //        currentUser.Relationships.Add(new UserRelationship()
        //        {
        //            BigUserId = values.Last(), SmallUserId = values.First()
        //        });

        //        dialog.Participants = new List<Participant>
        //        {
        //            new Participant {AppUserId = currentUser.Id, AppUserName = currentUser.UserName},
        //            new Participant {AppUserId = anotherUser.Id, AppUserName = anotherUser.UserName}
        //        };
        //        dialog.LastActivityUtc = DateTime.UtcNow;
        //    }

        //    //////await _context.Entry(anotherUser).Collection(x => x.Relationships).LoadAsync();
        //    //////anotherUser.Relationships.Add(new UserRelationship() { BigUserId = values.Last(), SmallUserId = values.First(), Id = String.Concat(values.Last(), values.First()), Status = RelationshipStatus.Friend });
        //    //////не надо ли второму пользователю добавлять этот же диалог и статус???????????????????

        //    await _context.Dialogs.AddAsync(dialog);
        //    await _context.SaveChangesAsync();

        //    //  return Ok();\
        //    return View(dialog);
        //}

        //[HttpPost("home/DeleteFriend/{anotherUserId}")]
        //public async Task<IActionResult> DeleteFriend(string anotherUserId)
        //{
        //    var currentUser = await _userManager.GetUserAsync(User);

        //    if (User.Identity.IsAuthenticated)
        //    {
        //        ViewBag.CurrentUserName = currentUser.UserName;
        //    }

        //    //статус отношений меняю на чужака у юсера, с диалогом наверное ничего не делаю, допустим в индексе перестает предлагаться друг с этим ником, но если пользователю все-таки написал тот чел,то появится диалог?

        //    var anotherUser = _context.AspNetUsers.ToList().Find(i => i.Id == anotherUserId);
        //    var values = new[] {currentUser.Id, anotherUserId}.OrderBy(x => x).ToArray();

        //    await _context.Entry(currentUser).Collection(x => x.Relationships).LoadAsync();

        //    var CURelationship =
        //        currentUser.Relationships.SingleOrDefault(i =>
        //            i.Id == String.Concat(values.First(), values.Last()));

        //    CURelationship.Status = RelationshipStatus.Stranger;
        //    //var firstVersionForAU = anotherUser.Relationships.SingleOrDefault(i => i.Id == String.Concat(values.Last(), values.First()));
        //    //firstVersionForAU.Status = RelationshipStatus.Stranger;

        //    //currentUser.Relationships.Remove(itemToRemove);

        //    await _context.SaveChangesAsync();

        //    //  return View("Searching");
        //    return Ok();
        //}

        //[HttpPost("home/Block/{anotherUserId}")]
        //public async Task<IActionResult> Block(string anotherUserId)
        //{
        //    var currentUser = await _userManager.GetUserAsync(User);

        //    if (User.Identity.IsAuthenticated)
        //    {
        //        ViewBag.CurrentUserName = currentUser.UserName;
        //    }

        //    var values = new[] {currentUser.Id, anotherUserId}.OrderBy(x => x).ToArray();

        //    await _context.Entry(currentUser).Collection(x => x.Relationships).LoadAsync();

        //    var userForBlock = currentUser.Relationships.ToList()
        //        .Find(i => i.Id == String.Concat(values.First(), values.Last()));
        //    if (userForBlock != null)
        //    {
        //        if (currentUser.Id == values.First())
        //        {
        //            userForBlock.Status = RelationshipStatus.SmallBlockBigger;
        //        }
        //        else
        //        {
        //            userForBlock.Status = RelationshipStatus.BigBlockSmaller;
        //        }

        //    }
        //    else
        //    {
        //        if (currentUser.Id == values.First())
        //        {
        //            currentUser.Relationships.Add(new UserRelationship()
        //            {
        //                BigUserId = values.Last(), SmallUserId = values.First(),
        //                Id = String.Concat(values.First(), values.Last()),
        //                Status = RelationshipStatus.SmallBlockBigger
        //            });
        //        }
        //        else
        //        {
        //            currentUser.Relationships.Add(new UserRelationship()
        //            {
        //                BigUserId = values.Last(), SmallUserId = values.First(),
        //                Id = String.Concat(values.First(), values.Last()),
        //                Status = RelationshipStatus.BigBlockSmaller
        //            });
        //        }

        //    }

        //    await _context.SaveChangesAsync();
        //    return Ok();
        //}

        //[HttpPost("home/Unblock/{anotherUserId}")]
        //public async Task<IActionResult> Unblock(string anotherUserId)
        //{
        //    var currentUser = await _userManager.GetUserAsync(User);

        //    if (User.Identity.IsAuthenticated)
        //    {
        //        ViewBag.CurrentUserName = currentUser.UserName;
        //    }

        //    var values = new[] {currentUser.Id, anotherUserId}.OrderBy(x => x).ToArray();

        //    await _context.Entry(currentUser).Collection(x => x.Relationships).LoadAsync();

        //    var userForUnblock = currentUser.Relationships.ToList()
        //        .Find(i => i.Id == String.Concat(values.First(), values.Last()));
        //    if (userForUnblock != null)
        //    {
        //        userForUnblock.Status = RelationshipStatus.Stranger;
        //    }

        //    await _context.SaveChangesAsync();
        //    return Ok();
        //}

        // [HttpGet("home/GetMessages/{dialogId}")]
        public async Task<IActionResult> GetMessages(int dialogId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.CurrentUserName = currentUser.UserName;
            }

            var dialog = await _context.Dialogs.Where(i => i.Id == dialogId).Include(x => x.Messages)
                .Include(p => p.Participants).ToArrayAsync();
            //var messages=new List<Message>();
            //foreach (var item in dialog.First().Messages.Skip(Math.Max(0, dialog.First().Messages.Count - 20)))
            //{
            //    messages.Add(item);
            //}

            var messages = dialog.First().Messages.OrderByDescending(c => c.CreatedUtc).Take(10).ToList();

           
            return PartialView("_Messages", messages);
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


        //avatars

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
