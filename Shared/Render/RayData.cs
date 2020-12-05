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
        public RayData()
        {
            ax = 1;
            ay = 0;
            x = 0;
            y = 0;
            map_x = 0;
            map_y = 0;
            hit = false;
            texOfs = 0;
            dis = 0;
        }
    }
}
