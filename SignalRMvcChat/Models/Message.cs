using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
 
namespace SignalRMvcChat.Models
{
    public class Message
    {
        public int Id { get; set; }
     //   public int IdFrom { get; set; }
        public int IdTo { get; set; }
        public string Text { get; set; }
    }
}