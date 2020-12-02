//using Microsoft.AspNetCore.Components;
//using Microsoft.JSInterop;
using System;
using System.Numerics;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
using dfe.Shared;
using dfe.Shared.Entities;

namespace dfe.Shared.Render
{

    public struct input_ctrl
    {
        public bool u;
        public bool d;
        public bool l;
        public bool r;
        public float mouseDelta;
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

    /// <summary>
    /// Describes a ray -> map intersection.
    /// Used for detecting where a ray hits a block on the blockmap.
    /// </summary>
    public class Ray
    {
        // X angle of the ray.
        public float ax;
        // Y angle of the ray.
        public float ay;
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
        // Texture index of hit.
        public int wallId;
        // Texture offset where the hit occured.
        public float texOfs;
        // The distance, in map units, from the cast point.
        public float dis;
        public Ray(float x_coord, float y_coord, int map_x, int map_y, bool hit, float new_d)
        {
            this.ax = 1; 
            this.ay = 0; 
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
        public PixelBuffer s_tex;
        public PixelBuffer f_tex;

        // Number of Viewport columns
        public int view_cols = 320;
        // Number of Viewport rows
        public int view_rows = 240;
        // Fog color of the current render
        public fog4i fogColor = new fog4i(new color4i(0xFFFFFFFF), 0.0f);
        // Rate of heading change during turns.
        public const float turnRate = 0.05f;
        // Grid size in units.
        public const int grid_x = 16;
        public const int grid_y = 16;
        // Controller!
        public input_ctrl input = new input_ctrl();
        // Observer Mob - This should be the player.
        public Player self = new Player(128, 128, 0);
        // Map Object
        public Map lvl_map = new Map(16, 16);
        // Ray Data Array
        public Ray[] ray_buffer;
        // Field of View
        public float fov;

        public Mob testSprite = new Mob(new Vector2(136, 128), 0);

        // Color gradient for fun :)
        public string[] colors;
        public Map big_map;
        public Tracer()
        {
            IRender.ray_tracer = this;

            frameBuffer = new PixelBuffer(view_cols, view_rows);
            tex = new PixelBuffer(16, 16);
            s_tex = new PixelBuffer(16, 16);
            tex.Clear(new color4i(0, 64, 128));
            s_tex.Clear(new color4i(0, 64, 128));
            for (int y = 0; y < 16; y++)
                for (int x = 0; x < 16; x++)
                {
                    byte b = (byte)(x * 16);
                    byte c = (byte)(y * 16);
                    tex.DrawPoint(x, y, new color4i(0, b, c));
                }

            tex.DrawPoint(0, 0, new color4i(255, 0, 0));
            tex.DrawPoint(2, 0, new color4i(255, 128, 0));
            tex.DrawPoint(3, 0, new color4i(255, 0, 0));

            // Floor test texture
            f_tex = new PixelBuffer(64, 64);
            for(int y = 0; y < 64; y++)
            {
                for(int x =0; x < 64; x++)
                {
                    f_tex.DrawPoint(x, y, new color4i((byte)(x << 2), 0, (byte)(y << 2)));
                }
            }
            // Ray buffer used for storing ray cast results.
            ray_buffer = new Ray[view_cols];
            for (int index = 0; index < view_cols; index++)
            {
                ray_buffer[index] = new Ray(0, 0, 0, 0, false, 0);
            }

            // Current Field of Vision
            fov = (float)Math.PI / 2.8f;

        }
        /// <summary>
        /// Render the current Level
        /// </summary>
        /// <param name="level_map">The map to perform raytracing against.</param>
        public void renderLevel(Map level_map)
        {
            ray_buffer = buildRayBuffer(level_map);
            renderFloor();
            renderCeiling();
            renderWalls(level_map);
            renderSprites();
            Render.rectDepth(frameBuffer, 80, 2, 128, 64, ray_buffer);
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

        public Ray[] buildRayBuffer(Map level_map)
        {
            // Lazy init of the ray_buffer
            if (ray_buffer == null)
                ray_buffer = new Ray[view_cols];

            float ang_step = fov / view_cols;
            float ray_angle = self.angle - (fov / 2);
            float ang_diff = -(fov / 2);

            for (int index = 0; index < view_cols; index++)
            {
                rayCast(self, ray_angle, level_map, ray_buffer[index]);
                // Dewarp
                ray_buffer[index].dis = ray_buffer[index].dis * (float)Math.Cos(ang_diff);
                ray_angle += ang_step;
                ang_diff += ang_step;
            }
            return ray_buffer;
        }
        public void renderFloor()
        {
            try
            {
                float obsX = -self.position.X / grid_x;
                float obsY = -self.position.Y / grid_y;
                Ray leftAngles = ray_buffer[0];
                Ray rightAngles = ray_buffer[ray_buffer.Length - 1];
                for (int screenY = frameBuffer.height / 2; screenY < frameBuffer.height; screenY++)
                {
                    frameBuffer.TexturedRow(screenY, 1, obsX, obsY, leftAngles, rightAngles, tex
                        
                        );
                }
            } catch (Exception e)
            {
                Console.Write(e.Message);
            }
            /*
            color4i floorColor = new color4i(0xFF808080);
            float dchange = 1 / 15f;
            float distance = (frameBuffer.height / 2) * dchange;
            for (int y = frameBuffer.height / 2; y < frameBuffer.height; y++)
            {
                frameBuffer.ShadeRow(y, distance, floorColor, fogColor);
                distance -= dchange;
            }*/
            
        }
        public void renderCeiling()
        {
            color4i ceilColor = new color4i(0xFF602000);
            float dchange = 1 / 15f;
            float distance = (frameBuffer.height / 2) * dchange;
            for (int y = frameBuffer.height / 2; y >= 0; y--)
            {
                frameBuffer.ShadeRow(y, distance, ceilColor, fogColor);
                distance -= dchange;
            }
        }

        public void renderSprite(Entity ent_to_render)
        {
            //OPTI: This code section could benefit from using some fixed point integers instead of floats.
            float nx = (float)Math.Cos(-self.angle);
            float ny = (float)Math.Sin(-self.angle);

            float tx = (ent_to_render.position.X - self.position.X) / 16;
            float ty = (ent_to_render.position.Y - self.position.Y) / 16;
            float sx = ((tx * nx) - (ty * ny));
            float sy = ((tx * ny) + (ty * nx));


            //OPTI: Math.Tan(fov / 2) is a constant that only needs to be calculated once.
            float viewAdjacent = (float)(frameBuffer.width / 2) / (float)Math.Tan(fov / 2);
            int screenX = (int)((sy / sx) * viewAdjacent);

            frameBuffer.DrawPoint(screenX + 160, 16, 0, 255, 255);
            frameBuffer.DrawSpritePerspective((int)screenX + (frameBuffer.width / 2), sx, ray_buffer, ent_to_render.sprite);
            
        }

        public void renderSprites()
        {
            // Sort sprites from furthest to closest.

            // Translate the sprites to screen space.


            //OPTI: This code section could benefit from using some fixed point integers instead of floats.
            float nx = (float)Math.Cos(-self.angle);
            float ny = (float)Math.Sin(-self.angle);
            
            float tx = (testSprite.position.X - self.position.X) / 16;
            float ty = (testSprite.position.Y - self.position.Y) / 16;
            float sx = ((tx * nx) - (ty * ny));
            float sy = ((tx * ny) + (ty * nx));

            // Debug Drawing 
            // ---
            // frameBuffer.DrawPoint((int)tx + (frameBuffer.width / 2), ((int)-ty + frameBuffer.height / 2), 255, 0, 255);
            // frameBuffer.DrawPoint((int)sx + (frameBuffer.width / 2), ((int)-sy + frameBuffer.height / 2), 0, 255, 0);
            // frameBuffer.DrawPoint(frameBuffer.width / 2, frameBuffer.height / 2, 255, 255, 255);
            // ---

            //OPTI: Math.Tan(fov / 2) is a constant that only needs to be calculated once.
            float viewAdjacent = (float)(frameBuffer.width / 2) / (float)Math.Tan(fov / 2);
            int screenX = (int)((sy / sx) * viewAdjacent);

            //float screenX = (float)((sy * (frameBuffer.width >> 1)) / (Math.Tan(fov / 2)));
            //int screenX = (int)((Math.Tan(fov / 2) * sy) * (frameBuffer.width / 2));
            frameBuffer.DrawPoint(screenX + 160, 16, 0, 255, 255);
            frameBuffer.DrawSpritePerspective((int)screenX + (frameBuffer.width / 2), 8, sx, ray_buffer, tex);
        }

        /// <summary>
        /// renderCols()
        /// 
        /// Renders the ray_buffer to the screen buffer. 
        /// </summary>
        public void renderWalls(Map level_map)
        {
            Ray ray;
            for (int x = 0; x < frameBuffer.width; x++)
            {
                ray = ray_buffer[x];
                // TODO: Cleanup this little code block.
                PixelBuffer texBuffer = null;
                if (ray.wallId == 1)
                    texBuffer = f_tex;
                else if (ray.wallId == 2)
                    texBuffer = s_tex;

                if (level_map.textures[ray.wallId].pb_data != null)
                {
                    texBuffer = level_map.textures[ray.wallId].pb_data;

                    // frameBuffer.ShadeTexturedWall(x, 1, ray.dis, ray.texOfs, level_map.getWallTexture(ray.wallId).pb_data, fogColor);
                    // frameBuffer.ShadeTexturedWall(x, 1, ray.dis, ray.texOfs, texBuffer, fogColor);
                    Render.wallColumn(frameBuffer, x, ray_buffer[x].dis);
                }
            }
        }
        public Ray rayCast(Mob obs, float ang, Map level_map, Ray hit)
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

            // Store the ray vector for floor casting later.
            hit.ax = nx;
            hit.ay = ny;

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

            var side = 0;
            while (mx >= 0 && mx < level_map.width && my >= 0 && my < level_map.height)
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
                if (level_map.walls[mx + (my * level_map.width)] > 0)
                {
                    hit.wallId = (int)level_map.walls[mx + (my * level_map.width)];
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
        //public void presentScreen(IJSRuntime js)
        //{
        //    IJSUnmarshalledRuntime umjs = (IJSUnmarshalledRuntime)js;
        //    object result = umjs.InvokeUnmarshalled<byte[], int, int, object>("blitScreen", frameBuffer.pixels, frameBuffer.width, frameBuffer.height);
        //}

        public void updateObserver()
        {

            if (input.u == true)
            {
                self.walk(0.5f);
            }
            else if (input.d == true)
            {
                self.walk(-0.5f);
            }

            if (input.l == true)
            {
                self.strafe(-0.5f);
            }
            else if (input.r == true)
            {
                self.strafe(0.5f);
            }

            if (input.mouseDelta != 0)
            {
                self.rotate(input.mouseDelta / 64);
                input.mouseDelta = 0;
            }
        }
    }
}