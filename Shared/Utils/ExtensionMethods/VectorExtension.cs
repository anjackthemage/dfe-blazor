using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dfe.Shared.Utils.ExtensionMethods
{
    static class VectorExtension
    {
        public static Vector2 rotate(ref this Vector2 vec, float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            float new_x = (vec.X * cos) - (vec.Y * sin);
            float new_y = (vec.X * sin) + (vec.Y * cos);
            vec.X = new_x;
            vec.Y = new_y;

            return vec;
        }
    }
}
