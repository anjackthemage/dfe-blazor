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
        public Dictionary<Guid, Mob> local_mobs;
        public Dictionary<Guid, Entity> local_props;
        public Map map;
        public ClientState() {
            player = new Player(128,128, 0);
            local_mobs = new Dictionary<Guid, Mob>();
            local_props = new Dictionary<Guid, Entity>();
        }

        public void setMap(Map map)
        {
            Console.WriteLine("Received map.");
            this.map = map;
        }
    }
}
