using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dfe.Shared.Entities;


namespace dfe.Shared.Render
{
    public interface IRender
    {
        public static Renderer ray_tracer { get; set; }

        public void render();

        public void renderSprite(Entity entity_to_render)
        {
            ray_tracer.renderSprite(entity_to_render);
        }

        public void renderMap(Map map_to_render)
        {
            ray_tracer.renderLevel(map_to_render);
        }
    }
}
