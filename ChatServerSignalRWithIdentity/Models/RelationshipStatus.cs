﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServerSignalRWithIdentity.Models
{
    public enum RelationshipStatus
    {
        Friend,
        SmallBlockBigger,
        BigBlockSmaller,
        SmallFollowBigger,
        BigFollowSmaller
    }
}
