using dfe.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace dfe.Shared.Render
{
    /// <summary>
    /// Camera for the player's point of view.
    /// </summary>
    public class Camera
    {
        // X position in the world.
        public float x;
        // Y position in the world.
        public float y;
        // View angle, in radians.
        public float view_angle;
        // Number of X map units per grid step.
        public readonly float grid_x = 16;
        // Number of Y map units per grid step.
        public readonly float grid_y = 16;
        // Number of pixel rows on the display.
        public readonly int view_rows;
        // Number of pixel columns on the display.
        public readonly int view_cols;
        // Field of View
        public float fov;

        public Camera(float grid_x, float grid_y, int view_cols, int view_rows, float fov)
        {
            x = 0;
            y = 0;
            view_angle = 0;
            this.grid_x = grid_x;
            this.grid_y = grid_y;
            this.view_cols = view_cols;
            this.view_rows = view_rows;
            this.fov = fov;
        }
        public void setPosition(Coord position)
        {
            x = position.X;
            y = position.Y;
            Console.WriteLine("X:" + x + " : Y" + y);
        }
    }
}
