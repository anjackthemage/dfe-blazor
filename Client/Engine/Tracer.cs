//using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
using dfe.Shared;
using dfe.Shared.Entity;
using System.Numerics;

namespace dfe.Client.Engine
{

    public struct input_ctrl
    {
        public bool u;
        public bool d;
        public bool l;
        public bool r;
    }

    public class Observer
    {
        public float x;
        public float y;
        public float a;

        public Observer(float x_coord, float y_coord, float angle)
        {
            this.x = x_coord;
            this.y = y_coord;
            this.a = angle;
        }
    }

    [ObsoleteAttribute("dfe.Client.Engine.level_map is deprecated. Use Map instead.")]
    public struct level_map
    {
        public int w;
        public int h;
        public float[] d;

        public level_map(int width, int height)
        {
            //Random rand = new Random();

            this.w = width;
            this.h = height;
            this.d = new float[width * height];

            //for (int index = 0; index < this.w * this.h; index++)
            //{
            //    if (rand.NextDouble() < 0.09)
            //    {
            //        this.d[index] = 1;
            //    }
            //    else
            //    {
            //        this.d[index] = 0;
            //    }
            //}
        }
    }

    /// <summary>
    /// Describes a ray -> map intersection.
    /// Used for detecting where a ray hits a block on the blockmap.
    /// </summary>
    public struct ray
    {
        // X coordinate ray hit.
        public float x;
        // Y coordinate ray hit.
        public float y;
        // The map tile that the ray hit.
        public int map_x;
        // The map tile Y
        public int map_y;
        // Bool value stating whether this ray hit or not.
        public bool hit;
        // Texture offset where the hit occured.
        public float texOfs;
        // The distance, in map units, from the cast point.
        public float dis;
        public ray(float x_coord, float y_coord, int map_x, int map_y, bool hit, float new_d)
        {
            this.x = x_coord;
            this.y = y_coord;
            this.map_x = map_x;
            this.map_y = map_y;
            this.hit = hit;
            this.texOfs = 0;
            this.dis = new_d;
        }
    }

    public class Tracer
    {
        // The framebuffer to be displayed to the user.
        public PixelBuffer frameBuffer;
        // Test texture
        public PixelBuffer tex;

        // Number of Viewport columns
        public int view_cols = 320;
        // Number of Viewport rows
        public int view_rows = 240;

        // Rate of heading change during turns.
        public const float turnRate = 0.05f;
        // Grid size in units.
        public const int grid_x = 16;
        public const int grid_y = 16;
        // Controller!
        public input_ctrl keyb = new input_ctrl();
        // Observer Mob - This should be the player.
        public Mob self = new Mob(128, 128, 0);
        // Map Object
        public Map lvl_test = new Map(16, 16);
        // Ray Data Array
        public ray[] ray_buffer;
        // Field of View
        public float fov;

        public Mob testSprite = new Mob(new Vector2(136, 128), 0);

        // Color gradient for fun :)
        public string[] colors;
        public Tracer()
        {
            frameBuffer = new PixelBuffer(view_cols, view_rows);
            tex = new PixelBuffer(16, 16);
            tex.Clear(new Color4i(0, 64, 128));
            for (int y = 0; y < 16; y++)
                for (int x = 0; x < 16; x++)
                {
                    byte b = (byte)(x * 16);
                    byte c = (byte)(y * 16);
                    tex.DrawPoint(x, y, new Color4i(0, b, c));
                }

            tex.DrawPoint(0, 0, new Color4i(255, 0, 0));
            tex.DrawPoint(2, 0, new Color4i(255, 128, 0));
            tex.DrawPoint(3, 0, new Color4i(255, 0, 0));

            // Ray buffer used for storing ray cast results.
            ray_buffer = new ray[view_cols];
            for (int index = 0; index < view_cols; index++)
            {
                ray_buffer[index] = new ray(0, 0, 0, 0, false, 0);
            }

            // Current Field of Vision
            fov = (float)Math.PI / 2.8f;

            //for (int index = 0; index < 16; index++)
            //{
            //    lvl_test.d[index] = 1;
            //    lvl_test.d[index + 240] = 1;
            //    lvl_test.d[index * 16] = 1;
            //    lvl_test.d[15 + (index * 16)] = 1;
            //}
        }

        /// <summary>
        /// Render a single frame.
        /// </summary>
        public void render()
        {
            ray_buffer = buildRayBuffer();
            renderWalls();
            renderSprites();
        }

        public void rotObserver(Observer o, float a)
        {
            o.a += a;
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
            // Lazy init of the ray_buffer
            if (ray_buffer == null)
                ray_buffer = new ray[view_cols];

            float ang_step = fov / view_cols;
            float ray_angle = self.angle - (fov / 2);
            float ang_diff = -(fov / 2);

            for (int index = 0; index < view_cols; index++)
            {
                ray_buffer[index] = rayCast(self, ray_angle);
                // Dewarp
                ray_buffer[index].dis = ray_buffer[index].dis * (float)Math.Cos(ang_diff);
                ray_angle += ang_step;
                ang_diff += ang_step;
            }
            return ray_buffer;
        }

