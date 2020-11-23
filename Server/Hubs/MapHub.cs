﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using dfe.Shared;

namespace dfe.Server.Hubs
{
    public class MapHub : Hub
    {
        public async Task getMap()
        {

            Console.WriteLine("Sending map!");
            await Clients.Caller.SendAsync("receiveMap", Map.current.map_contents);
            Console.WriteLine("Map sent!");
        }
    }
}