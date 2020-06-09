using System;
using System.Collections.Generic;
using System.Configuration;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace SignalRMvcChat.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;

        private ApplicationContext _context;

        public AccountController(ApplicationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("/token")]
        public async Task<IActionResult> Token(string username, string password)
        {
            var identity = await GetIdentity(username, password);

            if (identity == null)
            {
                return BadRequest("Invalid username or password.");
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };
            return Json(response);
        }

        //public async Task<IActionResult> Get()
        //{
        //    var identity = await _context.Users.

        //    var response = new
        //    {
        //        username = identity.Name
        //    };
        //    return Json(response);
        //}

        private async Task<ClaimsIdentity> GetIdentity(string username, string password)
        {

            var person = await _userManager.FindByEmailAsync(username);


//            User person = await _context.Users.Include(r => r.Role).FirstOrDefaultAsync(x => x.Email == username && x.Password == password);
            
            if (person != null)
            {
                if (await _userManager.CheckPasswordAsync(person, password))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, person.Email),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role.Name)
                    };

                    ClaimsIdentity claimsIdentity =
                        new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                            ClaimsIdentity.DefaultRoleClaimType);

                    return claimsIdentity;
                }
            }
            // если пользователь не найден
            return null;
        }

   
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
