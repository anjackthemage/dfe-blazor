using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

namespace dfe.Shared
{
    public class Map
    {
        public static Map current;

        public int width;
        public int height;
        public float[] map_contents;

        public Map(int width, int height)
        {
            generateMap(width, height);
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
    }
}
