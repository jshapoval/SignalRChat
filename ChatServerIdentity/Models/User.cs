using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ChatServerIdentity.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            Messages = new HashSet<Message>();
        }
        public int Year { get; set; }
        public virtual ICollection<Message> Messages { get; set; }

    }
}
