using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ChatServerSignalRWithIdentity.Models
{
    public class AppUser : IdentityUser
    {
        public virtual ICollection<Message> Messages { get; set; }
   //     public virtual ICollection<RelationshipStatus> Relationships { get; set; }
        [ForeignKey("OwnerId")]
        public ICollection<File> Files { get; set; }

        public virtual Avatar Avatar { get; set; }
        public bool IsDeleted { get; set; }

        public AppUser()
        {
            Messages = new HashSet<Message>(); 
           // Relationships = new HashSet<RelationshipStatus>();
            Files = new List<File>();
        }
    }
}
