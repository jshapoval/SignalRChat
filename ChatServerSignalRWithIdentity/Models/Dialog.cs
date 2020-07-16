using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServerSignalRWithIdentity.Data.DTO;

namespace ChatServerSignalRWithIdentity.Models
{
    public class Dialog
    {
        public int Id { get; set; }
        public bool Status { get; set; }
        public int LastMessageId { get; set; }
        public virtual ICollection<Participant> Participants { get; set; }
        public ICollection<Message> Messages { get; set; }
        public DateTime LastActivityUtc { get; set; }

        public Dialog()
        {
            Messages = new List<Message>();
            Participants = new List<Participant>();
        }
    }
}
