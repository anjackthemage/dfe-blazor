using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dfe.Shared.Render;
using System.Numerics;
using dfe.Shared.Utils.ExtensionMethods;

namespace dfe.Shared.Entities
{
    public class Entity : IRender
    {
        public Vector2 position;

        public byte[] sprite;

        public Entity()
        {

        }

        public void render()
        {
            renderSprite(this);
        }

        public void renderSprite(Entity ent_to_render) { }
    }
}
