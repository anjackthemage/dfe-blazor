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
    public class Coord
    {
        [JsonInclude]
        public float X;
        [JsonInclude]
        public float Y;
    }
    public class Entity : IRender
    {
        [JsonInclude]
        public Coord coord;

        public Vector2 position;
        
        public PixelBuffer sprite { get; set; }

        [JsonInclude]
        public int type;
        [JsonInclude]
        public int sprite_id;

        public Entity()
        {
            
        }

        public void render()
        {
            IRender render = this;
            render.renderSprite(this);
        }
    }
}
