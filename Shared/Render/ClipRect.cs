using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dfe.Shared.Render
{
    /// <summary>
    /// Clipping rectangle structure.
    /// Values are stored in screen pixel coordinates.
    /// </summary>
    public struct ClipRect
    {
        public int x;
        public int y;
        public int w;
        public int h;
        public int left;
        public int right;
        public int top;
        public int bot;
        public ClipRect(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
            left = x;
            right = x + w;
            top = y;
            bot = y + h;
        }

        public override string ToString()
        {
            return "x:" + x + " y:" + y + " w:" + w + " h:" + h + " left:" + left + " right:" + right + " top:" + top + " bot:" + bot;
        }
    }
}
