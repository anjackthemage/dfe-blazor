using System;
//using System.Collections.Generic;
//using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using dfe.Shared;
using System.IO;
using dfe.Server.Engine;

namespace dfe.Server.Hubs
{
    public class MapHub : Hub
    {
        public async Task getMap()
        {
            Map map = GameServer.server.world[0].map;
            await Clients.Caller.SendAsync("receiveMap", map);
        }
    }
}
