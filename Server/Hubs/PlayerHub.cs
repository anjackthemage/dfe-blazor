using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using dfe.Shared.Entities;

namespace dfe.Server.Hubs
{
    public class PlayerHub : Hub
    {
        List<Mob> connected_player_list = new List<Mob>();
        public async Task registerPlayerConnection(string player_name)
        {
            
        }
    }
}
