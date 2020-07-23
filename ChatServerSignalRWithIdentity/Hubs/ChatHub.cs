using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ChatServerSignalRWithIdentity.Data;
using ChatServerSignalRWithIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatServerSignalRWithIdentity.Hubs
{
    [Authorize]
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

       
       //public async Task SendMessage(Message message) =>
       //     await Clients.All.SendAsync("receiveMessage", message);

        public async Task SendMessage(string text, int dialogId)
        {
            var currentUserId = Context.UserIdentifier;
            var myUser = _context.AspNetUsers.FindAsync(Context.UserIdentifier).Result;

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

          //  var message = _mapper.Map<Message>(messageModel);
          //??????????????преобразовать ли сообщение????????????????????????????????????????????????????????????????
            var message = new Message
            {
                Text = text,
                CreatedUtc = DateTime.UtcNow,
                OwnerId = currentUserId,
                SenderId = anotherUser.AppUserId,
                DialogId = dialogId,
                UserName = myUser.UserName
            };

            await _dialogService.WriteMessage(dialog, message);

            dialog.LastActivityUtc = DateTime.UtcNow;
            dialog.Messages.Add(message);
            dialog.LastMessageId = message.Id;

            _context.Dialogs.Update(dialog);

            await _context.SaveChangesAsync();

            await Clients.Users(dialog.Participants.Select(x => x.AppUserId).ToList())
                .SendAsync("GetMessage");
        }
    }
}
