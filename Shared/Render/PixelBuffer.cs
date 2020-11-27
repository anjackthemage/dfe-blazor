using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dfe.Shared.Render
{
    /// <summary>
    /// An RGBA color with byte components.
    /// </summary>
    public struct color4i
    {
        public byte r, g, b, a;
        public color4i(uint color)
        {
            r = (byte)(color & 0xFF); // red
            g = (byte)((color >> 8) & 0xFF);   // green
            b = (byte)((color >> 16) & 0xFF);  // blue
            a = (byte)((color >> 24) & 0xFF); // alpha  
        }

        public color4i(byte red, byte green, byte blue)
        {
            r = red;
            g = green;
            b = blue;
            a = 255;
        }
        public void set(byte red, byte green, byte blue)
        {
            r = red;
            g = green;
            b = blue;
            a = 255;
        }

        public void set(byte red, byte green, byte blue, byte alpha)
        {
            r = red;
            g = green;
            b = blue;
            a = alpha;
        }
        public void set(float red, float green, float blue)
        {
            red = Math.Min(1, Math.Max(0, red));
            green = Math.Min(1, Math.Max(0, green));
            blue = Math.Min(1, Math.Max(0, blue));
            set((byte)red * 255, (byte)blue * 255, (byte)green * 255, 255);
        }
        public void set(float red, float green, float blue, float alpha)
        {
            red = Math.Min(1, Math.Max(0, red));
            green = Math.Min(1, Math.Max(0, green));
            blue = Math.Min(1, Math.Max(0, blue));
            alpha = Math.Min(1, Math.Max(0, alpha));
            set((byte)red * 255, (byte)blue * 255, (byte)green * 255, (byte)alpha * 255);
        }
        public void set(uint color)
        {
            set((byte)color & 0xFF, // red
                (byte)(color >> 8) & 0xFF,   // green
                (byte)(color >> 16) & 0xFF,  // blue
                (byte)(color >> 24) & 0xFF); // alpha
        }
    }
    /// <summary>
    /// Fog color and intensity structure.
    /// </summary>
    public struct fog4i
    {
        public byte r, g, b, i;
        public fog4i(byte red, byte green, byte blue)
        {
            r = red;
            g = green;
            b = blue;
            i = 0x80;
        }

        public fog4i(color4i color, float intensity)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            if (intensity < 0) i = 0;
            else if (intensity >= 1) i = 128;
            else { i = (byte)(intensity * 128f); }
            //Console.WriteLine(intensity);
        }
    }

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

    public class PixelBuffer
    {
        // Bytes per pixel
        public const int bpp = 4;
        // Worker clipping rectangle - Assume this to be volatile.
        private static ClipRect cRect = new ClipRect();
        // Debugging color for bounds of an obscured sprite.
        private static color4i clippedColor = new color4i(255, 0, 0);
        // Debugging color for bounds of a visible sprite.
        private static color4i visibleColor = new color4i(255, 255, 255);
        // Overall width of the buffer, in pixels.
        public readonly int width;
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
            stride = width * bpp;
            pixels = new byte[width * height * bpp];
        }
        /// <summary>
        /// Sets the color of a single pixel in the buffer.
        /// </summary>
        /// <param name="x">The X coordinate of the target pixel.</param>
        /// <param name="y">The Y coordinate of teh target pixel.</param>
        /// <param name="color">The desired RGBA pixel color.</param>
        /// <summary>
        /// Clears the buffer to black.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < width * height * bpp; i += bpp)
            {
                pixels[i] = 0;
                pixels[i + 1] = 0;
                pixels[i + 2] = 0;
                pixels[i + 3] = 255;
            }
        }
        /// <summary>
        /// Clears the buffer to a desired color.
        /// </summary>
        /// <param name="color">The desired RGBA color.</param>
        public void Clear(color4i color)
        {
            for (int i = 0; i < width * height * bpp; i += bpp)
            {
                pixels[i] = color.r;
                pixels[i + 1] = color.g;
                pixels[i + 2] = color.b;
                pixels[i + 3] = color.a;
            }
        }
        public void DrawPoint(int x, int y, color4i color)
        {
            int i = (x << 2) + (y * stride) % pixels.Length;
            pixels[i] = color.r;
            pixels[i + 1] = color.g;
            pixels[i + 2] = color.b;
            pixels[i + 3] = color.a;
        }
        /// <summary>
        /// Sets the color of a single pixel in the buffer.
        /// </summary>
        /// <param name="x">The X coordinate of the target pixel.</param>
        /// <param name="y">The Y coordinate of teh target pixel.</param>
        /// <param name="r">Red intensity (0-255)</param>
        /// <param name="g">Green intensity (0-255)</param>
        /// <param name="b">Blue intensity (0-255)</param>
        public void DrawPoint(int x, int y, byte r, byte g, byte b)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return;

            int i = (x << 2) + (y * stride) % pixels.Length;
            pixels[i] = r;
            pixels[i + 1] = g;
            pixels[i + 2] = b;
            pixels[i + 3] = 255;
        }
        /// <summary>
        /// Draws a rectangle
        /// </summary>
        /// <param name="x">Left edge coordinate of the rectangle.</param>
        /// <param name="y">Top edge coordinate of teh rectangle.</param>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        /// <param name="color">The desired RGBA color.</param>
        public void DrawRect(int x, int y, int width, int height, color4i color)
        {
            // Find clipping rectangle.
            int left = Math.Max(x, 0);
            int right = Math.Min(x + width, this.width - 1);
            int top = Math.Max(y, 0);
            int bot = Math.Min(y + height, this.height - 1);
            int clipH = bot - top;
            int clipW = right - left;

            if (clipH <= 0 || clipW <= 0)
                return;

            int dy = top * stride;
            int dstA = dy + (left << 2);
            int dstB = dy + (right << 2);
            // Draw cols.
            for (int i = 0; i < clipH; i++)
            {
                if (left == x)
                {
                    pixels[dstA + 0] = color.r;
                    pixels[dstA + 1] = color.g;
                    pixels[dstA + 2] = color.b;
                    pixels[dstA + 3] = color.a;
                }
                if (right == x + width)
                {
                    pixels[dstB + 0] = color.r;
                    pixels[dstB + 1] = color.g;
                    pixels[dstB + 2] = color.b;
                    pixels[dstB + 3] = color.a;
                }
                dstA += stride;
                dstB += stride;
            }

            dstB = dy + (left << 2);
            // Draw rows
            for(int i = 0; i < clipW; i++)
            {
                if(bot == (y + height))
                {
                    pixels[dstA + 0] = color.r;
                    pixels[dstA + 1] = color.g;
                    pixels[dstA + 2] = color.b;
                    pixels[dstA + 3] = color.a;

                }
                if (top == y)
                {
                    pixels[dstB + 0] = color.r;
                    pixels[dstB + 1] = color.g;
                    pixels[dstB + 2] = color.b;
                    pixels[dstB + 3] = color.a;
                }
                dstA += bpp;
                dstB += bpp;
            }

        }
        /// <summary>
        /// Draws a bilboard rectangle with depth.
        /// </summary>
        /// <param name="screenX">X Coordinate of the rectangle's center</param>
        /// <param name="distance">Distance from the camera to render.</param>
        /// <param name="width">Width of the rectangle at distance = 1</param>
        /// <param name="height">Height of the rectangle at distance = 1</param>
        /// <param name="ray_buffer">A ray buffer to do distance clipping against.</param>
        /// <param name="color">The color to render the rectangle.</param>
        public void DrawRectPerspective(int screenX, float distance, int width, int height, ray[] ray_buffer, color4i color)
        {
            width = (int)(width / distance);
            height = (int)(height / distance);
            int hw = (int)((width >> 1));
            int hh = (int)((height >> 1));

            cRect.x = screenX - hw;
            cRect.y = (this.height >> 1) - hh;
            cRect.w = width;
            cRect.h = height;
            DrawRect(cRect.x, cRect.y, cRect.w, cRect.h, clippedColor);

            cRect.left = Math.Max(cRect.x, 0);
            cRect.right = Math.Min(cRect.x + cRect.w, this.width - 1);
            cRect.top = cRect.y;
            cRect.bot = cRect.y + cRect.h;
            // clip left
            for (int i = cRect.left; i < cRect.right; i++)
            {
                if (ray_buffer[i].dis <= distance)
                    cRect.left = i;
                else break;
            }
            // clip right
            for (int i = cRect.left + 1; i < cRect.right; i++)
            {
                if (ray_buffer[i].dis <= distance)
                {
                    cRect.right = i;
                    break;
                }
            }
            DrawRect(cRect.left, cRect.top, cRect.right - cRect.left, cRect.bot - cRect.top, color);
        }
        /// <summary>
        /// Renders a column of pixels in perspective at column x
        /// </summary>
        /// <param name="column">The pixel column to render to</param>
        /// <param name="distance">The distance of the 'wall' from the camera.</param>
        /// <param name="color">The desired color of the wall.</param>
        public void DrawWallColumn(int column, float distance, color4i color)
        {
            int colHeight = height;
            if (distance != 0)
                colHeight = (int)(height / distance);
            if (colHeight > height || distance == 0) 
                colHeight = height;
            byte c = (byte)colHeight;
            // Find the top of the column
            int y = (column * bpp) + (((height - colHeight) >> 1) * stride);
            for(int i = 0; i < colHeight; i++)
            {
                pixels[y] = c;
                pixels[y + 1] = c;
                pixels[y + 2] = c;
                pixels[y + 3] = 255;
                y += stride;
            }
        }
        /// <summary>
        /// Draw a wall column using a texture for pixel colors.
        /// </summary>
        /// <param name="screenX">The pixel column to render to</param>
        /// <param name="distance">The distance of the 'wall' from the camera.</param>
        /// <param name="texOfs">The source pixel column to draw from, (value from 0..1)</param>
        /// <param name="srcImage">The source pixel buffer to render pixels from</param>
        public void TexturedWall(int screenX, float viewHeight, float distance, float texOfs, PixelBuffer srcImage)
        {
            // Calculate initial source texture position.
            int fp_Src = (int)((float)srcImage.height * texOfs);
            fp_Src = fp_Src * srcImage.width;
            fp_Src = fp_Src << 8;

            // Calculate the height of the column render.
            int colHeight = height;
            if (distance != 0) colHeight = (int)(height / distance);

            // Calculate view offset
            int viewOfs = (int)(colHeight * (viewHeight - 1));

            // Calculate base step ratio for source texture read.
            int fpSrcStep = (srcImage.width << 8) / colHeight; 

            // Handle columns too tall for the screen.
            if (colHeight > height || distance == 0)
            {
                // Adjust the source texture data offset.
                fp_Src += fpSrcStep * ((colHeight - height) >> 1);
                // Clip the column height to screen height.
                colHeight = height;
            }
            // Calculate the destination data offset for the write.
            int dst = (screenX * bpp) + ((((height + viewOfs) - colHeight) >> 1) * stride);
            int src;

            // Draw the column.
            for (int i = 0; i < colHeight; i++)
            {
                src = (fp_Src >> 8) << 2;
                pixels[dst] = (byte)(srcImage.pixels[src]);
                pixels[dst + 1] = (byte)(srcImage.pixels[src + 1]);
                pixels[dst + 2] = (byte)(srcImage.pixels[src + 2]);
                pixels[dst + 3] = (byte)(srcImage.pixels[src + 3]);
                fp_Src += fpSrcStep;
                dst += stride;
            }
        }
        /// <summary>
        /// Draw a wall column using a texture for pixel colors.
        /// </summary>
        /// <param name="column">The pixel column to render to</param>
        /// <param name="distance">The distance of the 'wall' from the camera.</param>
        /// <param name="texOfs">The source pixel column to draw from, (value from 0..1)</param>
        /// <param name="srcImage">The source pixel buffer to render pixels from</param>
        /// <param name="fogColor">Fog color to blend as distance increases.</param>
        public void ShadeTexturedWall(int screenX, float viewHeight, float distance, float texOfs, PixelBuffer srcImage, fog4i fogColor)
        {
            // Calculate initial source texture position.
            int fp_Src = (int)(srcImage.height * texOfs);
            fp_Src = fp_Src * srcImage.width;
            fp_Src = fp_Src << 8;

            // Calculate the height of the column render.
            int colHeight = height;
            if (distance != 0) colHeight = (int)(height / distance);

            // Calculate view offset
            int viewOfs = (int)(colHeight * (viewHeight - 1));

            // Calculate base step ratio for source texture read.
            int fp_srcStep = (srcImage.width << 8) / colHeight;

            // Handle columns too tall for the screen.
            if (colHeight > height || distance == 0)
            {
                // Adjust the source texture data offset.
                fp_Src += fp_srcStep * ((colHeight - height) >> 1);
                // Clip the column height to screen height.
                colHeight = height;
            }
            // Calculate the destination data offset for the write.
            int dst = (screenX * bpp) + ((((height + viewOfs) - colHeight) >> 1) * stride);

            // Calcualte shading values
            // Fixed point rgba maths
            int fp_r;
            int fp_g;
            int fp_b;
            int fp_fogCol = (int)(distance * 16f);
            fp_fogCol += fogColor.i;
            if (fp_fogCol > 0x0100) fp_fogCol = 0x0100;

            //Console.WriteLine(fp_fogCol.ToString("X8"));
            int fog_r = fp_fogCol * (fogColor.r);
            int fog_g = fp_fogCol * (fogColor.g);
            int fog_b = fp_fogCol * (fogColor.b);

            int fp_srcCol = 0x0100 - fp_fogCol;

            int src;

            if (fp_srcStep >= 0x10000)
            {
                // Draw the column.
                for (int i = 0; i < colHeight; i++)
                {

                    src = (fp_Src >> 8) << 2;
                    if (dst < 0 || dst >= height)
                        break;

                    fp_r = (srcImage.pixels[src] * fp_srcCol) + fog_r;
                    fp_g = (srcImage.pixels[src + 1] * fp_srcCol) + fog_g;
                    fp_b = (srcImage.pixels[src + 2] * fp_srcCol) + fog_b;

                    pixels[dst] = (byte)(fp_r >> 8);
                    pixels[dst + 1] = (byte)(fp_g >> 8);
                    pixels[dst + 2] = (byte)(fp_b >> 8);
                    pixels[dst + 3] = 255;
                    fp_Src += fp_srcStep;
                    dst += stride;
                }
            }
            else {
                // Compressed Column Drawing
                int fp_nextSrc;
                byte r, g, b;
                // Draw the column.
                for (int i = 0; i < colHeight;)
                {
                    fp_nextSrc = (fp_Src + 0x100) & 0xFFFFF00;
                    src = (fp_Src >> 8) << 2;
                    fp_r = ((srcImage.pixels[src] * fp_srcCol) + fog_r) >> 8;
                    fp_g = ((srcImage.pixels[src + 1] * fp_srcCol) + fog_g) >> 8;
                    fp_b = ((srcImage.pixels[src + 2] * fp_srcCol) + fog_b) >> 8;
                    r = (byte)fp_r;
                    g = (byte)fp_g;
                    b = (byte)fp_b;
                    while (fp_Src < fp_nextSrc && i < colHeight)
                    {
                        // HACK: Nearsided renderer should really not have to do this.
                        if (dst >= 0 && dst < pixels.Length)
                        {

                            pixels[dst] = r;
                            pixels[dst + 1] = g;
                            pixels[dst + 2] = b;
                            pixels[dst + 3] = 255;
                        }
                        fp_Src += fp_srcStep;
                        dst += stride;
                        i++;
                    }
                }
            }
        }
        /// <summary>
        /// Render a bilboard sprite at a specific distance with depth clipping.
        /// </summary>
        /// <param name="screenX">X Coordinate of the rectangle's center</param>
        /// <param name="distance">Distance from the camera to render.</param>
        /// <param name="ray_buffer">A ray buffer to do distance clipping against.</param>
        /// <param name="sprite">Pixel buffer to use as a source texture.</param>
        public void DrawSpritePerspective(int screenX, float distance, ray[] ray_buffer, PixelBuffer sprite)
        {
            int w = (int)((sprite.width << 4) / distance);
            int h = (int)((sprite.height << 4) / distance);
            int hw = (int)((w >> 1));
            int hh = (int)((h >> 1));
            if (w <= 0 || h <= 0)
                return;
            cRect.x = screenX - hw;
            cRect.y = (height >> 1) - hh;
            cRect.w = w;
            cRect.h = h;

            cRect.left = Math.Max(cRect.x, 0);
            cRect.right = Math.Min(cRect.x + cRect.w, width);
            cRect.top = Math.Max(cRect.y, 0);
            cRect.bot = Math.Min(cRect.y + cRect.h, height);

            // TODO : Remove debugging rectangle for clipped sprites
            DrawRect(cRect.x, cRect.y, cRect.w, cRect.h, clippedColor);

            // find the first visible column
            for (; cRect.left < cRect.right; cRect.left++)
                if (ray_buffer[cRect.left].dis >= distance)
                    break;

            // Draw the sprite

            // Control vars
            int fp_src_yStep = (sprite.height << 16) / cRect.h;
            int fp_src_xStep = (sprite.width << 16) / cRect.w;

            // Textures are rendered at 90 degrees due to column rendering
            int fp_src_ix = ((cRect.top - cRect.y) * fp_src_xStep);
            int fp_src_iy = ((cRect.left - cRect.x) * fp_src_yStep);

            //int fp_src_i = ((cRect.top - cRect.y) * fp_src_yStep) + ((cRect.left - cRect.x) * fp_src_xStep);
            int fp_src;
            int src;
            // Destination pixel index.
            int dst_i = (cRect.left << 2) + (cRect.top * stride);
            int dst;

            // OPTI: Implement Nearsided and Farsighted Sprite Renderer

            // Step through the rendering columns.
            for (; cRect.left < cRect.right; cRect.left++)
            {
                dst = dst_i;
                fp_src = ((fp_src_iy & 0x7FFF0000) * sprite.width) + fp_src_ix;
                // If the sprite is clipped here, we're done.
                if (ray_buffer[cRect.left].dis < distance)
                    break;
                for (int t = cRect.top; t < cRect.bot; t++)
                {
                    src = (fp_src >> 16) << 2;
                    if (src >= sprite.pixels.Length)
                        break;
                    pixels[dst] = sprite.pixels[src];
                    pixels[dst + 1] = sprite.pixels[src + 1];
                    pixels[dst + 2] = sprite.pixels[src + 2];
                    pixels[dst + 3] = 255;
                    dst += stride;
                    fp_src += fp_src_xStep;
                }
                fp_src_iy += fp_src_yStep;
                dst_i += 4;
            }
            // TODO : Remove debugging rectangle for visible sprites
            DrawRect(cRect.left, cRect.top, cRect.right - cRect.left, cRect.bot - cRect.top, visibleColor);
        }

        public void DrawRow(int screenY, color4i color)
        {
            int dst = screenY * stride;
            for(int i = 0; i < width; i++)
            {
                pixels[dst] = color.r;
                pixels[dst + 1] = color.g;
                pixels[dst + 2] = color.b;
                pixels[dst + 3] = color.a;
                dst += bpp;
            }
        }

        public void ShadeRow(int screenY, float distance, color4i color, fog4i fogColor)
        {
            int dst = screenY * stride;

            // Calcualte shading values
            // Fixed point rgba maths
            int fp_fogCol = (int)(distance * 16f);
            fp_fogCol += fogColor.i;
            if (fp_fogCol > 0x0100) fp_fogCol = 0x0100;

            //Console.WriteLine(fp_fogCol.ToString("X8"));
            int fog_r = fp_fogCol * (fogColor.r);
            int fog_g = fp_fogCol * (fogColor.g);
            int fog_b = fp_fogCol * (fogColor.b);

            int fp_srcCol = 0x0100 - fp_fogCol;

            byte r = (byte)(((color.r * fp_srcCol) + fog_r) >> 8);
            byte g = (byte)(((color.g * fp_srcCol) + fog_g) >> 8);
            byte b = (byte)(((color.b * fp_srcCol) + fog_b) >> 8);

            for (int i = 0; i < width; i++)
            {
                pixels[dst] = r;
                pixels[dst + 1] = g;
                pixels[dst + 2] = b;
                pixels[dst + 3] = color.a;
                dst += bpp;
            }
        }

        public void TexturedRow(int screenY, float distance, PixelBuffer srcImage)
        {

        }
    }
}
