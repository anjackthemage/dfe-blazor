using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dfe.Client.Engine
{

    public struct input_ctrl
    {
        public bool u;
        public bool d;
        public bool l;
        public bool r;
    }

    public struct observer
    {
        public float x;
        public float y;
        public float a;

        public observer(float x_coord, float y_coord, float angle)
        {
            this.x = x_coord;
            this.y = y_coord;
            this.a = angle;
        }
    }

    public struct level_map
    {
        public int w;
        public int h;
        public float[] d;

        public level_map(int width, int height)
        {
            Random rand = new Random();

            this.w = width;
            this.h = height;
            this.d = new float[width * height];

            for (int index = 0; index < this.w * this.h; index++)
            {
                if (rand.NextDouble() < 0.09)
                {
                    this.d[index] = 1;
                }
                else
                {
                    this.d[index] = 0;
                }
            }
        }
    }

    public struct ray
    {
        public float x;
        public float y;
        public int mx;
        public int my;
        public bool h;
        public float d;

        public ray(float x_coord, float y_coord, int map_x, int map_y, bool hit, float new_d)
        {
            this.x = x_coord;
            this.y = y_coord;
            this.mx = map_x;
            this.my = map_y;
            this.h = hit;
            this.d = new_d;
        }
    }

    public class Tracer
    {

        // Grid size in units.
        public const int grid_x = 16;
        public const int grid_y = 16;
        // Controller!
        public input_ctrl keyb = new input_ctrl();
        // Observer object.
        public observer obs = new observer(128.5f, 128.5f, 0.0f);
        // Map Object
        public level_map lvl_test = new level_map(16, 16);
        // Number of Viewport columns
        public int view_cols = 256;
        // Ray Data Array
        public ray[] ray_buffer;
        // Field of View
        public float fov;

        // Color gradient for fun :)
        public string[] colors;

        public Tracer()
        {
            ray_buffer = new ray[view_cols];
            for (int index = 0; index < view_cols; index++)
            {
                ray_buffer[index] = new ray(0, 0, 0, 0, false, 0);
            }

            fov = (float)Math.PI / 4;

            for (int index = 0; index < 16; index++)
            {
                lvl_test.d[index] = 1;
                lvl_test.d[index + 240] = 1;
                lvl_test.d[index * 16] = 1;
                lvl_test.d[15 + (index * 16)] = 1;
            }

            colors = new string[64];
            for (int index = 0; index < 64; index++)
            {
                int c = ((256 / 64) * index) >> 0;
                colors[63 - index] = "rgb(" + c.ToString() + ", " + c.ToString() + ", " + c.ToString() + ")";
            }
        }

        public void rotObserver(observer o, float a)
        {
            Console.WriteLine("Starting rotation: {0}", o.a);
            o.a += a;
            Console.WriteLine("Ending rotation: {0}", o.a);

            if (o.a >= 2 * (float)Math.PI)
            {
                o.a -= (2 * (float)Math.PI);
            }
            if (o.a < 0)
            {
                o.a += (2 * (float)Math.PI);
            }
        }

        public ray[] buildRayBuffer()
        {
            ray[] rayBuffer = new ray[256];

            float ang_step = fov / 256;

            float ray_angle = obs.a - (fov / 2);

            for (int index = 0; index < 256; index++)
            {
                rayBuffer[index] = rayCast(obs, ray_angle);
                ray_angle += ang_step;
            }

            return rayBuffer;
        }

        public ray rayCast(observer obs, float ang)
        {
            // Find the map cell that the observer is in.
            float obsGridX = obs.x / grid_x;
            float obsGridY = obs.y / grid_y;

            // the '>> 0' is a method to floor to an integer.
            int mx = (int)obsGridX; //obsGridX << 0;
            int my = (int)obsGridY; //obsGridY << 0;

            // Calculate the unit vector
            float nx = (float)Math.Cos(ang);
            float ny = (float)Math.Sin(ang);

            // Length of the ray to the next x or y intersection
            float deltaX = 0;
            float deltaY = 0;

            // Handle vertical and horizontal vectors.
            if (nx == 0) { deltaY = 1; }
            else if (ny == 0) { deltaY = 1; }
            else
            {
                deltaX = (float)Math.Abs(1 / nx);
                deltaY = (float)Math.Abs(1 / ny);
            }

            // Calc step directions and initial step for our cast.
            int stepX = 0;
            int stepY = 0;
            float iDeltaX = 0;
            float iDeltaY = 0;

            if (nx < 0)
            {
                stepX = -1;
                iDeltaX = (obsGridX - mx) * deltaX;
            }
            else
            {
                stepX = 1;
                iDeltaX = (1 - (obsGridX - mx)) * deltaX;
            }

            if (ny < 0)
            {
                stepY = -1;
                iDeltaY = (obsGridY - my) * deltaY;
            }
            else
            {
                stepY = 1;
                iDeltaY = (1 - (obsGridY - my)) * deltaY;
            }

            var hit = new ray(0, 0, 0, 0, false, 0);

            var side = 0;
            while (mx >= 0 && mx < lvl_test.w && my >= 0 && my < lvl_test.h)
            {
                if (iDeltaX < iDeltaY)
                {
                    iDeltaX += deltaX;
                    mx += stepX;
                    side = 0;
                }
                else
                {
                    iDeltaY += deltaY;
                    my += stepY;
                    side = 1;
                }
                if (lvl_test.d[mx + (my * lvl_test.w)] == 1)
                {
                    hit.mx = mx;
                    hit.my = my;
                    hit.h = true;
                    break;
                }
            }

            if (hit.h == true)
            {
                if (side == 0)
                {
                    hit.x = mx;
                    if (nx < 0) { hit.x = mx + 1; }

                    var d = hit.x - obsGridX;
                    hit.y = (d * (ny / nx)) + obsGridY;
                    hit.d = (mx - obsGridX + (1 - stepX) / 2) / nx;

                }
                else
                {
                    hit.y = my;
                    if (ny < 0) { hit.y = my + 1; }
                    var d = hit.y - obsGridY;
                    hit.x = (d * (nx / ny)) + obsGridX;
                    hit.d = (my - obsGridY + (1 - stepY) / 2) / ny;
                }

                hit.x = hit.x * grid_x;
                hit.y = hit.y * grid_y;
            }
            return hit;
        }

        public void updateObserver()
        {

            if (keyb.u == true)
            {
                //Console.WriteLine("MOVE FORWARD");
                obs.x += (float)Math.Cos(obs.a);
                obs.y += (float)Math.Sin(obs.a);
            }
            else if (keyb.d == true)
            {
                //Console.WriteLine("MOVE BACKWARD");
                obs.x -= (float)Math.Cos(obs.a);
                obs.y -= (float)Math.Sin(obs.a);
            }

            if (keyb.l == true)
            {
                //Console.WriteLine("TURN LEFT");
                rotObserver(obs, -0.25f);
            }
            else if (keyb.r == true)
            {
                //Console.WriteLine("TURN RIGHT");
                rotObserver(obs, 0.25f);
            }
        }
    }
}
