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
        //public virtual ICollection<AppUser> AppUserList { get; set; }
        //public ICollection<Message> MessagesList { get; set; }

        //public ChatModel()
        //{
        //    Messages = new List<Message>();
        //    AppUsers = new List<AppUser>();
        //}
        public int Id { get; set; }
        public List<AppUserResponse> AppUserList { get; set; }
        public List<MessageResponse> MessagesList { get; set; }

        //public IEnumerable<AppUser> AppUsers
        //{
        //    get { return AppUsers.Skip(0); }
        //}

        //public IEnumerable<Message> Messages
        //{
        //    get { return Messages.Skip(0); }
        //}

        //public IEnumerable<Tuple<AppUser, Message>> Components
        //{
        //    get
        //    {
        //        return AppUserList.Zip(MessagesList, (a, b) => Tuple.Create(a, b));
        //    }
        //}
    }
}
