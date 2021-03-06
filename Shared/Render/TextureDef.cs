﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dfe.Shared.Render
{
    /// <summary>
    /// Texture Defenition
    /// </summary>
    public class TextureDef
    {
        // Lookup Id number for this texture.
        public int id { get; set; }
        // Filename for this texture, used on the server side.
        public string file { get; set; }
        // The pixel buffer loaded into this texture.
        public PixelBuffer pixelBuffer { get; set; }
        /// <summary>
        /// Default Constructor
        /// </summary>
        public TextureDef()
        {
            id = 0;
        }
        /// <summary>
        /// Server side constructor.
        /// </summary>
        /// <param name="id">Texture ID</param>
        /// <param name="filename">Source filename for this texture's bitmap.</param>
        public TextureDef(int id, string filename)
        {
            this.id = id;
            this.file = filename;
        }
        /// <summary>
        /// Client Side Constructor 
        /// </summary>
        /// <param name="id">Texture ID</param>
        /// <param name="pixelBuffer">PixelBuffer that is downloaded from the server.</param>
        public TextureDef(int id, PixelBuffer pixelBuffer)
        {

            this.id = id;
            this.pixelBuffer = pixelBuffer;
        }

        public bool isValid()
        {
            if (pixelBuffer == null
            || pixelBuffer.pixels == null
            || pixelBuffer.width * pixelBuffer.height * 4 != pixelBuffer.pixels.Length)
                return false;
            return true;
        }

        public override string ToString()
        {
            return "Texture Id :" + id + " Filename :" + file + " PixelBuffer:" + pixelBuffer;
        }
    }
}
