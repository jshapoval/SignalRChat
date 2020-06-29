using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServerSignalRWithIdentity.Models
{
    public class UserRelationship
    {
        public string Id { get; set; }
        public string SmallUserId { get; set; }
        public string BigUserId { get; set; }
        public RelationshipStatus Status { get; set; }
    }
}
