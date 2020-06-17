﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServerSignalRWithIdentity.Models
{
    public class Avatar
    {
        public int Id { get; set; }
        public bool Default { get; set; }
        public File Original { get; set; }
        public File Square { get; set; }
        public File Square_100 { get; set; }
        public File Square_300 { get; set; }
        public File Square_600 { get; set; }
    }

}