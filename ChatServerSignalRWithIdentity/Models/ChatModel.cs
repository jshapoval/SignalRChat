using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServerSignalRWithIdentity.Data.DTO;

namespace ChatServerSignalRWithIdentity.Models
{
    public class ChatModel
    {
        public int Id { get; set; }
        public string CallerId { get; set; }
        public List<AppUser> FriendList { get; set; }
        public List<Dialog> DialogsWithFriendsList { get; set; }
        public List<Message> MessagesList { get; set; }
    //    public List<Message> MessagesFromOneUser { get; set; }

    }
}
