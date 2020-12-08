using System;
using System.IO;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
using System.Text.Json;
using dfe.Shared.Render;
using System.Numerics;
using System.Collections.Generic;
using dfe.Shared.Entities;
using System.Text.Json.Serialization;

namespace dfe.Shared
{
    public class sprite
    {
        [JsonInclude]
        public int id { get; set; }
        [JsonInclude]
        public string file { get; set; }
        [JsonInclude]
        public PixelBuffer pb_data { get; set; }
    }

    public class actor
    {
        public int type { get; set; }
        public int sprite_id { get; set; }
        public Vector2 position { get; set; }
    }

    public class Map
    {
        public int width;
        public int height;
        public int[] walls { get; set; }
        public string name { get; set; }
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
            this.entities = ref_map.entities;
            this.mobs = ref_map.mobs;
        }

        public void initMap()
        {
            //foreach (Entity ent in this.entities)
            //{
            //    ent.sprite = this.sprites[ent.sprite_id].pb_data;
            //    ent.position = new Coord(ent.position.X, ent.position.Y);
            //}

            //foreach (Mob mob in this.mobs)
            //{
            //    mob.sprite = this.sprites[mob.sprite_id].pb_data;
            //    mob.position = new Coord(mob.position.X, mob.position.Y);
            //}
        }

        public void generateMap(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.walls = new int[this.width * this.height];

            this.name = "Generated Map";

            Random rng = new Random();

            // Temporary random map generator
            for (int index = 0; index < this.width * this.height; index++)
            {
                if (rng.NextDouble() < 0.09)
                {
                    this.walls[index] = rng.Next(1, 3);
                }
                else
                {
                    this.walls[index] = 0;
                }
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
                //Console.WriteLine("Mobs: {0}", temp_map.mobs.Length);
                
                cloneMap(temp_map);

                Console.WriteLine("Loaded Map Name: {0}", this.name.ToString());

            }
            else
            {
                Console.WriteLine("Missing map file at {0}", file_path);
            }
        }

        public override string ToString()
        {
            return "W:" + width + " H:" + height + " Walls:" + walls.Length;
        }
    }
}
