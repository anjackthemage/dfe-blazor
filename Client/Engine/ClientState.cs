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
        public Dictionary<int, Mob> local_mobs;
        public Dictionary<int, Entity> local_props;
        public Map map;
        public ClientState() {
            player = new Player(128,128, 0);
            local_mobs = new Dictionary<int, Mob>();
            local_props = new Dictionary<int, Entity>();
        }

        public void setMap(Map map)
        {
            Mob mob = new Mob(new Coord(136, 136), 0);
            mob.sprite_id = 1;
            local_props.Add(1024, mob);

            Console.WriteLine("Received map.");
            this.map = map;
            for (int y = 0; y < 256; y += 8)
            {
                local_props.Add(y, new Mob(new Coord(24, y), 0));
                local_mobs.Add(y, new Mob(new Coord(128, y), 0));
            }
        }
    }
}
