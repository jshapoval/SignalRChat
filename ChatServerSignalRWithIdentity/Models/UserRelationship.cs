using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServerSignalRWithIdentity.Models
{
    public class UserRelationship
    {
        public int Id { get; set; }

        public string SmallerUserId { get; set; }
        public string BiggerUserId { get; set; }
        public RelationshipStatus Status { get; set; }
    }
}
