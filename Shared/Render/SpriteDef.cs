using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dfe.Shared.Render
{
    /// <summary>
    /// Represents metadata about a sprite.
    /// </summary>
    public class SpriteDef
    {
        public int id;
        public PixelBuffer pixelBuffer;
        public byte alpha;
        // Server side only.
        public string filename;
        /// <summary>
        /// Creates a sprite defenition that references a given texture.
        /// </summary>
        /// <param name="texture_id"></param>
        public SpriteDef(int texture_id)
        {
            this.id = texture_id;
            alpha = 0xFF;
            pixelBuffer = null;
        }
        public SpriteDef(int texture_id, byte alpha)
        {
            this.id = texture_id;
            this.alpha = alpha;
            pixelBuffer = null;
        }
    }
}
