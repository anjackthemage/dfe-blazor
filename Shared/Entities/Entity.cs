using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dfe.Shared.Render;
using System.Numerics;
using dfe.Shared.Utils.ExtensionMethods;
using System.Text.Json.Serialization;

namespace dfe.Shared.Entities
{
    public class Entity
    {
        public int id;

        [JsonInclude]
        public Guid guid;

        [JsonInclude]
        public Coord position = new Coord();

        [JsonInclude]
        public bool b_state_has_changed = false;
        [JsonInclude]
        public int type;
        [JsonInclude]
        public int sprite_id;

        public Entity()
        {
            
        }
        public void update(float time)
        {

        }

    }
}
