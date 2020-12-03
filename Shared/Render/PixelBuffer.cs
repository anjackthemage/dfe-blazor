using dfe.Shared.Entities;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dfe.Shared.Render
{
    /// <summary>
    /// Basic container for raw pixel data.
    /// </summary>
    public class PixelBuffer
    {
        public int width;
        // Overall height of the buffer, in pixels.
        public readonly int height;
        // The stride of the buffer -or- the data size of one row of pixels in bytes.
        public readonly int stride;
        // The RGBA pixel buffer, each pixel is 4 bytes in RGBA order.
        public byte[] pixels;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="width">Overall width of the buffer, in pixels.</param>
        /// <param name="height">Overall height of the buffer, in pixels.</param>
        public PixelBuffer(int width, int height)
        {
            this.width = width;
            this.height = height;
            stride = width * 4;
            pixels = new byte[width * height * 4];
        }
    }
}
