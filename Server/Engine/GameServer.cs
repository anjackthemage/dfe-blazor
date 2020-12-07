using dfe.Shared;
using dfe.Shared.Render;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.DependencyInjection;

namespace dfe.Server.Engine
{
    public class GameServer
    {

        public static GameServer server;

        public static bool b_is_running { get; private set; }

        public Dictionary<int, Zone> world;
        public Dictionary<int, Texture> texture_assets;
        public Dictionary<int, sprite> sprite_assets;

        public GameServer()
        {
            world = new Dictionary<int, Zone>();
            texture_assets = new Dictionary<int, Texture>();
            sprite_assets = new Dictionary<int, sprite>();

            Zone zone = new Zone();
            world.Add(0, zone);

            server = this;
            b_is_running = true;

            doGameLoop();
        }

        private async Task doGameLoop()
        {
            while (b_is_running)
            {
                // This will hold all of the simulate tasks we're about to create
                Task[] zone_sims = new Task[world.Count];

                // Call `simulation` code on each zone in world:
                foreach (KeyValuePair<int, Zone> element in world)
                {
                    Task zone_sim_task = element.Value.simulate();  // assign task
                    zone_sims[element.Key] = zone_sim_task;         // put task in array
                }

                Task.WhenAll(zone_sims); // Wait for all tasks to complete before releasing thread

                // Pause execution on this thread to let main thread process for a bit
                await Task.Delay(1000);
            }
        }
    }
}
