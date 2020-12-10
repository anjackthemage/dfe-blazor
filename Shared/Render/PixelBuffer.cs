using dfe.Shared.Entities;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.IO;
using System.IO.Compression;

namespace dfe.Shared.Render
{
    /// <summary>
    /// Basic container for raw pixel data.
    /// </summary>
    public class PixelBuffer
    {
        [JsonInclude]
        public readonly int width;
        // Overall height of the buffer, in pixels.
        [JsonInclude]
        public readonly int height;
        // The stride of the buffer -or- the data size of one row of pixels in bytes.
        [JsonInclude]
        public readonly int stride;
        // The RGBA pixel buffer, each pixel is 4 bytes in RGBA order.
        [JsonInclude]
        public byte[] pixels;
        // Returns true if this pixel buffer is compressed.
        public bool is_compressed { get { return (stride * height) != pixels.Length; } }
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

        public void compressPixels()
        {
            if (pixels == null)
            {
                Console.WriteLine("No Pixel Data to Compress");
                return;
            }
            if (is_compressed == true) return;
            MemoryStream memStream = new MemoryStream();
            GZipStream gzip = new GZipStream(memStream, CompressionMode.Compress);
            gzip.Write(pixels, 0, pixels.Length);
            gzip.Close();
            pixels = memStream.ToArray();
        }

        public void decompressPixels()
        {
            if (pixels == null)
            {
                Console.WriteLine("No Pixel Data to Decompress");
                return;
            }
            if (is_compressed == false) return;
            GZipStream gzip = new GZipStream(new MemoryStream(pixels), CompressionMode.Decompress);
            byte[] buffer = new byte[width * height * 4];
            gzip.Read(buffer, 0, buffer.Length);
            gzip.Close();
            pixels = buffer;
        }

    }
}
