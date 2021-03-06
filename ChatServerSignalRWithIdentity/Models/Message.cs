﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServerSignalRWithIdentity.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int DialogId { get; set; }
        public string SenderId { get; set; }
        public string OwnerId { get; set; }
        public string SenderUserName { get; set; }

        public string OwnerUserName { get; set; }
        [Required]
        public string Text { get; set;}
        public DateTime CreatedUtc { get; set; }
   
        public  virtual AppUser Sender { get; set; }

        public virtual AppUser Owner { get; set; }
        public bool Read { get; set; }

        public Message()
        {
            CreatedUtc = DateTime.Now;
        }
    }
}