        public void renderSprites()
        {
            // Sort sprites from furthest to closest.

            // Translate the sprites to screen space.



            float nx = (float)Math.Cos(-self.angle);
            float ny = (float)Math.Sin(-self.angle);
            float tx = (testSprite.position.X - self.position.X) / 16;
            float ty = (testSprite.position.Y - self.position.Y) / 16;
            float sx = ((tx * nx) - (ty * ny));
            float sy = ((tx * ny) + (ty * nx));

            frameBuffer.DrawPoint((int)tx + (frameBuffer.width / 2), ((int)-ty + frameBuffer.height / 2), 255, 0, 255);
            frameBuffer.DrawPoint((int)sx + (frameBuffer.width / 2), ((int)-sy + frameBuffer.height / 2), 0, 255, 0);
            frameBuffer.DrawPoint(frameBuffer.width / 2, frameBuffer.height / 2, 255, 255, 255);

            float viewAdjacent = (float)(frameBuffer.width / 2) / (float)Math.Tan(fov / 2);
            int screenX = (int)((sy / sx) * viewAdjacent);

            //float screenX = (float)((sy * (frameBuffer.width >> 1)) / (Math.Tan(fov / 2)));
            //int screenX = (int)((Math.Tan(fov / 2) * sy) * (frameBuffer.width / 2));
            frameBuffer.DrawPoint(screenX + 160, 16, 0, 255, 255);
            frameBuffer.DrawSpritePerspective((int)screenX + (frameBuffer.width / 2), sx, ray_buffer, tex);
        }
        /// <summary>
        /// renderCols()
        /// 
        /// Renders the ray_buffer to the screen buffer. 
        /// </summary>
        public void renderWalls()
        {
            frameBuffer.Clear();
            Color4i white = new Color4i(255, 255, 255);
            Color4i testColor = new Color4i(0, 128, 64);
            Fog4i fog = new Fog4i(testColor, 0.0f);
            for (int x = 0; x < frameBuffer.width; x++)
            {
                frameBuffer.ShadeTexturedWall(x, ray_buffer[x].dis, ray_buffer[x].texOfs, tex, fog);
            }
        }
        public ray rayCast(Mob obs, float ang)
        {
            Vector2 pos = obs.position;
            // Find the map cell that the observer is in.
            float obsGridX = pos.X / grid_x;
            float obsGridY = pos.Y / grid_y;

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
            while (mx >= 0 && mx < lvl_test.width && my >= 0 && my < lvl_test.height)
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
                if (lvl_test.map_contents[mx + (my * lvl_test.width)] == 1)
                {
                    hit.map_x = mx;
                    hit.map_y = my;
                    hit.hit = true;
                    break;
                }
            }

            if (hit.hit == true)
            {
                if (side == 0)
                {
                    hit.x = mx;
                    if (nx < 0) { hit.x = mx + 1; }

                    var d = hit.x - obsGridX;
                    hit.y = (d * (ny / nx)) + obsGridY;
                    hit.dis = (mx - obsGridX + (1 - stepX) / 2) / nx;
                    hit.texOfs = hit.y - (int)hit.y;
                    // Mirrored texture correction
                    if (nx < 0) { hit.texOfs = 1.0f - hit.texOfs; }
                }
                else
                {
                    hit.y = my;
                    if (ny < 0) { hit.y = my + 1; }
                    var d = hit.y - obsGridY;
                    hit.x = (d * (nx / ny)) + obsGridX;
                    hit.dis = (my - obsGridY + (1 - stepY) / 2) / ny;
                    hit.texOfs = hit.x - (int)hit.x;
                    // Mirrored texture correction
                    if (ny > 0) { hit.texOfs = 1.0f - hit.texOfs; }
                }

                hit.x = hit.x * grid_x;
                hit.y = hit.y * grid_y;
            }
            return hit;
        }

        /// <summary>
        /// Presents the screen to the user by calling the blitScreen javascript function.
        /// </summary>
        /// <param name="js"></param>
        /// 
        public void presentScreen(IJSRuntime js)
        {
            IJSUnmarshalledRuntime umjs = (IJSUnmarshalledRuntime)js;
            object result = umjs.InvokeUnmarshalled<byte[], int, int, object>("blitScreen", frameBuffer.pixels, frameBuffer.width, frameBuffer.height);
        }

        public void updateObserver()
        {

            if (keyb.u == true)
            {
                //Console.WriteLine("MOVE FORWARD");
                self.walk(1);
            }
            else if (keyb.d == true)
            {
                //Console.WriteLine("MOVE BACKWARD");
                self.walk(-1);
            }

            if (keyb.l == true)
            {
                //Console.WriteLine("TURN LEFT");
                self.rotate(-turnRate);
            }
            else if (keyb.r == true)
            {
                //Console.WriteLine("TURN RIGHT");
                self.rotate(turnRate);
            }
        }
    }
}