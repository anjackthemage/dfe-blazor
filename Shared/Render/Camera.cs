using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dfe.Shared.Render
{
    /// <summary>
    /// Camera for the player's point of view.
    /// </summary>
    public class Camera
    {
        public float x;
        public float y;
        public float a;

        public Camera(float x_coord, float y_coord, float angle)
        {
            this.x = x_coord;
            this.y = y_coord;
            this.a = angle;
        }
    }
}
