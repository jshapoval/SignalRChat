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
        private readonly ApplicationDbContext _context;
        public ChatHub( ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task SendMessageToPublicChat(Message message) =>
            await Clients.All.SendAsync("receiveMessageToPublicChat", message);

        public async Task FindFriendInBase(string friendLogin) =>
            await Clients.All.SendAsync("receiveMessageToPublicChat", friendLogin);


        //public async Task SendMessageToPrivateChat(Message message, string to)
        //{
        //    var userName = Context.User.Identity.Name;
        //    if (Context.UserIdentifier != to) // если получатель и текущий пользователь не совпадают
        //    {
        //        await Clients.User(Context.UserIdentifier).SendAsync("receiveMessageToPrivateChat", message, userName);
        //    }

        //    await Clients.User(to).SendAsync("receiveMessageToPrivateChat", message, userName);
        //}

        //public async Task SendMessage(int dialogId, string data, string ownerEncKey, string recipientEncKey)
        //{
        //    var currentUserId = Context.UserIdentifier;

        //    var dialog = await _context.Dialogs
        //        .Include(x => x.Participants).FirstOrDefaultAsync(x =>
        //            x.Id.Equals(dialogId) );//&& x.Status == DialogStatus.Active

        //    if (dialog == null)
        //        return;

        //    var currentUser = dialog.Participants.FirstOrDefault(x => x.Id.Equals(currentUserId));

        //    if (currentUser == null)
        //        return;

        //    var anotherUser = dialog.Participants.FirstOrDefault(x =>
        //        !x.Id.Equals(currentUser.Id));

        //    var message = new Message
        //    {
        //        Text = data,
        //        CreatedUtc = DateTime.UtcNow,
        //        OwnerId = currentUserId
        //    };

        //    await _dialogService.WriteMessage(dialog, message);

        //    dialog.LastActivityUtc = DateTime.UtcNow;
        //    _context.Dialogs.Update(dialog);
        //    await _context.SaveChangesAsync();

        //    await Clients.Users(dialog.Participants.Select(x => x.Id).ToList())
        //        .SendAsync("Notify", message);
        //}
    }
}
