using System;
using System.IO;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
using System.Text.Json;
using dfe.Shared.Render;

namespace dfe.Shared
{
    public class sprite
    {
        public string name { get; set; }
        public int id { get; set; }
        public string file { get; set; }
    }

    public class level
    {
        public string name { get; set; }
        public float[] map { get; set; }
        public sprite[] sprites { get; set; }
    }

    public class Map : IRender
    {
        private string level_path = "Maps/level_test.json";

        public static Map current;

        public int width;
        public int height;
        public float[] map_contents;

        public Map()
        {
        }
        public Map(int width, int height)
        {
            generateMap(width, height);
        }


        public void render()
        {
            IRender render = this;
            render.renderMap(this);
        }

        public void generateMap(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.map_contents = new float[this.width * this.height];

            Random rng = new Random();

            // Temporary random map generator
            for (int index = 0; index < this.width * this.height; index++)
            {
                if (rng.NextDouble() < 0.09)
                {
                    this.map_contents[index] = 1;
                }
                else
                {
                    this.map_contents[index] = 0;
                }
            }

            for (int index = 0; index < 16; index++)
            {
                this.map_contents[index] = 1;
                this.map_contents[index + 240] = 1;
                this.map_contents[index * 16] = 1;
                this.map_contents[15 + (index * 16)] = 1;
            }

        }

        public void loadMap()
        {
            this.width = 16;
            this.height = 16;

            if (File.Exists(level_path))
            {
                string json_str = File.ReadAllText(level_path);
                level test_lvl = JsonSerializer.Deserialize<level>(json_str);
                this.map_contents = test_lvl.map;
                Console.WriteLine("Level name: {0}", test_lvl.name.ToString());
                
            }
            else
            {
                Console.WriteLine("Missing file at {0}", level_path);
            }
        }
    }
}
