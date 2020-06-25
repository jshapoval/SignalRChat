using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServerSignalRWithIdentity.Models
{
    public class Participant
    {
      //  public int  Id { get; set; }
        public int DialogId { get; set; }

        [Required]
        public string AppUserId { get; set; }
        public AppUser Sender { get; set; }
      //  public bool KeyReceived { get; set; }
    }
}
