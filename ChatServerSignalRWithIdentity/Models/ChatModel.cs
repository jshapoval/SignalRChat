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
        public List<AppUserResponse> AppUserList { get; set; }//потом заменить на друзей
        public List<MessageResponse> MessagesList { get; set; }

    }
}
