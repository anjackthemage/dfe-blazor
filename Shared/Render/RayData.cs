using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dfe.Shared.Render
{
    /// <summary>
    /// Describes a ray -> map intersection.
    /// Used for detecting where a ray hits a block on the blockmap.
    /// </summary>
    public class RayData
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
        public RayData(float x_coord, float y_coord, int map_x, int map_y, bool hit, float new_d)
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
}
