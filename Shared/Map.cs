﻿using System;
using System.IO;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
using System.Text.Json;
using dfe.Shared.Render;
using System.Numerics;
using System.Collections.Generic;
using dfe.Shared.Entities;

namespace dfe.Shared
{
    public class sprite
    {
        public int id { get; set; }
        public string file { get; set; }
        public PixelBuffer pb_data { get; set; }
    }

    public class texture
    {
        public int id { get; set; }
        public string file { get; set; }
        public PixelBuffer pb_data { get; set; }
    }

    public class actor
    {
        public int type { get; set; }
        public int sprite_id { get; set; }
        public Vector2 position { get; set; }
    }

    public class Map : IRender
    {
        public static Map current;

        public static bool loaded = false;

        public int width;
        public int height;
        public float[] walls { get; set; }
        public string name { get; set; }
        public sprite[] sprites { get; set; }
        public texture[] textures { get; set; }
        public Entity[] entities { get; set; }
        public Mob[] mobs { get; set; }

        public Map()
        {
            generateMap(16, 16);
        }
        public Map(int width, int height)
        {
            generateMap(width, height);
        }

        public Map(string map_name)
        {
            loadMap("Maps/" + map_name + ".json");
        }

        public Map(Map ref_map)
        {
            cloneMap(ref_map);
        }

        private void cloneMap(Map ref_map)
        {
            this.width = ref_map.width;
            this.height = ref_map.height;
            this.name = ref_map.name;
            this.walls = ref_map.walls;
            this.sprites = ref_map.sprites;
            this.textures = ref_map.textures;
            this.entities = ref_map.entities;
            this.mobs = ref_map.mobs;
        }

        public void initMap()
        {
            foreach (Entity ent in this.entities)
            {
                ent.sprite = this.textures[ent.sprite_id].pb_data;
                ent.position = new Vector2(ent.coord.X, ent.coord.Y);
            }

            foreach (Mob mob in this.mobs)
            {
                mob.sprite = this.textures[mob.sprite_id].pb_data;
                mob.position = new Vector2(mob.coord.X, mob.coord.Y);
            }
        }

        public void render()
        {
            IRender render = this;
            render.renderMap(this);

            //foreach (Entity ent in entities)
            //{
            //    Console.WriteLine("Rendering ENT");
            //    ent.render();
            //}

            //foreach (Mob mob in mobs)
            //{
            //    Console.WriteLine("Rendering MOB");
            //    Console.WriteLine("Type: {0} Sprite ID: {1} X: {2} Y: {3}", mob.type, mob.sprite_id, mob.position.X, mob.position.Y);
            //    mob.render();
            //}
        }

        public void generateMap(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.walls = new float[this.width * this.height];

            this.name = "Generated Map";

            Random rng = new Random();

            // Temporary random map generator
            for (int index = 0; index < this.width * this.height; index++)
            {
                if (rng.NextDouble() < 0.09)
                {
                    this.walls[index] = 1;
                }
                else
                {
                    this.walls[index] = 0;
                }
            }

            for (int index = 0; index < 16; index++)
            {
                this.walls[index] = 1;
                this.walls[index + 240] = 1;
                this.walls[index * 16] = 1;
                this.walls[15 + (index * 16)] = 1;
            }

        }

        public void loadMap(string file_path)
        {
            this.width = 16;
            this.height = 16;

            if (File.Exists(file_path))
            {
                string json_str = File.ReadAllText(file_path);

                Map temp_map = new Map(JsonSerializer.Deserialize<Map>(json_str));
                Console.WriteLine("Mobs: {0}", temp_map.mobs.Length);
                
                cloneMap(temp_map);

                Console.WriteLine("Loaded Map Name: {0}", this.name.ToString());

                //loadTextures();
                //loadSprites();
            }
            else
            {
                Console.WriteLine("Missing file at {0}", file_path);
            }
        }

        public byte[] loadImage(string file_path)
        {
            Stream image_fs = new FileStream(file_path, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] image_bytes = new byte[image_fs.Length];
            image_fs.Read(image_bytes);

            return image_bytes;
        }

        public void loadTextures()
        {
            foreach (texture tex in this.textures)
            {
                string file_path = "Assets/Textures/" + tex.file;
                if (File.Exists(file_path))
                {
                    tex.pb_data = new PixelBuffer(64, 64);

                    tex.pb_data.pixels = loadImage(file_path);
                }
            }

            if (!loaded)
            {
                loaded = true;
            }
        }

        public void loadSprites()
        {
            foreach(sprite spr in this.sprites)
            {
                string file_path = "Assets/Sprites/" + spr.file;
                if(File.Exists(file_path))
                {
                    spr.pb_data = new PixelBuffer(16, 16);

                    spr.pb_data.pixels = loadImage(file_path);
                }
            }

            if(!loaded)
            {
                loaded = true;
            }
        }
    }
}
