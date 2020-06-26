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


        public async Task<IActionResult> Create(MessageModel messageModel)
        {
            if (ModelState.IsValid)
            {
                var dialog = await _context.Dialogs.FindAsync(messageModel.DialogId);

                var sender = await _userManager.GetUserAsync(User);
                var anotherUser = await _userManager.FindByIdAsync(messageModel.UserId);

                if (dialog is null)
                {
                    dialog = new Dialog
                    {
                        Participants = new List<Participant>
                            {new Participant {AppUserId = sender.Id, AppUserName = sender.UserName}, new Participant {AppUserId = anotherUser.Id, AppUserName = anotherUser.UserName}}
                    };

                    await _context.Dialogs.AddAsync(dialog);
                    await _context.SaveChangesAsync();
                }

                var message = _mapper.Map<Message>(messageModel);
                
                message.UserName = sender.UserName;
                message.SenderId = sender.Id;
                message.DialogId = dialog.Id;

                //var dialog = _context.Dialogs.FirstOrDefaultAsync(x=>x.Participants.Any(y=>y.AppUserId == sender.Id) && x.Participants.Any(y => y.AppUserId == sender.Id))
                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();

                return Ok();
            }

            return Error();
        }


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

        public async Task<IActionResult> Searching(string friendLogin)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.CurrentUserName = currentUser.UserName;
            }

            var user =  _context.AspNetUsers.ToList().Find(i => i.UserName == friendLogin) ;
            var newFriend = _mapper.Map<AppUserResponse>(user);
            
            return View(newFriend);
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

    }
}
