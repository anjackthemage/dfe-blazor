using dfe.Shared;
using dfe.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dfe.Client.Engine
{
    public class ClientState
    {
        public Player player;
        public Map map;
        public ClientState() {
            player = new Player();
            player.position.X = 10;
            player.position.Y = 8;
            player.angle = 1;
        }
    }
}
