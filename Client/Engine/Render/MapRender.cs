using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dfe.Shared;

namespace dfe.Client.Engine.Render
{
    public class MapRender : Map, IRender
    {
        
        public MapRender(int width, int height) : base(width, height)
        {
            
        }

        public void render()
        {
            IRender.ray_tracer.renderLevel(this);
        }
    }
}
