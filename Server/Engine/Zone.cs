using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dfe.Shared;
using dfe.Shared.Entities;
using dfe.Shared.Render;

namespace dfe.Server.Engine
{
    /// <summary>
    /// Container class for a game Zone
    /// </summary>
    public class Zone
    {
        #region init
        public Guid guid;
        public Dictionary<Guid, Player> local_players;
        public Dictionary<Guid, Mob> local_mobs;
        public Dictionary<Guid, Entity> local_props;
        public Map map;

        public Zone() {
            guid = Guid.NewGuid();
            local_players = new Dictionary<Guid, Player>();
            local_mobs = new Dictionary<Guid, Mob>();
            local_props = new Dictionary<Guid, Entity>();

            // Default zone settings.
            // Generate a template map.
            map = new Map("level_test");
            initZone();
        }

        public void initZone()
        {
            Console.WriteLine("Initializing zone: {0}", this.guid);
        }
        #endregion

        #region simulation
        /// <summary>
        /// Function to handle processing non-render calculations for each object in this zone.
        /// </summary>
        public async Task update()
        {
            // Cycle through all the mobs and ents in the map
            //      process movement
            //      process ai
            //      process environmental effects?
            //Console.WriteLine("Simulating zone: {0}", this.guid);

            // Check for updated entities

        }
        #endregion

    }
}
