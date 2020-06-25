using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServerSignalRWithIdentity.Data.DTO
{
    public class ParticipantResponse
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public string ImageId { get; set; }
    }
}
