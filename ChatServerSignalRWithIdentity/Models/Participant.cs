using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServerSignalRWithIdentity.Models
{
    public class Participant
    {
        public int DialogId { get; set; }
        public string AppUserId { get; set; }
        public string AppUserName { get; set; }
        public byte[] MyAvatar { get; set; }
        public virtual AppUser AppUser { get; set; }
    }
}
