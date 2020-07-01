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

        public HomeController(ApplicationDbContext context, UserManager<AppUser> userManager,IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper; 
        }

       public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (User.Identity.IsAuthenticated) 
            {
                ViewBag.CurrentUserName = currentUser.UserName;
            }

            var users = await _context.AspNetUsers.ToListAsync(); 
            var messages = await _context.Messages.ToListAsync();
            var dialogs = await _context.Dialogs.ToListAsync();

          var response = new ChatModel
          {
              AppUserList = _mapper.Map<List<AppUser>>(users),
              MessagesList = _mapper.Map<List<Message>>(messages),
              DialogsList = _mapper.Map<List<Dialog>>(dialogs)
          };

          return View(response);
        }


        public async Task<IActionResult> Send(MessageModel messageModel)
        {
            if (ModelState.IsValid)
            {
                var dialog = await _context.Dialogs.FindAsync(messageModel.DialogId);

                var myUser = await _userManager.GetUserAsync(User);
                var anotherUser = await _userManager.FindByIdAsync(messageModel.UserId);

                if (dialog is null)
                {
                    dialog = new Dialog
                    {
                        Participants = new List<Participant>
                            {new Participant {AppUserId = myUser.Id, AppUserName = myUser.UserName}, new Participant {AppUserId = anotherUser.Id, AppUserName = anotherUser.UserName}}
                    };

                    await _context.Dialogs.AddAsync(dialog);
                    await _context.SaveChangesAsync();
                }

                var message = _mapper.Map<Message>(messageModel);
                
                message.UserName = myUser.UserName;
                message.SenderId = myUser.Id;
                message.DialogId = dialog.Id;

                //var dialog = _context.Dialogs.FirstOrDefaultAsync(x=>x.Participants.Any(y=>y.AppUserId == sender.Id) && x.Participants.Any(y => y.AppUserId == sender.Id))
                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();

                return Ok();
            }

            return Error();
        }


        public async Task<IActionResult> Searching(string username)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.CurrentUserName = currentUser.UserName;
            }

            var user =  _context.AspNetUsers.ToList().Find(i => i.UserName == username) ;
         
            var newFriend = _mapper.Map<AppUserResponse>(user);
            var myUser = _mapper.Map<AppUserResponse>(currentUser);

            var response = new Couple()
            {
                MyUser = myUser,
                OtherUser = newFriend
            };

                return View(response);
        }

        

        public async Task<IActionResult> AddFriend(string anotherUserId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.CurrentUserName = currentUser.UserName;
            }

            var anotherUser = _context.AspNetUsers.ToList().Find(i => i.Id == anotherUserId);

            //теперь добавляем в друзья (пока без требования ответного подтверждения), т.е. статус отношений устанавливаем у юсера и создаем диалог

            var values = new[] {currentUser.Id, anotherUserId}.OrderBy(x => x).ToArray();

            await _context.Entry(currentUser).Collection(x => x.Relationships).LoadAsync();

            //ПРОВЕРКА НА СУЩЕСТВОВАНИЕ ОТНОШЕНИЙ, ВДРУГ Я РАЗБЛОЧИДА ПОЛЬЗОВАТЕЛЯ И ХОЧУ ДОБАВИТЬ В ДРУЗЬЯ
            var relationship = currentUser.Relationships.ToList().Find(i => i.Id == String.Concat(values.First(), values.Last()));
            if (relationship != null)
            {
                relationship.Status = RelationshipStatus.Friend;
            }

            else
            {
                currentUser.Relationships.Add(new UserRelationship() { BigUserId = values.Last(), SmallUserId = values.First(), Id = String.Concat(values.First(), values.Last()), Status = RelationshipStatus.Friend });
            }


            //////await _context.Entry(anotherUser).Collection(x => x.Relationships).LoadAsync();
            //////anotherUser.Relationships.Add(new UserRelationship() { BigUserId = values.Last(), SmallUserId = values.First(), Id = String.Concat(values.Last(), values.First()), Status = RelationshipStatus.Friend });
            //////не надо ли второму пользователю добавлять этот же диалог и статус???????????????????

            var dialog = new Dialog
                {
                    Participants = new List<Participant>
                        {
                            new Participant {AppUserId = currentUser.Id, AppUserName = currentUser.UserName}, 
                            new Participant {AppUserId = anotherUser.Id, AppUserName = anotherUser.UserName}
                        }
                };

                await _context.Dialogs.AddAsync(dialog);
                await _context.SaveChangesAsync();

            //  return Ok();\
            return View("Searching");
            //return View(dialog);
        }

        public async Task<IActionResult> DeleteFriend(string anotherUserId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.CurrentUserName = currentUser.UserName;
            }

            //статус отношений меняю на чужака у юсера, с диалогом наверное ничего не делаю, допустим в индексе перестает предлагаться друг с этим ником, но если пользователю все-таки написал тот чел,то появится диалог?

            var anotherUser = _context.AspNetUsers.ToList().Find(i => i.Id == anotherUserId);
            var values = new[] { currentUser.Id, anotherUserId }.OrderBy(x => x).ToArray();

            await _context.Entry(currentUser).Collection(x => x.Relationships).LoadAsync();

            var CURelationship =
                currentUser.Relationships.SingleOrDefault(i => i.Id == String.Concat(values.First(), values.Last()));

                CURelationship.Status = RelationshipStatus.Stranger;
                //var firstVersionForAU = anotherUser.Relationships.SingleOrDefault(i => i.Id == String.Concat(values.Last(), values.First()));
                //firstVersionForAU.Status = RelationshipStatus.Stranger;

            //currentUser.Relationships.Remove(itemToRemove);
            
            await _context.SaveChangesAsync();

            return View("Searching");
        }


        public async Task<IActionResult> Block(string anotherUserId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.CurrentUserName = currentUser.UserName;
            }

            var values = new[] { currentUser.Id, anotherUserId }.OrderBy(x => x).ToArray();

            await _context.Entry(currentUser).Collection(x => x.Relationships).LoadAsync();

            var userForBlock = currentUser.Relationships.ToList().Find(i => i.Id == String.Concat(values.First(), values.Last()));
            if (userForBlock!=null)
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
                    currentUser.Relationships.Add(new UserRelationship() { BigUserId = values.Last(), SmallUserId = values.First(), Id = String.Concat(values.First(), values.Last()), Status = RelationshipStatus.SmallBlockBigger });
                }
                else
                {
                    currentUser.Relationships.Add(new UserRelationship() { BigUserId = values.Last(), SmallUserId = values.First(), Id = String.Concat(values.First(), values.Last()), Status = RelationshipStatus.BigBlockSmaller });
                }
               
            }

            await _context.SaveChangesAsync();
            return View("Searching");
        }

        public async Task<IActionResult> Unblock(string anotherUserId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.CurrentUserName = currentUser.UserName;
            }

            var values = new[] { currentUser.Id, anotherUserId }.OrderBy(x => x).ToArray();

            await _context.Entry(currentUser).Collection(x => x.Relationships).LoadAsync();

            var userForUnblock = currentUser.Relationships.ToList().Find(i => i.Id == String.Concat(values.First(), values.Last()));
            if (userForUnblock != null)
            {
                userForUnblock.Status = RelationshipStatus.Stranger;
            }
           
            await _context.SaveChangesAsync();
            return View("Searching");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
