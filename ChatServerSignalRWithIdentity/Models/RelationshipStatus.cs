using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServerSignalRWithIdentity.Models
{
    public enum RelationshipStatus
    {
       Stranger,
       Friend,
        SmallBlockBigger,
        BigBlockSmaller,
       SmallFollowBigger,
       BigFollowSmaller
    }
}
