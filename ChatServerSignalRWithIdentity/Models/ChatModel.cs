﻿using System;
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
        public List<AppUser> AppUserList { get; set; }//потом заменить на друзей 
        public List<Dialog> DialogsList { get; set; }
        public List<Message> MessagesList { get; set; }

    }
}