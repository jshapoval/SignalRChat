using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ChatServerSignalRWithIdentity.Data;
using ChatServerSignalRWithIdentity.Data.DTO;
using ChatServerSignalRWithIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp.Formats;

namespace ChatServerSignalRWithIdentity.Services
{
    public class AccountService
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext db, UserManager<AppUser> userManager, ApplicationDbContext context)
        {
            this.db = db;
            this._userManager = userManager;
            this._context = context;
        }

      
    }
}
