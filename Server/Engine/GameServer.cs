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

        public GameServer()
        {
            server = this;
            b_is_running = true;

            doGameLoop();
        }

        private async Task doGameLoop()
        {
            while (b_is_running)
            {
                // do server stuff
            }
        }
    }
}
