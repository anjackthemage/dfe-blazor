using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace dfe.Client.Engine.Render
{
    public interface IRender
    {
        public static Tracer ray_tracer { get; set; }

        public void render();
    }
}
