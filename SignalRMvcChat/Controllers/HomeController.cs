using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using SignalRMvcChat.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using SignalRMvcChat.Views;

namespace SignalRMvcChat.Controllers
{
    public class HomeController : Controller
    {
        FilesContext _context;

        public HomeController(FilesContext context)
        {
            _context = context;
        }
        public IActionResult Registration ()
        {
            var h = _context.People.ToList();
            return View((object)h);
            //(_context.People.ToList());
        }

        [HttpPost]
        public IActionResult Create(UserViewModel uvm)
        {
            var user = new User { Email = uvm.Email, Password =  uvm.Password};

            _context.People.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Registration");
        }
    }
}