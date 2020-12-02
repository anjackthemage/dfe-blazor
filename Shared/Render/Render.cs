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
        public static readonly Rgba C_BLACK = new Rgba(0x00, 0x00, 0x00);
        public static readonly Rgba C_GREY = new Rgba(0x80, 0x80, 0x80);
        public static readonly Rgba C_WHITE = new Rgba(0xFF, 0xFF, 0xFF);
        public static readonly Rgba C_RED = new Rgba(0xFF, 0x00, 0x00);
        public static readonly Rgba C_GREEN = new Rgba(0x00, 0xFF, 0x00);
        public static readonly Rgba C_BLUE = new Rgba(0x00, 0x00, 0xFF);

        #endregion

        #region Global Controls

        public static Rgba clearColor = C_BLACK;
        public static Rgba drawColor = C_WHITE;
        public static Rgba clippedColor = C_RED;
        private static ClipRect clipRect = new ClipRect();

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
        /// Draw a rectangle.
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
        /// <summary>
        /// Draws a bilboard rectangle with depth. Rendering is clipped against a Y buffer.
        /// </summary>
        /// <param name="dst">Target PixelBuffer.</param>
        /// <param name="centerX">X Coordinate of the rectangle's center</param>
        /// <param name="distance">Distance from the camera to render.</param>
        /// <param name="width">Width of the rectangle at distance = 1</param>
        /// <param name="height">Height of the rectangle at distance = 1</param>
        /// <param name="y_buffer">A ray buffer to do distance clipping against.</param>
        public static void rectDepth(PixelBuffer dst, int centerX, float distance, int width, int height, Ray[] y_buffer)
        {
            width = (int)(width / distance);
            height = (int)(height / distance);
            int hw = (int)((width >> 1));
            int hh = (int)((height >> 1));

            clipRect.x = centerX - hw;
            clipRect.y = (dst.height >> 1) - hh;
            clipRect.w = width;
            clipRect.h = height;
            rect(dst, clipRect.x, clipRect.y, clipRect.w, clipRect.h, clippedColor);

            clipRect.left = Math.Max(clipRect.x, 0);
            clipRect.right = Math.Min(clipRect.x + clipRect.w, dst.width - 1);
            clipRect.top = clipRect.y;
            clipRect.bot = clipRect.y + clipRect.h;
            // clip left
            for (int i = clipRect.left; i < clipRect.right; i++)
            {
                if (y_buffer[i].dis <= distance)
                    clipRect.left = i;
                else break;
            }
            // clip right
            for (int i = clipRect.left + 1; i < clipRect.right; i++)
            {
                if (y_buffer[i].dis <= distance)
                {
                    clipRect.right = i;
                    break;
                }
            }
            rect(dst, clipRect.left, clipRect.top, clipRect.right - clipRect.left, clipRect.bot - clipRect.top, drawColor);
        }
        /// <summary>
        /// Draws a bilboard rectangle with depth. Rendering is clipped against a Y buffer.
        /// </summary>
        /// <param name="dst">Target PixelBuffer.</param>
        /// <param name="centerX">X Coordinate of the rectangle's center</param>
        /// <param name="distance">Distance from the camera to render.</param>
        /// <param name="width">Width of the rectangle at distance = 1</param>
        /// <param name="height">Height of the rectangle at distance = 1</param>
        /// <param name="y_buffer">A ray buffer to do distance clipping against.</param>
        /// <param name="color">The color to render the rectangle.</param>
        public static void rectDepth(PixelBuffer dst, int centerX, float distance, int width, int height, Ray[] y_buffer, Rgba color)
        {
            width = (int)(width / distance);
            height = (int)(height / distance);
            int hw = (int)((width >> 1));
            int hh = (int)((height >> 1));

            clipRect.x = centerX - hw;
            clipRect.y = (dst.height >> 1) - hh;
            clipRect.w = width;
            clipRect.h = height;
            rect(dst, clipRect.x, clipRect.y, clipRect.w, clipRect.h, clippedColor);

            clipRect.left = Math.Max(clipRect.x, 0);
            clipRect.right = Math.Min(clipRect.x + clipRect.w, dst.width - 1);
            clipRect.top = clipRect.y;
            clipRect.bot = clipRect.y + clipRect.h;
            // clip left
            for (int i = clipRect.left; i < clipRect.right; i++)
            {
                if (y_buffer[i].dis <= distance)
                    clipRect.left = i;
                else break;
            }
            // clip right
            for (int i = clipRect.left + 1; i < clipRect.right; i++)
            {
                if (y_buffer[i].dis <= distance)
                {
                    clipRect.right = i;
                    break;
                }
            }
            rect(dst, clipRect.left, clipRect.top, clipRect.right - clipRect.left, clipRect.bot - clipRect.top, color);
        }

        #endregion

        #region Walls
        /// <summary>
        /// Draws a flat shaded wall column based on distance.
        /// </summary>
        /// <param name="dst">Target PixelBuffer.</param>
        /// <param name="x">X coordinate to render the wall on.</param>
        /// <param name="distance">Distance of the wall from the camera.</param>
        /// <param name="color">Color to render the wall.</param>
        public static void wallColumn(PixelBuffer dst, int x, float distance)
        {
            int colHeight = dst.height;
            if (distance != 0)
                colHeight = (int)(dst.height / distance);

            int fp8_blend = (colHeight * drawColor.a) / dst.height;
            byte r = (byte)Math.Min(0xFF, ((drawColor.r * fp8_blend) >> 8));
            byte g = (byte)Math.Min(0xFF, ((drawColor.g * fp8_blend) >> 8));
            byte b = (byte)Math.Min(0xFF, ((drawColor.b * fp8_blend) >> 8));

            if (colHeight > dst.height || distance == 0)
                colHeight = dst.height;

            // Find the top of the column
            int y = (x * BPP) + (((dst.height - colHeight) >> 1) * dst.stride);
            for (int i = 0; i < colHeight; i++)
            {
                dst.pixels[y] = r;
                dst.pixels[y + 1] = g;
                dst.pixels[y + 2] = b;
                dst.pixels[y + 3] = 255;
                y += dst.stride;
            }
        }
        /// <summary>
        /// Draws a flat shaded wall column based on distance.
        /// </summary>
        /// <param name="dst">Target PixelBuffer.</param>
        /// <param name="x">X coordinate to render the wall on.</param>
        /// <param name="distance">Distance of the wall from the camera.</param>
        /// <param name="color">Color to render the wall. Alpha channel is used for distance intensity.</param>
        public static void wallColumn(PixelBuffer dst, int x, float distance, Rgba color)
        {
            int colHeight = dst.height;
            if (distance != 0)
                colHeight = (int)(dst.height / distance);

            int fp8_blend = (colHeight * color.a) / dst.height;
            byte r = (byte)Math.Min(0xFF, ((color.r * fp8_blend) >> 8));
            byte g = (byte)Math.Min(0xFF, ((color.g * fp8_blend) >> 8));
            byte b = (byte)Math.Min(0xFF, ((color.b * fp8_blend) >> 8));

            if (colHeight > dst.height || distance == 0)
                colHeight = dst.height;

            // Find the top of the column
            int y = (x * BPP) + (((dst.height - colHeight) >> 1) * dst.stride);
            for (int i = 0; i < colHeight; i++)
            {
                dst.pixels[y] = r;
                dst.pixels[y + 1] = g;
                dst.pixels[y + 2] = b;
                dst.pixels[y + 3] = 255;
                y += dst.stride;
            }
        }

        #endregion

        #region Ceilings / Floors
        #endregion
    }
}