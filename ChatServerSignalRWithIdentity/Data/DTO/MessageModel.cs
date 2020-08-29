using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServerSignalRWithIdentity.Models
{
    public class MessageModel
    {
        public int Id { get; set; }
        public int DialogId { get; set; }
        public string UserId { get; set; }
       
        public string Text { get; set;}
      

        public MessageModel()
        {
        }
    }
}
