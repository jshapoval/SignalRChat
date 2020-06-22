using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ChatServerSignalRWithIdentity.Models;

namespace ChatServerSignalRWithIdentity.Data.DTO
{
    public class AppUserResponse
    {
        //     public virtual ICollection<RelationshipStatus> Relationships { get; set; }
        public string Id { get; set; }
        public string Login { get; set; }
        public string ImageId { get; set; }

        //public AppUserResponse()
        //{
        //    // Relationships = new HashSet<RelationshipStatus>();
        //}
    }
}
