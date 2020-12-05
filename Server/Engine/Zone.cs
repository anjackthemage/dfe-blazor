﻿using System;
using System.Collections.Generic;
using dfe.Shared;
using dfe.Shared.Entities;

namespace dfe.Server.Engine
{
    /// <summary>
    /// Container class for a game Zone
    /// </summary>
    public class Zone
    {
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
        }
    }
}