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

        /// <summary>
        /// Draw a wall column using a texture for pixel colors.
        /// </summary>
        /// <param name="dst">Target PixelBuffer.</param>
        /// <param name="x">X coordinate to render the wall on.</param>
        /// <param name="z">View height (but does nothing at the moment) </param>
        /// <param name="distance">Distance of the wall from the camera.</param>
        /// <param name="textureOffset">Offset of the source texture to render from</param>
        /// <param name="src">Source texture to render from</param>
        public static void wallColumn(PixelBuffer dst, int x, float z, float distance, float textureOffset, PixelBuffer src)
        {
            // Calculate initial source texture position.
            int fp_Src = (int)((float)src.height * textureOffset);
            fp_Src = fp_Src * src.width;
            fp_Src = fp_Src << 8;

            // Calculate the height of the column render.
            int colHeight = dst.height;
            if (distance != 0) colHeight = (int)(dst.height / distance);

            // Calculate view offset
            int viewOfs = (int)(colHeight * (z - 1));

            // Calculate base step ratio for source texture read.
            int fpSrcStep = (src.width << 8) / colHeight;

            // Handle columns too tall for the screen.
            if (colHeight > dst.height || distance == 0)
            {
                // Adjust the source texture data offset.
                fp_Src += fpSrcStep * ((colHeight - dst.height) >> 1);
                // Clip the column height to screen height.
                colHeight = dst.height;
            }
            // Calculate the destination data offset for the write.
            int dstIdx = (x * BPP) + ((((dst.height + viewOfs) - colHeight) >> 1) * dst.stride);
            int srcIdx;

            // Draw the column.
            for (int i = 0; i < colHeight; i++)
            {
                srcIdx = (fp_Src >> 8) << 2;
                dst.pixels[dstIdx] = (byte)(src.pixels[srcIdx]);
                dst.pixels[dstIdx + 1] = (byte)(src.pixels[srcIdx + 1]);
                dst.pixels[dstIdx + 2] = (byte)(src.pixels[srcIdx + 2]);
                dst.pixels[dstIdx + 3] = (byte)(src.pixels[srcIdx + 3]);
                fp_Src += fpSrcStep;
                dstIdx += dst.stride;
            }
        }

        /// <summary>
        /// Draw a wall column using a texture for pixel colors and use fog calculations.
        /// </summary>
        /// <param name="dst">Target PixelBuffer</param>
        /// <param name="x">X coordinate to render the wall on.</param>
        /// <param name="z">View height (but does nothing at the moment) </param>
        /// <param name="distance">Distance of the wall from the camera</param>
        /// <param name="textureOffset">Offset of the source texture to render from</param>
        /// <param name="src">Source texture to render from</param>
        public static void wallColumn(PixelBuffer dst, int x, float z, float distance, float textureOffset, PixelBuffer src, Rgba fogColor)
        {
            // Calculate initial source texture position.
            int fp_Src = (int)(src.height * textureOffset);
            fp_Src = fp_Src * src.width;
            fp_Src = fp_Src << 8;

            // Calculate the height of the column render.
            int colHeight = dst.height;
            if (distance != 0) colHeight = (int)(dst.height / distance);

            // Calculate view offset
            int viewOfs = (int)(colHeight * (z - 1));

            // Calculate base step ratio for source texture read.
            int fp_srcStep = (src.width << 8) / colHeight;

            // Handle columns too tall for the screen.
            if (colHeight > dst.height || distance == 0)
            {
                // Adjust the source texture data offset.
                fp_Src += fp_srcStep * ((colHeight - dst.height) >> 1);
                // Clip the column height to screen height.
                colHeight = dst.height;
            }
            // Calculate the destination data offset for the write.
            int dstIdx = (x * BPP) + ((((dst.height + viewOfs) - colHeight) >> 1) * dst.stride);

            // Calcualte shading values
            // Fixed point rgba maths
            int fp_r;
            int fp_g;
            int fp_b;
            int fp_fogCol = (int)(distance * 16f);
            fp_fogCol += fogColor.a;
            if (fp_fogCol > 0x0100) fp_fogCol = 0x0100;

            //Console.WriteLine(fp_fogCol.ToString("X8"));
            int fog_r = fp_fogCol * (fogColor.r);
            int fog_g = fp_fogCol * (fogColor.g);
            int fog_b = fp_fogCol * (fogColor.b);

            int fp_srcCol = 0x0100 - fp_fogCol;

            int srcIdx;

            if (fp_srcStep >= 0x10000)
            {
                // Draw the column.
                for (int i = 0; i < colHeight; i++)
                {

                    srcIdx = (fp_Src >> 8) << 2;
                    if (dstIdx < 0 || dstIdx >= dst.height)
                        break;

                    fp_r = (src.pixels[srcIdx] * fp_srcCol) + fog_r;
                    fp_g = (src.pixels[srcIdx + 1] * fp_srcCol) + fog_g;
                    fp_b = (src.pixels[srcIdx + 2] * fp_srcCol) + fog_b;

                    dst.pixels[dstIdx] = (byte)(fp_r >> 8);
                    dst.pixels[dstIdx + 1] = (byte)(fp_g >> 8);
                    dst.pixels[dstIdx + 2] = (byte)(fp_b >> 8);
                    dst.pixels[dstIdx + 3] = 255;
                    fp_Src += fp_srcStep;
                    dstIdx += dst.stride;
                }
            }
            else
            {
                // Compressed Column Drawing
                int fp_nextSrc;
                byte r, g, b;
                // Draw the column.
                for (int i = 0; i < colHeight;)
                {
                    fp_nextSrc = (fp_Src + 0x100) & 0xFFFFF00;
                    srcIdx = (fp_Src >> 8) << 2;
                    fp_r = ((src.pixels[srcIdx] * fp_srcCol) + fog_r) >> 8;
                    fp_g = ((src.pixels[srcIdx + 1] * fp_srcCol) + fog_g) >> 8;
                    fp_b = ((src.pixels[srcIdx + 2] * fp_srcCol) + fog_b) >> 8;
                    r = (byte)fp_r;
                    g = (byte)fp_g;
                    b = (byte)fp_b;
                    while (fp_Src < fp_nextSrc && i < colHeight)
                    {
                        // HACK: Nearsided renderer should really not have to do this.
                        if (dstIdx >= 0 && dstIdx < dst.pixels.Length)
                        {

                            dst.pixels[dstIdx] = r;
                            dst.pixels[dstIdx + 1] = g;
                            dst.pixels[dstIdx + 2] = b;
                            dst.pixels[dstIdx + 3] = 255;
                        }
                        fp_Src += fp_srcStep;
                        dstIdx += dst.stride;
                        i++;
                    }
                }
            }
        }

        #endregion

        #region Ceilings / Floors
        /// <summary>
        /// Draw a solid row of pixel colors across the screen at Y
        /// </summary>
        /// <param name="dst">Target PixelBuffer</param>
        /// <param name="y">Y coordinate to render the row at</param>
        /// <param name="color">Color of the pixels for this row</param>
        public static void floor(PixelBuffer dst, int y, Rgba color)
        {
            int dstIdx = y * dst.stride;
            for (int i = 0; i < dst.width; i++)
            {
                dst.pixels[dstIdx] = color.r;
                dst.pixels[dstIdx + 1] = color.g;
                dst.pixels[dstIdx + 2] = color.b;
                dst.pixels[dstIdx + 3] = color.a;
                dstIdx += BPP;
            }
        }
        /// <summary>
        /// Draw a row of pixels of a given color and shade them by a fog color
        /// </summary>
        /// <param name="dst">Target PixelBuffer</param>
        /// <param name="y">Y coordinate to render the row at</param>
        /// <param name="distance">Distance of this row from the camera, used in fog calculations.</param>
        /// <param name="color">Color of the pixels for this row</param>
        /// <param name="fogColor">Color of the fog</param>
        public static void floor(PixelBuffer dst,  int y, float distance, Rgba color, Rgba fogColor)
        {
            int dstIdx = y * dst.stride;

            // Calcualte shading values
            // Fixed point rgba maths
            int fp_fogCol = (int)(distance * 16f);
            fp_fogCol += fogColor.a;
            if (fp_fogCol > 0x0100) fp_fogCol = 0x0100;

            //Console.WriteLine(fp_fogCol.ToString("X8"));
            int fog_r = fp_fogCol * (fogColor.r);
            int fog_g = fp_fogCol * (fogColor.g);
            int fog_b = fp_fogCol * (fogColor.b);

            int fp_srcCol = 0x0100 - fp_fogCol;

            byte r = (byte)(((color.r * fp_srcCol) + fog_r) >> 8);
            byte g = (byte)(((color.g * fp_srcCol) + fog_g) >> 8);
            byte b = (byte)(((color.b * fp_srcCol) + fog_b) >> 8);

            for (int i = 0; i < dst.width; i++)
            {
               dst.pixels[dstIdx] = r;
               dst.pixels[dstIdx + 1] = g;
               dst.pixels[dstIdx + 2] = b;
               dst.pixels[dstIdx + 3] = color.a;
                dstIdx += BPP;
            }
        }
        /// <summary>
        /// Draw a row of pixels using a perspective transformed texture map.
        /// </summary>
        /// <param name="dst">Target PixelBuffer</param>
        /// <param name="y">Y coordinate to render this row at</param>
        /// <param name="z">Height of the viewer from the floor</param>
        /// <param name="location_x">X coordinate of viewer, in map units.</param>
        /// <param name="location_y">Y coordinate of viewer, in map units.</param>
        /// <param name="leftAngles">The Ray that describes the far left pixel column angle.</param>
        /// <param name="rightAngles">The Ray that describes the far right pixel column angle.</param>
        /// <param name="src">Source texture to render from.</param>
        public static void floor(PixelBuffer dst, int y, float z, float location_x, float location_y, Ray leftAngles, Ray rightAngles, PixelBuffer src)
        {

            int row = (dst.height / 2) - y;
            z = 0.5f * dst.height;
            float rowDist = z / row;

            float floorStepX = rowDist * (rightAngles.ax - leftAngles.ax) / dst.width;
            float floorStepY = rowDist * (rightAngles.ay - leftAngles.ay) / dst.width;

            float floorX = location_x + rowDist * leftAngles.ax;
            float floorY = location_y + rowDist * leftAngles.ay;

            int u, v;
            int srcIdx;
            int dstIdx = y * dst.stride;

            for (int x = 0; x < dst.width; x++)
            {
                u = (int)(src.width * (floorX)) & (src.width - 1);
                v = (int)(src.height * (floorY)) & (src.height - 1);
                floorX += floorStepX;
                floorY += floorStepY;
                srcIdx = (u << 2) + (v * src.stride);
                dst.pixels[dstIdx] = src.pixels[srcIdx];
                dst.pixels[dstIdx + 1] = src.pixels[srcIdx + 1];
                dst.pixels[dstIdx + 2] = src.pixels[srcIdx + 2];
                dst.pixels[dstIdx + 3] = 255;
                dstIdx += 4;
            }

        }

        #endregion

        #region Sprites
        /// <summary>
        /// Render a sprite on screen, scaled by a given distance.
        /// </summary>
        /// <param name="dst">Target PixelBuffer.</param>
        /// <param name="x">X coordinate of the sprite, centered.</param>
        /// <param name="distance">Distance from the viewer to render the sprite.</param>
        /// <param name="y_buffer">A Ray buffer, used for sprite clipping.</param>
        /// <param name="sprite">The source texture to use for the sprite.</param>
        public static void sprite(PixelBuffer dst, int x, float distance, Ray[] y_buffer, PixelBuffer sprite)
        {
            Render.sprite(dst, x, 1, distance, y_buffer, sprite);
        }
        /// <summary>
        /// Render a sprite on screen, scaled by a given distance.
        /// </summary>
        /// <param name="dst">Target PixelBuffer.</param>
        /// <param name="x">X coordinate of the sprite, centered.</param>
        /// <param name="scale">Scale the sprite by this amount after distance scaling.</param>
        /// <param name="distance">Distance from the viewer to render the sprite.</param>
        /// <param name="y_buffer">A Ray buffer, used for sprite clipping.</param>
        /// <param name="sprite">The source texture to use for the sprite.</param>
        public static void sprite(PixelBuffer dst, int x, float scale, float distance, Ray[] y_buffer, PixelBuffer src)
        {
            int w = (int)((src.width * scale) / distance);
            int h = (int)((src.height * scale) / distance);
            int hw = (int)((w >> 1));
            int hh = (int)((h >> 1));
            if (w <= 0 || h <= 0)
                return;
            clipRect.x = x - hw;
            clipRect.y = (dst.height >> 1) - hh;
            clipRect.w = w;
            clipRect.h = h;

            clipRect.left = Math.Max(clipRect.x, 0);
            clipRect.right = Math.Min(clipRect.x + clipRect.w, dst.width);
            clipRect.top = Math.Max(clipRect.y, 0);
            clipRect.bot = Math.Min(clipRect.y + clipRect.h, dst.height);

            // find the first visible column
            for (; clipRect.left < clipRect.right; clipRect.left++)
                if (y_buffer[clipRect.left].dis >= distance)
                    break;

            // Draw the sprite

            // Control vars
            int fp_src_yStep = (src.height << 8) / clipRect.h;
            int fp_src_xStep = (src.width << 8) / clipRect.w;

            // Textures are rendered at 90 degrees due to column rendering
            int fp_src_ix = ((clipRect.top - clipRect.y) * fp_src_xStep);
            int fp_src_iy = ((clipRect.left - clipRect.x) * fp_src_yStep);

            //int fp_src_i = ((cRect.top - cRect.y) * fp_src_yStep) + ((cRect.left - cRect.x) * fp_src_xStep);
            int fp_src;
            int srcIdx;
            // Destination pixel index.
            int dst_i = (clipRect.left << 2) + (clipRect.top * dst.stride);
            int dstIdx;

            // OPTI: Implement Nearsided and Farsighted Sprite Renderer

            // Step through the rendering columns.
            for (; clipRect.left < clipRect.right; clipRect.left++)
            {
                dstIdx = dst_i;
                fp_src = ((fp_src_iy & 0x7FFFFF00) * src.width) + fp_src_ix;
                // If the sprite is clipped here, we're done.
                if (y_buffer[clipRect.left].dis < distance)
                    break;
                for (int t = clipRect.top; t < clipRect.bot; t++)
                {
                    srcIdx = (fp_src >> 8) << 2;
                    if (srcIdx >= src.pixels.Length)
                        break;
                    dst.pixels[dstIdx] = src.pixels[srcIdx];
                    dst.pixels[dstIdx + 1] = src.pixels[srcIdx + 1];
                    dst.pixels[dstIdx + 2] = src.pixels[srcIdx + 2];
                    dst.pixels[dstIdx + 3] = 255;
                    dstIdx += dst.stride;
                    fp_src += fp_src_xStep;
                }
                fp_src_iy += fp_src_yStep;
                dst_i += 4;
            }
        }

        #endregion
    }
}