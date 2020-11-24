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

    public struct vector
    {
        public float x;
        public float y;
        public vector(float x_new, float y_new)
        {
            x = x_new;
            y = y_new;
        }
        /// <summary>
        /// Normalizes this vector.
        /// </summary>
        public void normalize()
        {
            if(x != 0 || y != 0)
            {
                float d = (float)Math.Sqrt(x + y);
                x = x / d;
                y = y / d;
            }
        }

        /// <summary>
        /// Rotate this vector by the given number of radians.
        /// </summary>
        /// <param name="angle">An angle, in radians.</param>
        public void rotate(float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            float nx = (x * cos) - (y * sin);
            float ny = (x * sin) + (y * cos);
            x = nx;
            y = ny;
        }
    }

    public class Mob : Entity
    {
        public static readonly vector RIGHT = new vector(1, 0);
        public coord position;
        // Heading of this mob - Should always be normalized.
        public vector heading;
        public float angle;

        /// <summary>
        /// Create a Mob.
        /// </summary>
        public Mob()
        {
            this.position = new coord(0, 0);
            this.angle = 0.0f;
        }

        /// <summary>
        /// Create a mob at specified X and Y coordinates, facing specified direction.
        /// </summary>
        /// <param name="x_initial">X coordinate where mob will be placed.</param>
        /// <param name="y_initial">Y coordinate where mob will be placed</param>
        /// <param name="angle_initial">Initial angle mob will face.</param>
        public Mob(float x_initial, float y_initial, float angle_initial)
        {
            this.position = new coord(x_initial, y_initial);
            this.angle = angle_initial;
        }

        /// <summary>
        /// Create a mob at specified position, facing specified direction.
        /// </summary>
        /// <param name="position_initial">Initial location of mob.</param>
        /// <param name="angle_initial">Initial angle mob will face.</param>
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
            heading.x = RIGHT.x;
            heading.y = RIGHT.y;
            heading.rotate(angle);
        }

        /// <summary>
        /// Walk along the current view angle 
        /// </summary>
        /// <param name="distance"></param>
        public void walk(float distance)
        {
            position.x += (heading.x * distance);
            position.y += (heading.y * distance);
        }
        public void updatePosition()
        {
            // Assuming this is for velocity in the future? :3
        }
    }
}
