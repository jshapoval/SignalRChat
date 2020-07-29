using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SharpCompress.Common;

namespace ChatServerSignalRWithIdentity.Models
{
    public class AppUser : IdentityUser
    {
        public virtual ICollection<Message> Messages { get; set; }
        //public virtual ICollection<UserRelationship> Relationships { get; set; }
        [ForeignKey("OwnerId")]
        public ICollection<File> Files { get; set; }
        public byte[] MyAvatar { get; set; }
        public  virtual Avatar Avatar { get; set; }
      //  public byte[] UserAvatar { get; set; }
        public bool IsDeleted { get; set; }

        public AppUser()
        {
            Messages = new HashSet<Message>(); 
           // Relationships = new HashSet<RelationshipStatus>();
            Files = new List<File>();
        }
    }
}
