using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServerSignalRWithIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatServerSignalRWithIdentity.Hubs
{
    public class ChatHub : Hub
    {
        [Authorize]
        public async Task SendMessageToPublicChatToPublicChat(Message message) =>
            await Clients.All.SendAsync("receiveMessageToPublicChat", message);


        public async Task SendMessageToPrivateChat(Message message, string to)
        {
            var userName = Context.User.Identity.Name;
            if (Context.UserIdentifier != to) // если получатель и текущий пользователь не совпадают
            {
                await Clients.User(Context.UserIdentifier).SendAsync("receiveMessageToPrivateChat", message, userName);
            }

            await Clients.User(to).SendAsync("receiveMessageToPrivateChat", message, userName);
        }


    }
}
