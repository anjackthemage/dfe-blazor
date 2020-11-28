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

        public PixelBuffer sprite { get; set; }

        public Entity()
        {
            //sprite = IRender.ray_tracer.s_tex;
        }

        public void render()
        {
            IRender render = this;
            render.renderSprite(this);
        }
    }
}
