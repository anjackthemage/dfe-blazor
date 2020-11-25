using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dfe.Client.Engine
{
    public struct Color4i
    {
        public byte r, g, b, a;
        public Color4i(byte red, byte green, byte blue)
        {
            r = red;
            g = green;
            b = blue;
            a = 255;
        }
    }

    public struct Fog4i
    {
        public byte r, g, b, i;
        public Fog4i(byte red, byte green, byte blue)
        {
            r = red;
            g = green;
            b = blue;
            i = 0x80;
        }

        public Fog4i(Color4i color, float intensity)
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
        public const int bpp = 4;
        public readonly int width;
        public readonly int height;
        public readonly int stride;
        public byte[] pixels;
        public PixelBuffer(int width, int height)
        {
            this.width = width;
            this.height = height;
            stride = width * bpp;
            pixels = new byte[width * height * bpp];
        }

        public void DrawPoint(int x, int y, Color4i color)
        {
            int i = (x << 2) + (y * stride) % pixels.Length;
            pixels[i] = color.r;
            pixels[i + 1] = color.g;
            pixels[i + 2] = color.b;
            pixels[i + 3] = color.a;
        }

        public void DrawPoint(int x, int y, byte r, byte g, byte b)
        {
            int i = (x << 2) + (y * stride) % pixels.Length;
            pixels[i] = r;
            pixels[i + 1] = g;
            pixels[i + 2] = b;
            pixels[i + 3] = 255;
        }

        public void DrawRect(int x, int y, int w, int h, Color4i c)
        {
            // Find clipping rectangle.
            int left = Math.Max(x, 0);
            int right = Math.Min(x + w, width - 1);
            int top = Math.Max(y, 0);
            int bot = Math.Min(y + h, height - 1);
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
                    pixels[dstA + 0] = c.r;
                    pixels[dstA + 1] = c.g;
                    pixels[dstA + 2] = c.b;
                    pixels[dstA + 3] = c.a;
                }
                if (right == x + w)
                {
                    pixels[dstB + 0] = c.r;
                    pixels[dstB + 1] = c.g;
                    pixels[dstB + 2] = c.b;
                    pixels[dstB + 3] = c.a;
                }
                dstA += stride;
                dstB += stride;
            }

            dstB = dy + (left << 2);
            // Draw rows
            for(int i = 0; i < clipW; i++)
            {
                if(bot == (y + h))
                {
                    pixels[dstA + 0] = c.r;
                    pixels[dstA + 1] = c.g;
                    pixels[dstA + 2] = c.b;
                    pixels[dstA + 3] = c.a;

                }
                if (top == y)
                {
                    pixels[dstB + 0] = c.r;
                    pixels[dstB + 1] = c.g;
                    pixels[dstB + 2] = c.b;
                    pixels[dstB + 3] = c.a;
                }
                dstA += bpp;
                dstB += bpp;
            }

        }
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
        public void Clear(Color4i color)
        {
            for (int i = 0; i < width * height * bpp; i += bpp)
            {
                pixels[i] = color.r;
                pixels[i + 1] = color.g;
                pixels[i + 2] = color.b;
                pixels[i + 3] = color.a;
            }
        }
        public void ShadeWall(int x, float distance, Color4i color)
        {
            int colHeight = height;
            if (distance != 0)
                colHeight = (int)(height / distance);
            if (colHeight > height || distance == 0) 
                colHeight = height;
            byte c = (byte)colHeight;
            // Find the top of the column
            int y = (x * bpp) + (((height - colHeight) >> 1) * stride);
            for(int i = 0; i < colHeight; i++)
            {
                pixels[y] = c;
                pixels[y + 1] = c;
                pixels[y + 2] = c;
                pixels[y + 3] = 255;
                y += stride;
            }
        }

        public void TexturedWall(int x, float distance, float texOfs, PixelBuffer srcImage)
        {
            // Calculate initial source texture position.
            int fp_Src = (int)((float)srcImage.height * texOfs);
            fp_Src = fp_Src * srcImage.width;
            fp_Src = fp_Src << 16;

            // Calculate the height of the column render.
            int colHeight = height;
            if (distance != 0) colHeight = (int)(height / distance);

            // Calculate base step ratio for source texture read.
            int fpSrcStep = (srcImage.width << 16) / colHeight; 

            // Handle columns too tall for the screen.
            if (colHeight > height || distance == 0)
            {
                // Adjust the source texture data offset.
                fp_Src += fpSrcStep * ((colHeight - height) >> 1);
                // Clip the column height to screen height.
                colHeight = height;
            }
            // Calculate the destination data offset for the write.
            int dst = (x * bpp) + (((height - colHeight) >> 1) * stride);
            int src;

            // Draw the column.
            for (int i = 0; i < colHeight; i++)
            {
                src = (fp_Src >> 16) << 2;
                pixels[dst] = (byte)(srcImage.pixels[src]);
                pixels[dst + 1] = (byte)(srcImage.pixels[src + 1]);
                pixels[dst + 2] = (byte)(srcImage.pixels[src + 2]);
                pixels[dst + 3] = (byte)(srcImage.pixels[src + 3]);
                fp_Src += fpSrcStep;
                dst += stride;
            }
        }

        public void ShadeTexturedWall(int x, float distance, float texOfs, PixelBuffer srcImage, Fog4i fogColor)
        {
            // Calculate initial source texture position.
            int fp_Src = (int)((float)srcImage.height * texOfs);
            fp_Src = fp_Src * srcImage.width;
            fp_Src = fp_Src << 16;

            // Calculate the height of the column render.
            int colHeight = height;
            if (distance != 0) colHeight = (int)(height / distance);

            // Calculate base step ratio for source texture read.
            int fp_srcStep = (srcImage.width << 16) / colHeight;

            // Handle columns too tall for the screen.
            if (colHeight > height || distance == 0)
            {
                // Adjust the source texture data offset.
                fp_Src += fp_srcStep * ((colHeight - height) >> 1);
                // Clip the column height to screen height.
                colHeight = height;
            }
            // Calculate the destination data offset for the write.
            int dst = (x * bpp) + (((height - colHeight) >> 1) * stride);

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

                    src = (fp_Src >> 16) << 2;
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
                    fp_nextSrc = (fp_Src + 0x10000) & 0xFFF0000;
                    src = (fp_Src >> 16) << 2;
                    fp_r = ((srcImage.pixels[src] * fp_srcCol) + fog_r) >> 8;
                    fp_g = ((srcImage.pixels[src + 1] * fp_srcCol) + fog_g) >> 8;
                    fp_b = ((srcImage.pixels[src + 2] * fp_srcCol) + fog_b) >> 8;
                    r = (byte)fp_r;
                    g = (byte)fp_g;
                    b = (byte)fp_b;
                    while (fp_Src < fp_nextSrc && i < colHeight)
                    {
                        pixels[dst] = r;
                        pixels[dst + 1] = g;
                        pixels[dst + 2] = b;
                        pixels[dst + 3] = 255;
                        fp_Src += fp_srcStep;
                        dst += stride;
                        i++;
                    }
                }
            }
        }

        // Worker clipping rectangle.
        private static ClipRect cRect = new ClipRect();
        private static Color4i red = new Color4i(255, 0, 0);
        private static Color4i white = new Color4i(255, 255, 255);
        public void DrawRectPerspective(int x, float distance, int w, int h, ray[] ray_buffer, Color4i c)
        {
            w = (int)(w / distance);
            h = (int)(h / distance);
            int hw = (int)((w >> 1));
            int hh = (int)((h >> 1));

            cRect.x = x - hw;
            cRect.y = (height >> 1) - hh;
            cRect.w = w;
            cRect.h = h;
            DrawRect(cRect.x, cRect.y, cRect.w, cRect.h, red);

            cRect.left = Math.Max(cRect.x, 0);
            cRect.right = Math.Min(cRect.x + cRect.w, width - 1);
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
            DrawRect(cRect.left, cRect.top, cRect.right - cRect.left, cRect.bot - cRect.top, c);
        }
        public void DrawSpritePerspective(int screenX, float distance, ray[] ray_buffer, PixelBuffer sprite)
        {
            int w = (int)(sprite.width / distance);
            int h = (int)(sprite.height / distance);
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

            // TODO : remove me :)
            DrawRect(cRect.x, cRect.y, cRect.w, cRect.h, red);

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
            // TODO : remove me :)
            DrawRect(cRect.left, cRect.top, cRect.right - cRect.left, cRect.bot - cRect.top, white);

        }
    }
}
