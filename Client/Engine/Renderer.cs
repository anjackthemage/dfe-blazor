//using Microsoft.AspNetCore.Components;
//using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Numerics;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
using dfe.Shared;
using dfe.Shared.Render;
using dfe.Shared.Entities;

namespace dfe.Client.Engine
{

    public class Renderer
    {
        public const float GRID_X = 16;
        public const float GRID_Y = 16;
        // The framebuffer to be displayed to the user.
        public PixelBuffer frameBuffer;
        // Ray Data Array
        public RayData[] ray_buffer;
        // Texture cache for rendering.
        public Dictionary<int, PixelBuffer> textures;
        // The current viewpoint.
        public Camera camera;

        public Renderer(int screenWidth, int screenHeight)
        {
            camera = new Camera(GRID_X, GRID_Y, screenWidth, screenHeight, (float)Math.PI / 2.8f);
            frameBuffer = new PixelBuffer(screenWidth, screenHeight);

            // Generate placeholder texture
            textures = new Dictionary<int, PixelBuffer>();
            PixelBuffer tex = new PixelBuffer(16, 16);
            Render.clear(tex, new Rgba(0, 0, 0));
            for (int y = 0; y < 16; y++)
                for (int x = 0; x < 16; x++)
                {
                    byte b = (byte)(x * 16);
                    byte c = (byte)(y * 16);
                    Render.point(tex, x, y, new Rgba(0, b, c));
                }
            textures.Add(0, tex);

            // Ray buffer used for storing ray cast results.
            ray_buffer = new RayData[camera.view_cols];
            for (int index = 0; index < ray_buffer.Length; index++)
            {
                ray_buffer[index] = new RayData();
            }
        }
        public void render()
        {
            ClientState state = GameClient.game_state;

            // Update viewpoint based on location.
            camera.setPosition(state.player.position);
            camera.view_angle = state.player.angle;

            renderCeiling();
            renderFloor();

            Map map = GameClient.game_state.map;
            if (map != null)
            {
                ray_buffer = buildRayBuffer(map);
                renderWalls(map);
            }
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

        public RayData[] buildRayBuffer(Map level_map)
        {
            // Lazy init of the ray_buffer
            if (ray_buffer == null)
                ray_buffer = new RayData[camera.view_cols];

            float ang_step = camera.fov / camera.view_cols;
            float ray_angle = camera.view_angle - (camera.fov / 2);
            float ang_diff = -(camera.fov / 2);

            for (int index = 0; index < camera.view_cols; index++)
            {
                rayCast(camera, ray_angle, level_map, ray_buffer[index]);
                // Dewarp
                ray_buffer[index].dis = ray_buffer[index].dis * (float)Math.Cos(ang_diff);
                ray_angle += ang_step;
                ang_diff += ang_step;
            }
            return ray_buffer;
        }
        public void renderFloor()
        {
            PixelBuffer tex = textures[0];
            try
            {
                float obsX = -camera.x / camera.grid_x;
                float obsY = -camera.y / camera.grid_y;
                RayData leftAngles = ray_buffer[0];
                RayData rightAngles = ray_buffer[ray_buffer.Length - 1];
                for (int screenY = frameBuffer.height / 2; screenY < frameBuffer.height; screenY++)
                {
                    Render.floor(frameBuffer, screenY, 1, obsX, obsY, leftAngles, rightAngles, tex);

                }
            } catch (Exception e)
            {
                Console.Write(e.Message);
            }
            
        }
        public void renderCeiling()
        {
            Rgba ceilColor = new Rgba(0x60, 0x20, 0x00);
            float dchange = 1 / 15f;
            float distance = (frameBuffer.height / 2) * dchange;
            for (int y = frameBuffer.height / 2; y >= 0; y--)
            {
                Render.floor(frameBuffer, y, distance, ceilColor, ceilColor);
                distance -= dchange;
            }
        }

        public void renderSprite(Entity ent_to_render)
        {
            //OPTI: This code section could benefit from using some fixed point integers instead of floats.
            float nx = (float)Math.Cos(-camera.view_angle);
            float ny = (float)Math.Sin(-camera.view_angle);

            float tx = (ent_to_render.position.X - camera.x) / 16;
            float ty = (ent_to_render.position.Y - camera.y) / 16;
            float sx = ((tx * nx) - (ty * ny));
            float sy = ((tx * ny) + (ty * nx));


            //OPTI: Math.Tan(fov / 2) is a constant that only needs to be calculated once.
            float viewAdjacent = (float)(frameBuffer.width / 2) / (float)Math.Tan(camera.fov / 2);
            int screenX = (int)((sy / sx) * viewAdjacent);

            Render.sprite(frameBuffer, (int)screenX + (frameBuffer.width / 2), sx, ray_buffer, ent_to_render.sprite);
            
        }

        public void renderSprites()
        {
            // Sort sprites from furthest to closest.

            // Translate the sprites to screen space.


            //OPTI: This code section could benefit from using some fixed point integers instead of floats.
            
            //float nx = (float)Math.Cos(-self.angle);
            //float ny = (float)Math.Sin(-self.angle);
            
            //float tx = (testSprite.position.X - self.position.X) / 16;
            //float ty = (testSprite.position.Y - self.position.Y) / 16;
            //float sx = ((tx * nx) - (ty * ny));
            //float sy = ((tx * ny) + (ty * nx));

            // Debug Drawing 
            // ---
            // frameBuffer.DrawPoint((int)tx + (frameBuffer.width / 2), ((int)-ty + frameBuffer.height / 2), 255, 0, 255);
            // frameBuffer.DrawPoint((int)sx + (frameBuffer.width / 2), ((int)-sy + frameBuffer.height / 2), 0, 255, 0);
            // frameBuffer.DrawPoint(frameBuffer.width / 2, frameBuffer.height / 2, 255, 255, 255);
            // ---

            //OPTI: Math.Tan(fov / 2) is a constant that only needs to be calculated once.
            //float viewAdjacent = (float)(frameBuffer.width / 2) / (float)Math.Tan(camera.fov / 2);
            //int screenX = (int)((sy / sx) * viewAdjacent);

            //float screenX = (float)((sy * (frameBuffer.width >> 1)) / (Math.Tan(fov / 2)));
            //int screenX = (int)((Math.Tan(fov / 2) * sy) * (frameBuffer.width / 2));
            //Render.sprite(frameBuffer, (int)screenX + (frameBuffer.width / 2), 8, sx, ray_buffer, textures[0]);
        }

        /// <summary>
        /// renderCols()
        /// 
        /// Renders the ray_buffer to the screen buffer. 
        /// </summary>
        public void renderWalls(Map level_map)
        {
            RayData ray;
            for (int x = 0; x < frameBuffer.width; x++)
            {
                ray = ray_buffer[x];
                // TODO: Cleanup this little code block.
                PixelBuffer texBuffer = null;
                if (texBuffer == null)
                    texBuffer = textures[0];

                if (level_map.textures[ray.wallId].pixelBuffer != null)
                {
                    texBuffer = level_map.textures[ray.wallId].pixelBuffer;
                    Render.wallColumn(frameBuffer, x, 1, ray_buffer[x].dis, ray_buffer[x].texOfs, texBuffer, new Rgba(0x0, 0x80,0xFF, 0x08));
                }
            }
        }
        public RayData rayCast(Camera camera, float ang, Map level_map, RayData hit)
        {
            // Find the map cell that the observer is in.
            float obsGridX = camera.x / this.camera.grid_x;
            float obsGridY = camera.y / this.camera.grid_y;

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

                hit.x = hit.x * this.camera.grid_x;
                hit.y = hit.y * this.camera.grid_y;
            }
            return hit;
        }
    }
}