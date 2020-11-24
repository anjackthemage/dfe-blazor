using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dfe.Shared.Entity
{
    public struct coord
    {
        public float x;
        public float y;

        public coord(float x_new, float y_new)
        {
            this.x = x_new;
            this.y = y_new;
        }
    }
    public class Mob : Entity
    {
        public coord position { get; private set; }

        public float angle { get; private set; }

        /// <summary>
        /// Create a Mob.
        /// </summary>
        public Mob()
        {
            this.position = new coord(0, 0);
            this.angle = 0.0f;
        }

        /// <summary>
        /// Create a mob with a specific position and angle.
        /// </summary>
        /// <param name="x_initial"></param>
        /// <param name="y_initial"></param>
        /// <param name="angle_initial"></param>
        public Mob(float x_initial, float y_initial, float angle_initial)
        {
            this.position = new coord(x_initial, y_initial);
            this.angle = angle_initial;
        }

        public Mob(coord position_initial, float angle_initial)
        {
            this.position = position_initial;
            this.angle = angle_initial;
        }

        /// <summary>
        /// Rotates mob by adding given angle to current angle.
        /// </summary>
        /// <param name="angle_rot">Float containing angle in degrees.</param>
        public void rotate(float angle_rot)
        {
            this.angle += angle_rot;
            if (this.angle >= 2 * (float)Math.PI)
            {
                this.angle -= (2 * (float)Math.PI);
            }
            if (this.angle < 0)
            {
                this.angle += (2 * (float)Math.PI);
            }
        }
    }
}
