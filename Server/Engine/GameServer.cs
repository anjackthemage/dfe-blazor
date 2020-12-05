using dfe.Shared;
using dfe.Shared.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        }

        private async Task doGameLoop()
        {
            
        }
    }
}
