using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ChatServerSignalRWithIdentity.Models;
using Microsoft.AspNetCore.Http;

namespace ChatServerSignalRWithIdentity.Data.DTO
{
    public class AppUserResponse
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public string ImageId { get; set; }
        public string PhoneNumber { get; set; }
        public byte[] MyAvatar { get; set; }


        public IFormFile Avatar { get; set; }
       
    }
}
