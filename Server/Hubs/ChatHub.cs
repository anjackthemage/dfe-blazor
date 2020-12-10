using System;
//using System.Collections.Generic;
//using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace dfe.Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task sendMessage(string user, string message)
        {
            Console.WriteLine("{0}: {1}", user, message);
            await Clients.All.SendAsync("receiveMessage", user, message);
        }
    }
}
