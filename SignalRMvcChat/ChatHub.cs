using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Builder;
using SignalRMvcChat.Models;

namespace SignalRMvcChat
{
   
    public class ChatHub : Hub
    {
        public async Task Registration(string email, string password)
        {
            //перекидываю ответ на клиента
            var resultMessage = UserSqlDataContext.Instance.CheckAndInsertUserInDb(email, password);
          //  //  await Clients.User(Context.UserIdentifier).SendAsync("AfterAuth", resultMessage);
            //await Clients.All.SendAsync("AfterRegist", resultMessage);
            //await Clients.All.SendAsync("Notify", $"Hi {Context.UserIdentifier}");
            //await base.OnConnectedAsync();
        }
        
      //  [Authorize]
        public async Task Send(string message, string to)
        {
            var userName = Context.User.Identity.Name;
            if (Context.UserIdentifier != to) // если получатель и текущий пользователь не совпадают
            {
                MessageSqlDataContext.Instance.InsertMessageInDb(userName, to, message);
                await Clients.User(Context.UserIdentifier).SendAsync("Receive", message, userName);
            }

            await Clients.User(to).SendAsync("Receive", message, userName);
        }

        
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("Notify", $"Hi {Context.UserIdentifier}");
            await base.OnConnectedAsync();
        }
    }
}
