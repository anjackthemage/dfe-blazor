using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dfe.Shared.Render
{
    /// <summary>
    /// Represents metadata about a sprite.
    /// </summary>
    public class SpriteDef
    {
        [JsonInclude]
        public int id;
        [JsonInclude]
        public byte alpha;
        // Server side only.
        [JsonInclude]
        public string filename;

        public PixelBuffer pixelBuffer;
        /// <summary>
        /// Default Constructor
        /// </summary>
        public SpriteDef()
        {
            id = 0;
            alpha = 0xFF;
            pixelBuffer = null;
        }
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
