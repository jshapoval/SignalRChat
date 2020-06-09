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
using ChatServerIdentity.Models;

namespace ChatServerIdentity
{
   
    public class ChatHub : Hub
    {
        [Authorize]
        public async Task Send(string message)
        {
            await this.Clients.All.SendAsync("Send", message);
        }

    }
}
