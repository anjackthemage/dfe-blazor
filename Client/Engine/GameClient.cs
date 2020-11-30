using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dfe.Client.Engine
{
    public class GameClient
    {

        public static GameClient client;

        public static bool b_is_running { get; private set; }

        public GameClient()
        {
            client = this;
            b_is_running = true;

            //doGameLoop();
            while (b_is_running)
            {
                // do client stuff
            }
        }

        private async Task doGameLoop()
        {
            
        }
    }
}
