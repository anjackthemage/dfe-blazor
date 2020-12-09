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
        // Server side only - Filename of the sprite bitmap.
        public string filename;
        // Id number of this sprite def
        [JsonInclude]
        public int id;
        // Alpha value for this sprite
        [JsonInclude]
        public byte alpha;
        // Scale adjustment for this sprite.
        [JsonInclude]
        public float scale = 1;
        // Pixel buffer for rendering.
        [JsonInclude]
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
        /// <param name="sprite_id">ID number of this sprite.</param>
        public SpriteDef(int id)
        {
            this.id = id;
            alpha = 0xFF;
            pixelBuffer = null;
        }
        /// <summary>
        /// Server Side Constructor - A sprite with no alpha blending.
        /// </summary>
        /// <param name="sprite_id">ID number of this sprite.</param>
        /// <param name="filename">Filename of the source texture data.</param>
        public SpriteDef(int id, string filename)
        {
            this.id = id;
            this.filename = filename;
            alpha = 0xFF;
            pixelBuffer = null;
        }

        /// <summary>
        /// Server Side Constructor - A sprite with alpha blending.
        /// </summary>
        /// <param name="texture_id"></param>
        /// <param name="alpha"></param>
        public SpriteDef(int id, byte alpha, string filename)
        {
            this.id = id;
            this.alpha = alpha;
            this.filename = filename;
            pixelBuffer = null;
        }

        /// <summary>
        /// Create a sprite defention with alpha parameter set.
        /// </summary>
        /// <param name="texture_id">ID number of this sprite.</param>
        /// <param name="alpha">Overall alpha value for this sprite.</param>
        public SpriteDef(int id, byte alpha)
        {
            this.id = id;
            this.alpha = alpha;
            pixelBuffer = null;
        }
        /// <summary>
        /// Creata a sprite defenition with no alpha.
        /// </summary>
        /// <param name="id">ID number of this sprite.</param>
        /// <param name="pixelBuffer">Pixelbuffer for this sprite.</param>
        public SpriteDef(int id, PixelBuffer pixelBuffer)
        {
            this.id = id;
            this.alpha = 0xFF;
            this.scale = 1;
            this.pixelBuffer = pixelBuffer;
        }
    }
}
