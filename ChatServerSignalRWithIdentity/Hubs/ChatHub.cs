using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServerSignalRWithIdentity.Data;
using ChatServerSignalRWithIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatServerSignalRWithIdentity.Hubs
{
    public class ChatHub : Hub
    {
        private readonly DialogService _dialogService;

        private static readonly HashSet<string> ConnectedUsers = new HashSet<string>();

        private readonly ApplicationDbContext _context;
      
        public ChatHub( ApplicationDbContext context, DialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;
        }

        [Authorize]

        public async Task GetNewMessage(Message message)
        {
           var owner = message.OwnerId;
            Clients.User(owner).GetMessage(message);
          //  await Clients.User(owner).SendAsync("GetMessage", message);
        }
        public async Task SendMessageToPublicChat(Message message) =>
            await Clients.All.SendAsync("receiveMessageToPublicChat", message);



        //public async Task SendMessageToPrivateChat(Message message, string to)
        //{
        //    var userName = Context.User.Identity.Name;
        //    if (Context.UserIdentifier != to) // если получатель и текущий пользователь не совпадают
        //    {
        //        await Clients.User(Context.UserIdentifier).SendAsync("receiveMessageToPrivateChat", message, userName);
        //    }

        //    await Clients.User(to).SendAsync("receiveMessageToPrivateChat", message, userName);
        //}

        public async Task Send(string text, int dialogId)
        {
            var currentUserId = Context.UserIdentifier;

            var dialog = await _context.Dialogs
                .Include(x => x.Participants).Include(m=>m.Messages).FirstOrDefaultAsync(x =>
                    x.Id.Equals(dialogId));

            if (dialog == null)
                return;

            var currentUser = dialog.Participants.FirstOrDefault(x => x.AppUserId.Equals(currentUserId));

            if (currentUser == null)
                return;

            var anotherUser = dialog.Participants.FirstOrDefault(x =>
                !x.AppUserId.Equals(currentUser.AppUserId));

            var message = new Message
            {
                Text = text,
                CreatedUtc = DateTime.UtcNow,
                OwnerId = currentUserId,
                SenderId = anotherUser.AppUserId
            };

            await _dialogService.WriteMessage(dialog, message);

            dialog.LastActivityUtc = DateTime.UtcNow;
            _context.Dialogs.Update(dialog);
            await _context.SaveChangesAsync();

            await Clients.Users(dialog.Participants.Select(x => x.AppUserId).ToList())
                .SendAsync("GetMessages", message);
        }
    }
}
