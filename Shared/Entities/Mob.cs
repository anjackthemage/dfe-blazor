﻿using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Numerics;
using dfe.Shared.Utils.ExtensionMethods;

namespace dfe.Shared.Entities
{

    public class Mob : Entity
    {
        public static readonly Vector2 RIGHT = new Vector2(1.0f, 0.0f); // new vector(1, 0);

        // Heading of this mob - Should always be normalized.
        public Vector2 heading;
        public float angle;

        /// <summary>
        /// Create a Mob.
        /// </summary>
        public Mob()
        {
            position.X = 0;
            position.Y = 0;
            rotate(0);
        }

        /// <summary>
        /// Create a mob at specified X and Y coordinates, facing specified direction.
        /// </summary>
        /// <param name="x_initial">X coordinate where mob will be placed.</param>
        /// <param name="y_initial">Y coordinate where mob will be placed</param>
        /// <param name="angle_initial">Initial angle mob will face.</param>
        public Mob(float x_initial, float y_initial, float angle_initial)
        {
            this.position = new Coord(x_initial, y_initial);
            heading = new Vector2(1, 0);
        }

        /// <summary>
        /// Create a mob at specified position, facing specified direction.
        /// </summary>
        /// <param name="position_initial">Initial location of mob.</param>
        /// <param name="angle_initial">Initial angle mob will face.</param>
        public Mob(Coord position_initial, float angle_initial)
        {
            this.position = position_initial;
            heading = new Vector2(1, 0);
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
            heading.X = RIGHT.X;
            heading.Y = RIGHT.Y;
            heading.rotate(angle);
        }

        /// <summary>
        /// Walk along the current view angle 
        /// </summary>
        /// <param name="distance"></param>
        public void walk(float distance)
        {
            position.X += (heading.X * distance);
            position.Y += (heading.Y * distance);
        }

        public void strafe(float distance)
        {
            position.X += (-heading.Y * distance);
            position.Y += (heading.X * distance);
        }
    }
}
