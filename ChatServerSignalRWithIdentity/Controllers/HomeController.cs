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

    namespace ChatServerSignalRWithIdentity.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
       // private readonly ILogger<HomeController> _logger;
       public readonly ApplicationDbContext _context;
       public readonly UserManager<AppUser> _userManager;
       protected readonly IMapper _mapper;

        public HomeController(ApplicationDbContext context, UserManager<AppUser> userManager,IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper; 
        }

        //  public async Task<IActionResult> Index()
        //// public async Task<ApiResponse<ICollection<MessageResponse>>> Index()
        // {
        //     var currentUser = await _userManager.GetUserAsync(User);
        //     if (User.Identity.IsAuthenticated)
        //     {
        //         ViewBag.CurrentUserName = currentUser.UserName;
        //     }

        //     //var messages = await _context.Messages.ToListAsync();
        //     //return View(messages);

        //     var messages = await _context.Messages.ToListAsync();
        //     var dto =  _mapper.Map<ICollection<Message>, ICollection<MessageResponse>>(messages);
        //    // return new ApiResponse<ICollection<MessageResponse>>(dto);
        //        return View(dto);
        // }

        //public async Task<IActionResult> Index()
        //{
        //    var currentUser = await _userManager.GetUserAsync(User);
        //    if (User.Identity.IsAuthenticated)
        //    {
        //       ViewBag.CurrentUserName = currentUser.UserName;
        //    }

        //    var friends = await _context.AspNetUsers.ToListAsync();
        //    var dto = _mapper.Map<ICollection<AppUser>, ICollection<AppUserResponse>>(friends);
        //    return View(dto);
        //}

        //public async Task<IActionResult> Index()
        //// public async Task<ApiResponse<ICollection<ChatModel>>> Index()
        //{
        //    var currentUser = await _userManager.GetUserAsync(User);
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        ViewBag.CurrentUserName = currentUser.UserName;
        //    }

        //    var messages = await _context.ChatModels.ToListAsync();
        //    return View(messages);

        //    // return new ApiResponse<ICollection<MessageResponse>>(dto);
        //}


        public async Task<IActionResult> Index()
         // public async Task<ApiResponse<ICollection<ChatModel>>> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (User.Identity.IsAuthenticated) 
            {
                ViewBag.CurrentUserName = currentUser.UserName;
            }

            var users = await _context.AspNetUsers.ToListAsync(); 
            var usersDto = _mapper.Map<ICollection<AppUser>, ICollection<AppUserResponse>>(users).ToList();
            var messages = await _context.Messages.ToListAsync();
            var messagesDto = _mapper.Map<ICollection<Message>, ICollection<MessageResponse>>(messages).ToList();
          //  return View(usersDto.Zip(messagesDto,(m,u)=>Tuple.Create(m,u)));
          return View(Tuple.Create(usersDto, messagesDto));
        }


        public async Task<IActionResult> Create(Message message)
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
