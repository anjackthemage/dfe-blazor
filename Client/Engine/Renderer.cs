using System;
using System.Collections.Generic;
using dfe.Shared;
using dfe.Shared.Render;
using dfe.Shared.Entities;
using System.Numerics;

namespace dfe.Client.Engine
{
    public class Renderer
    {
        public const float GRID_X = 16;
        public const float GRID_Y = 16;
        // The framebuffer to be displayed to the user.
        public PixelBuffer frame_buffer;
        // Ray Data Array
        public RayData[] ray_buffer;
        
        // Texture cache for rendering.
        public Dictionary<int, TextureDef> textures;
        // Sprite cache for rendering
        public Dictionary<int, SpriteDef> sprites;
        public SpriteDef default_sprite;
        public TextureDef default_texture;

        public Dictionary<int, bool> missing_sprites;
        public Dictionary<int, bool> missing_textures;

        // The current viewpoint.
        public Camera camera;

        public Renderer(int screenWidth, int screenHeight)
        {
            camera = new Camera(GRID_X, GRID_Y, screenWidth, screenHeight, (float)Math.PI / 2.8f);
            frame_buffer = new PixelBuffer(screenWidth, screenHeight);

            // Generate placeholder texture
            textures = new Dictionary<int, TextureDef>();
            sprites = new Dictionary<int, SpriteDef>();
            missing_sprites = new Dictionary<int, bool>();
            missing_textures = new Dictionary<int, bool>();

            PixelBuffer tex = new PixelBuffer(16, 16);
            Render.clear(tex, new Rgba(0, 0, 0));
            for (int y = 0; y < 16; y++)
                for (int x = 0; x < 16; x++)
                {
                    byte b = (byte)(x * 16);
                    byte c = (byte)(y * 16);
                    Render.point(tex, x, y, new Rgba(0, b, c));
                }
            default_sprite = new SpriteDef(0, tex);
            default_texture = new TextureDef(0, tex);
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

            Map map = GameClient.game_state.map;
            ray_buffer = buildRayBuffer(camera, map);
            renderCeiling();
            renderFloor();
            renderWalls(map);
            renderSprites(camera);

            // Request missing textures
            foreach(int id in missing_textures.Keys)
            {
                if(missing_textures[id] == false)
                {
                    // Request the texture
                    GameClient.client.requestTexturePixels(id);
                    // Flag it as requested.
                    missing_textures[id] = true;
                }
            }

            foreach(int id in missing_sprites.Keys)
            {
                if(missing_sprites[id] == false)
                {
                    // Request the sprite
                    GameClient.client.requestSpritePixels(id);
                    // Flag it as requested.
                    missing_sprites[id] = true;
                }
            }
        }
        bool once = false;
        public List<SpriteVis> sprite_vis;
        public void renderSprites(Camera cam)
        {
            if (sprite_vis == null)
                sprite_vis = new List<SpriteVis>();
            sprite_vis.Clear();

            // Setup culling space.


            Coord origin = new Coord(cam.x, cam.y);
            float angle = cam.view_angle - (cam.fov / 2);
            Vector2 left = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            angle = cam.view_angle + (cam.fov / 2);
            Vector2 right= new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

            int spritecount = 0;
            int cullcount = 0;
            // Cull all sprites out of the space.
            ClientState state = GameClient.game_state;
            foreach (Entity ent in state.local_props.Values)
            {
                spritecount++;
                if (((left.X * (ent.position.Y - origin.Y)) - (left.Y * (ent.position.X - origin.X))) > 0
                && ((right.X * (ent.position.Y - origin.Y)) - (right.Y * (ent.position.X - origin.X))) < 0)
                {
                    sprite_vis.Add(new SpriteVis(ent));
                }
                else
                {
                    cullcount++;
                }
            }

            foreach (Entity ent in state.local_mobs.Values)
            {
                spritecount++;
                if (((left.X * (ent.position.Y - origin.Y)) - (left.Y * (ent.position.X - origin.X))) > 0
                && ((right.X * (ent.position.Y - origin.Y)) - (right.Y * (ent.position.X - origin.X))) < 0)
                {
                    sprite_vis.Add(new SpriteVis(ent));
                }
                else
                {
                    cullcount++;
                }
            }

            // Perform translation calculations on the resulting list.
            //OPTI: This code section could benefit from using some fixed point integers instead of floats.
            float nx = (float)Math.Cos(-camera.view_angle);
            float ny = (float)Math.Sin(-camera.view_angle);
            //OPTI: Math.Tan(fov / 2) is a constant that only needs to be calculated once.
            float viewAdjacent = (float)(frame_buffer.width / 2) / (float)Math.Tan(camera.fov / 2);
            foreach (SpriteVis spr_vis in sprite_vis)
            {
                float tx = (spr_vis.x - camera.x) / 16;
                float ty = (spr_vis.y - camera.y) / 16;
                float sx = ((tx * nx) - (ty * ny));
                float sy = ((tx * ny) + (ty * nx));
                int screen_x = (int)((sy / sx) * viewAdjacent);
                spr_vis.screen_x = screen_x;
                spr_vis.distance = sx;
            }

            // Sort rendering order from back to front.
            sprite_vis.Sort(delegate (SpriteVis a, SpriteVis b)
            {
                if (a.distance == b.distance) return 0;
                else if (a.distance > b.distance) return -1;
                else return 1;
            });
            // Render the list.
            foreach (SpriteVis spr_vis in sprite_vis)
            {
                SpriteDef sprite = sprites[0];
                //PixelBuffer buffer = textures[0].pixelBuffer;
                if (sprites.TryGetValue(spr_vis.sprite_id, out sprite) == true && sprite.pixelBuffer != null)
                {
                    Render.sprite(frame_buffer, (int)spr_vis.screen_x + (frame_buffer.width / 2), spr_vis.distance, ray_buffer, sprite);
                }
                else
                {
                    Render.sprite(frame_buffer, (int)spr_vis.screen_x + (frame_buffer.width / 2), spr_vis.distance, ray_buffer, default_sprite);
                    // If the sprite is to be requested, or being downloaded, dont request it.
                    if (missing_sprites.ContainsKey(sprite.id) == false)
                        missing_sprites.Add(sprite.id, false);
                }
            }
        }

