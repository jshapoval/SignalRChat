using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServerSignalRWithIdentity.Data.DTO
{
    public class DialogResponse
    {
        public int Id { get; set; }
        public bool Status { get; set; }
        public DateTime LastActivityUtc { get; set; }
        public virtual ICollection<ParticipantResponse> Participants { get; set; }
    }
}
