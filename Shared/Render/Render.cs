using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dfe.Shared.Render
{
    /// <summary>
    /// A collection of static draw methods.
    /// </summary>
    public class Render
    {
        #region Constants
        
        // Bytes per pixel
        public const int BPP = 4;
        public static readonly Rgba C_BLACK = new Rgba(0, 0, 0);
        public static readonly Rgba C_WHITE = new Rgba(255, 255, 255);
        
        #endregion

        #region Global Controls

        public static Rgba clearColor = C_BLACK;
        public static Rgba drawColor = C_WHITE;
        
        #endregion

        #region Clear

        /// <summary>
        /// Clear a pixelbuffer to the current Render.drawColor.
        /// </summary>
        /// <param name="dst">Target PixelBuffer</param>
        public static void clear(PixelBuffer dst)
        {
            if (clearColor.a < 255)
            {
                // Draw pixel with alpha blending.
                int fp8_dstBlend = 0xFF - clearColor.a;
                int ar = clearColor.r * clearColor.a;
                int ag = clearColor.g * clearColor.a;
                int ab = clearColor.b * clearColor.a;
                for (int i = 0; i < dst.pixels.Length; i += BPP)
                {
                    dst.pixels[i + 0] = (byte)(((dst.pixels[i + 0] * fp8_dstBlend) + ar) >> 8);
                    dst.pixels[i + 1] = (byte)(((dst.pixels[i + 1] * fp8_dstBlend) + ag) >> 8);
                    dst.pixels[i + 2] = (byte)(((dst.pixels[i + 2] * fp8_dstBlend) + ab) >> 8);
                    dst.pixels[i + 3] = 255;
                }
            } else {
                for (int i = 0; i < dst.pixels.Length; i += BPP)
                {
                    dst.pixels[i] = clearColor.r;
                    dst.pixels[i + 1] = clearColor.g;
                    dst.pixels[i + 2] = clearColor.b;
                    dst.pixels[i + 3] = 255;
                }
            }
        }
        /// <summary>
        /// Clear a pixelbuffer to the specified color.
        /// </summary>
        /// <param name="dst">Target PixelBuffer</param>
        /// <param name="color">Color to clear to</param>
        public static void clear(PixelBuffer dst, Rgba color)
        {
            if (color.a < 255)
            {
                // Draw pixel with alpha blending.
                int fp8_dstBlend = 0xFF - color.a;
                int ar = color.r * color.a;
                int ag = color.g * color.a;
                int ab = color.b * color.a;
                for (int i = 0; i < dst.pixels.Length; i += BPP)
                {
                    dst.pixels[i + 0] = (byte)(((dst.pixels[i + 0] * fp8_dstBlend) + ar) >> 8);
                    dst.pixels[i + 1] = (byte)(((dst.pixels[i + 1] * fp8_dstBlend) + ag) >> 8);
                    dst.pixels[i + 2] = (byte)(((dst.pixels[i + 2] * fp8_dstBlend) + ab) >> 8);
                    dst.pixels[i + 3] = 255;
                }
            }
            else
            {
                for (int i = 0; i < dst.pixels.Length; i += BPP)
                {
                    dst.pixels[i] = color.r;
                    dst.pixels[i + 1] = color.g;
                    dst.pixels[i + 2] = color.b;
                    dst.pixels[i + 3] = 255;
                }
            }
        }

        #endregion

        #region Point

        /// <summary>
        /// Draws a point using the current Render.drawColor.
        /// </summary>
        /// <param name="dst">Target PixelBuffer</param>
        /// <param name="x">X Coordinate</param>
        /// <param name="y">Y Coordinate</param>
        public static void point(PixelBuffer dst, int x, int y)
        {
            if (x < 0 || x >= dst.width || y < 0 || y >= dst.height) return;
            int i = (x << 2) + (y * dst.stride) % dst.pixels.Length;
            if (drawColor.a < 0xFF)
            {
                // Draw pixel with alpha blending.
                int fp8_dstBlend = 0xFF - drawColor.a;
                int ar = drawColor.r * drawColor.a;
                int ag = drawColor.g * drawColor.a;
                int ab = drawColor.b * drawColor.a;

                dst.pixels[i + 0] = (byte)(((dst.pixels[i + 0] * fp8_dstBlend) + ar) >> 8);
                dst.pixels[i + 1] = (byte)(((dst.pixels[i + 1] * fp8_dstBlend) + ag) >> 8);
                dst.pixels[i + 2] = (byte)(((dst.pixels[i + 2] * fp8_dstBlend) + ab) >> 8);
                dst.pixels[i + 3] = 255;
            }
            else
            {
                // Draw pixel without alpha blending.
                dst.pixels[i] = drawColor.r;
                dst.pixels[i + 1] = drawColor.g;
                dst.pixels[i + 2] = drawColor.b;
                dst.pixels[i + 3] = 255;
            }
        }
        /// <summary>
        /// Sets the pixel color on a pixel buffer of the desired color.
        /// Ignores alpha blending.
        /// </summary>
        /// <param name="dst">Target PixelBuffer</param>
        /// <param name="x">X Coordinate</param>
        /// <param name="y">Y Coordinate</param>
        /// <param name="r">Red color component</param>
        /// <param name="g">Green color component</param>
        /// <param name="b">Blue color component</param>
        public static void point(PixelBuffer dst, int x, int y, byte r, byte g, byte b)
        {
            if (x < 0 || x >= dst.width || y < 0 || y >= dst.height) return;
            int i = (x << 2) + (y * dst.stride) % dst.pixels.Length;
            dst.pixels[i] = r;
            dst.pixels[i + 1] = g;
            dst.pixels[i + 2] = b;
            dst.pixels[i + 3] = 255;
        }
        /// <summary>
        /// Sets the pixel color on a pixel buffer of the desired color.
        /// </summary>
        /// <param name="dst">Target PixelBuffer</param>
        /// <param name="x">X Coordinate</param>
        /// <param name="y">Y Coordinate</param>
        /// <param name="color">Color to set the pixel to</param>
        public static void point(PixelBuffer dst, int x, int y, Rgba color)
        {
            if (x < 0 || x >= dst.width || y < 0 || y >= dst.height) return;
            int i = (x << 2) + (y * dst.stride) % dst.pixels.Length;
            if (color.a < 0xFF)
            {
                // Draw pixel with alpha blending.
                int fp8_dstBlend = 0xFF - color.a;
                dst.pixels[i + 0] = (byte)((dst.pixels[i + 0] * fp8_dstBlend) + (color.r * color.a) >> 8);
                dst.pixels[i + 1] = (byte)((dst.pixels[i + 1] * fp8_dstBlend) + (color.g * color.a) >> 8);
                dst.pixels[i + 2] = (byte)((dst.pixels[i + 2] * fp8_dstBlend) + (color.b * color.a) >> 8);
                dst.pixels[i + 3] = 255;
            } else {
                // Draw pixel without alpha blending.
                dst.pixels[i] = color.r;
                dst.pixels[i + 1] = color.g;
                dst.pixels[i + 2] = color.b;
                dst.pixels[i + 3] = 255;
            }
        }

        #endregion

        #region Rectangle

        /// <summary>
        /// Draw a Rectangle.
        /// </summary>
        /// <param name="dst">Target PixelBuffer</param>
        /// <param name="x">Left edge coordinate of the rectangle</param>
        /// <param name="y">Top edge coordinate of teh rectangle</param>
        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        public static void rect(PixelBuffer dst, int x, int y, int width, int height)
        {
            // Find clipping rectangle.
            int left = Math.Max(x, 0);
            int right = Math.Min(x + width, dst.width - 1);
            int top = Math.Max(y, 0);
            int bot = Math.Min(y + height, dst.height - 1);
            int clipH = bot - top;
            int clipW = right - left;

            if (clipH <= 0 || clipW <= 0)
                return;

            int dy = top * dst.stride;
            int dstA = dy + (left << 2);
            int dstB = dy + (right << 2);
            // Draw pixel with alpha blending.
            int fp8_dstBlend = 0xFF - drawColor.a;
            int ar = drawColor.r * drawColor.a;
            int ag = drawColor.g * drawColor.a;
            int ab = drawColor.b * drawColor.a;

            // Draw cols.
            for (int i = 0; i < clipH; i++)
            {
                if (left == x)
                {
                    dst.pixels[dstA + 0] = (byte)(((dst.pixels[dstA + 0] * fp8_dstBlend) + ar) >> 8);
                    dst.pixels[dstA + 1] = (byte)(((dst.pixels[dstA + 1] * fp8_dstBlend) + ag) >> 8);
                    dst.pixels[dstA + 2] = (byte)(((dst.pixels[dstA + 2] * fp8_dstBlend) + ab) >> 8);
                    dst.pixels[dstA + 3] = 255;
                }
                if (right == x + width)
                {
                    dst.pixels[dstB + 0] = (byte)(((dst.pixels[dstB + 0] * fp8_dstBlend) + ar) >> 8);
                    dst.pixels[dstB + 1] = (byte)(((dst.pixels[dstB + 1] * fp8_dstBlend) + ag) >> 8);
                    dst.pixels[dstB + 2] = (byte)(((dst.pixels[dstB + 2] * fp8_dstBlend) + ab) >> 8);
                    dst.pixels[dstB + 3] = 255;
                }
                dstA += dst.stride;
                dstB += dst.stride;
            }

            dstB = dy + (left << 2);
            // Draw rows
            for (int i = 0; i < clipW; i++)
            {
                if (bot == (y + height))
                {
                    dst.pixels[dstA + 0] = (byte)(((dst.pixels[dstA + 0] * fp8_dstBlend) + ar) >> 8);
                    dst.pixels[dstA + 1] = (byte)(((dst.pixels[dstA + 1] * fp8_dstBlend) + ag) >> 8);
                    dst.pixels[dstA + 2] = (byte)(((dst.pixels[dstA + 2] * fp8_dstBlend) + ab) >> 8);
                    dst.pixels[dstA + 3] = 255;

                }
                if (top == y)
                {
                    dst.pixels[dstB + 0] = (byte)(((dst.pixels[dstB + 0] * fp8_dstBlend) + ar) >> 8);
                    dst.pixels[dstB + 1] = (byte)(((dst.pixels[dstB + 1] * fp8_dstBlend) + ag) >> 8);
                    dst.pixels[dstB + 2] = (byte)(((dst.pixels[dstB + 2] * fp8_dstBlend) + ab) >> 8);
                    dst.pixels[dstB + 3] = 255;
                }
                dstA += BPP;
                dstB += BPP;
            }
        }
        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="x">Left edge coordinate of the rectangle</param>
        /// <param name="y">Top edge coordinate of teh rectangle</param>
        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        /// <param name="color">The desired RGBA color</param>
        public static void rect(PixelBuffer dst, int x, int y, int width, int height, Rgba color)
        {
            // Find clipping rectangle.
            int left = Math.Max(x, 0);
            int right = Math.Min(x + width, dst.width - 1);
            int top = Math.Max(y, 0);
            int bot = Math.Min(y + height, dst.height - 1);
            int clipH = bot - top;
            int clipW = right - left;

            if (clipH <= 0 || clipW <= 0)
                return;

            int dy = top * dst.stride;
            int dstA = dy + (left << 2);
            int dstB = dy + (right << 2);
            // Draw pixel with alpha blending.
            int fp8_dstBlend = 0xFF - color.a;
            int ar = color.r * color.a;
            int ag = color.g * color.a;
            int ab = color.b * color.a;

            // Draw cols.
            for (int i = 0; i < clipH; i++)
            {
                if (left == x)
                {
                    dst.pixels[dstA + 0] = (byte)(((dst.pixels[dstA + 0] * fp8_dstBlend) + ar) >> 8);
                    dst.pixels[dstA + 1] = (byte)(((dst.pixels[dstA + 1] * fp8_dstBlend) + ag) >> 8);
                    dst.pixels[dstA + 2] = (byte)(((dst.pixels[dstA + 2] * fp8_dstBlend) + ab) >> 8);
                    dst.pixels[dstA + 3] = 255;
                }
                if (right == x + width)
                {
                    dst.pixels[dstB + 0] = (byte)(((dst.pixels[dstB + 0] * fp8_dstBlend) + ar) >> 8);
                    dst.pixels[dstB + 1] = (byte)(((dst.pixels[dstB + 1] * fp8_dstBlend) + ag) >> 8);
                    dst.pixels[dstB + 2] = (byte)(((dst.pixels[dstB + 2] * fp8_dstBlend) + ab) >> 8);
                    dst.pixels[dstB + 3] = 255;
                }
                dstA += dst.stride;
                dstB += dst.stride;
            }

            dstB = dy + (left << 2);
            // Draw rows
            for (int i = 0; i < clipW; i++)
            {
                if (bot == (y + height))
                {
                    dst.pixels[dstA + 0] = (byte)(((dst.pixels[dstA + 0] * fp8_dstBlend) + ar) >> 8);
                    dst.pixels[dstA + 1] = (byte)(((dst.pixels[dstA + 1] * fp8_dstBlend) + ag) >> 8);
                    dst.pixels[dstA + 2] = (byte)(((dst.pixels[dstA + 2] * fp8_dstBlend) + ab) >> 8);
                    dst.pixels[dstA + 3] = 255;

                }
                if (top == y)
                {
                    dst.pixels[dstB + 0] = (byte)(((dst.pixels[dstB + 0] * fp8_dstBlend) + ar) >> 8);
                    dst.pixels[dstB + 1] = (byte)(((dst.pixels[dstB + 1] * fp8_dstBlend) + ag) >> 8);
                    dst.pixels[dstB + 2] = (byte)(((dst.pixels[dstB + 2] * fp8_dstBlend) + ab) >> 8);
                    dst.pixels[dstB + 3] = 255;
                }
                dstA += BPP;
                dstB += BPP;
            }
        }

        #endregion


    }
}