        public RayData[] buildRayBuffer(Camera cam, Map level_map)
        {
            float ang_step = cam.fov / cam.view_cols;
            float ray_angle = cam.view_angle - (cam.fov / 2);
            float ang_diff = -(cam.fov / 2);

            for (int index = 0; index < cam.view_cols; index++)
            {
                rayCast(cam, ray_angle, level_map, ray_buffer[index]);
                // Dewarp
                ray_buffer[index].dis = ray_buffer[index].dis * (float)Math.Cos(ang_diff);
                ray_angle += ang_step;
                ang_diff += ang_step;
            }

            return ray_buffer;
        }
        public void renderFloor()
        {
            TextureDef tex = textures[0];
            float obsX = -camera.x / camera.grid_x;
            float obsY = -camera.y / camera.grid_y;
            RayData leftAngles = ray_buffer[0];
            RayData rightAngles = ray_buffer[ray_buffer.Length - 1];
            for (int screenY = frame_buffer.height / 2; screenY < frame_buffer.height; screenY++)
            {
                Render.floor(frame_buffer, screenY, 1, obsX, obsY, leftAngles, rightAngles, default_texture.pixelBuffer);

            }
        }
        public void renderCeiling()
        {
            Rgba ceilColor = new Rgba(0x60, 0x20, 0x00);
            float dchange = 1 / 15f;
            float distance = (frame_buffer.height / 2) * dchange;
            for (int y = frame_buffer.height / 2; y >= 0; y--)
            {
                Render.floor(frame_buffer, y, distance, ceilColor, ceilColor);
                distance -= dchange;
            }
        }

        /// <summary>
        /// renderCols()
        /// 
        /// Renders the ray_buffer to the screen buffer. 
        /// </summary>
        public void renderWalls(Map level_map)
        {
            RayData ray;
            for (int x = 0; x < frame_buffer.width; x++)
            {
                ray = ray_buffer[x];
                TextureDef tex = textures[0];
                if (textures.TryGetValue(ray.texture_id, out tex) == true && tex.pixelBuffer != null)
                {
                    Render.wallColumn(frame_buffer, x, 1, ray.dis, ray.texOfs, tex.pixelBuffer, new Rgba(0x0, 0x80, 0xFF, 0x08));
                }  else  {
                    Render.wallColumn(frame_buffer, x, 1, ray.dis, ray.texOfs, default_texture.pixelBuffer, new Rgba(0x0, 0x80, 0xFF, 0x08));
                    // If the sprite is to be requested, or being downloaded, dont request it.
                    if (missing_textures.ContainsKey(ray.texture_id) == false)
                        missing_textures.Add(ray.texture_id, false);
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
                    hit.texture_id = (int)level_map.walls[mx + (my * level_map.width)];
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