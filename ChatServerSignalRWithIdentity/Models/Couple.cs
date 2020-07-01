using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServerSignalRWithIdentity.Data.DTO;

namespace ChatServerSignalRWithIdentity.Models
{
    public class Couple
    {
        public AppUserResponse MyUser { get; set; }
        public AppUserResponse OtherUser { get; set; }
    }
}
