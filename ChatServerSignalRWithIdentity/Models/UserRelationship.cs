using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServerSignalRWithIdentity.Models
{
    public class UserRelationship
    {
        public int Id { get; set; }
        public int SmallUserId { get; set; }
        public int BigUserId { get; set; }
        public RelationshipStatus Status { get; set; }
    }
}
