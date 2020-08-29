using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ChatServerSignalRWithIdentity.Data;
using ChatServerSignalRWithIdentity.Data.DTO;
using ChatServerSignalRWithIdentity.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatServerSignalRWithIdentity
{
    public class DialogService
    {
        ApplicationDbContext db;

        public DialogService(ApplicationDbContext db, IMapper _mapper)
        {
            this.db = db;
        }
        public async Task<Dialog> GetDialogBetweenUsers(string[] users)
        {
            string dipper = users[0], mabel = users[1];

            var user1 = db.Users.Find(dipper);
            var user2 = db.Users.Find(mabel);

            if (user1 != null && user2 != null)
            {
                var dialog = db.Dialogs
                    .Include(x => x.Participants)
                    .Include(x => x.Messages).FirstOrDefault(d =>
                        d.Participants.Any(p => p.AppUserId == user1.Id) &&
                        d.Participants.Any(p => p.AppUserId == user2.Id));

                if (dialog == null)
                {
                    //await db.Entry(user1).Reference(x => x.Avatar).LoadAsync();
                    //await db.Entry(user2).Reference(x => x.Avatar).LoadAsync();

                    dialog = new Dialog()
                    {
                        Participants = new List<Participant>()
                        {
                            new Participant() {AppUserId = user1.Id},
                            new Participant() {AppUserId = user2.Id}
                        }
                    };

                    db.Dialogs.Add(dialog);
                    await db.SaveChangesAsync();
                }

                return dialog;
            }
            return null;
        }
        public async Task MarkMessagesAsRead(List<Message> messages)
        {
            foreach (var m in messages)
            {
                var msg = await db.Messages.FindAsync(m.Id);
                msg.Read = true;
            }

            await db.SaveChangesAsync();
        }

        public async Task<ICollection<Message>> GetMessagesForDialog(Dialog dialog)
        {
            await db.Entry(dialog).Collection(i => i.Messages).LoadAsync();
            return dialog.Messages;
        }
     
        public async Task WriteMessage(Dialog dialog, Message message)
        {
            if (dialog != null)
            {
                await db.Entry(dialog).Collection(r => r.Messages).LoadAsync();
                dialog.Messages.Add(message);
                await db.SaveChangesAsync();
            }
        }
    }
}
