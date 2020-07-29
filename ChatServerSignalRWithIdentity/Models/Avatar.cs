using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServerSignalRWithIdentity.Models
{
    public class Avatar
    {
        public int Id { get; set; }
        public bool Default { get; set; }
        //
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public string OwnerId { get; set; }
        //    public byte[] Image { get; set; }

        public File Original { get; set; }
        public File Square { get; set; }
        public File Square_100 { get; set; }
        public File Square_300 { get; set; }
        public File Square_600 { get; set; }

        public Guid? OriginalId { get; set; }
        public Guid? SquareId { get; set; }
        public Guid? Square_100Id { get; set; }
        public Guid? Square_300Id { get; set; }
        public Guid? Square_600Id { get; set; }
    }

}
