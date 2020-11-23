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
            Console.WriteLine(intensity);
        }
    }

    public class PixelBuffer
    {
        public const int BPP = 4;
        public readonly int Width;
        public readonly int Height;
        public readonly int Stride;
        public byte[] Pixels;
        public PixelBuffer(int width, int height)
        {
            Width = width;
            Height = height;
            Stride = width * BPP;
            Pixels = new byte[width * height * BPP];
        }

        public void DrawPoint(int x, int y, Color4i color)
        {
            int i = (x << 2) + (y * Stride) % Pixels.Length;
            Pixels[i] = color.r;
            Pixels[i + 1] = color.g;
            Pixels[i + 2] = color.b;
            Pixels[i + 3] = color.a;
        }

        public void DrawPoint(int x, int y, byte r, byte g, byte b)
        {
            int i = (x << 2) + (y * Stride) % Pixels.Length;
            Pixels[i] = r;
            Pixels[i + 1] = g;
            Pixels[i + 2] = b;
            Pixels[i + 3] = 255;
        }

        public void Clear()
        {
            for (int i = 0; i < Width * Height * BPP; i += BPP)
            {
                Pixels[i] = 0;
                Pixels[i + 1] = 0;
                Pixels[i + 2] = 0;
                Pixels[i + 3] = 255;
            }
        }
        public void Clear(Color4i color)
        {
            for (int i = 0; i < Width * Height * BPP; i += BPP)
            {
                Pixels[i] = color.r;
                Pixels[i + 1] = color.g;
                Pixels[i + 2] = color.b;
                Pixels[i + 3] = color.a;
            }
        }
        public void ShadeWall(int x, float distance, Color4i color)
        {
            int colHeight = Height;
            if (distance != 0)
                colHeight = (int)(Height / distance);
            if (colHeight > Height || distance == 0) 
                colHeight = Height;
            byte c = (byte)colHeight;
            // Find the top of the column
            int y = (x * BPP) + (((Height - colHeight) >> 1) * Stride);
            for(int i = 0; i < colHeight; i++)
            {
                Pixels[y] = c;
                Pixels[y + 1] = c;
                Pixels[y + 2] = c;
                Pixels[y + 3] = 255;
                y += Stride;
            }
        }

        public void TexturedWall(int x, float distance, float texOfs, PixelBuffer srcImage)
        {
            // Calculate initial source texture position.
            int fp_Src = (int)((float)srcImage.Height * texOfs);
            fp_Src = fp_Src * srcImage.Width;
            fp_Src = fp_Src << 16;

            // Calculate the height of the column render.
            int colHeight = Height;
            if (distance != 0) colHeight = (int)(Height / distance);

            // Calculate base step ratio for source texture read.
            int fpSrcStep = (srcImage.Width << 16) / colHeight; 

            // Handle columns too tall for the screen.
            if (colHeight > Height || distance == 0)
            {
                // Adjust the source texture data offset.
                fp_Src += fpSrcStep * ((colHeight - Height) >> 1);
                // Clip the column height to screen height.
                colHeight = Height;
            }
            // Calculate the destination data offset for the write.
            int dst = (x * BPP) + (((Height - colHeight) >> 1) * Stride);
            int src;

            // Draw the column.
            for (int i = 0; i < colHeight; i++)
            {
                src = (fp_Src >> 16) << 2;
                Pixels[dst] = (byte)(srcImage.Pixels[src]);
                Pixels[dst + 1] = (byte)(srcImage.Pixels[src + 1]);
                Pixels[dst + 2] = (byte)(srcImage.Pixels[src + 2]);
                Pixels[dst + 3] = (byte)(srcImage.Pixels[src + 3]);
                fp_Src += fpSrcStep;
                dst += Stride;
            }
        }

        public void ShadeTexturedWall(int x, float distance, float texOfs, PixelBuffer srcImage, Fog4i fogColor)
        {
            // Calculate initial source texture position.
            int fp_Src = (int)((float)srcImage.Height * texOfs);
            fp_Src = fp_Src * srcImage.Width;
            fp_Src = fp_Src << 16;

            // Calculate the height of the column render.
            int colHeight = Height;
            if (distance != 0) colHeight = (int)(Height / distance);

            // Calculate base step ratio for source texture read.
            int fpSrcStep = (srcImage.Width << 16) / colHeight;

            // Handle columns too tall for the screen.
            if (colHeight > Height || distance == 0)
            {
                // Adjust the source texture data offset.
                fp_Src += fpSrcStep * ((colHeight - Height) >> 1);
                // Clip the column height to screen height.
                colHeight = Height;
            }
            // Calculate the destination data offset for the write.
            int dst = (x * BPP) + (((Height - colHeight) >> 1) * Stride);

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
            // Draw the column.
            for (int i = 0; i < colHeight; i++)
            {
                src = (fp_Src >> 16) << 2;

                fp_r = (srcImage.Pixels[src] * fp_srcCol) + fog_r;
                fp_g = (srcImage.Pixels[src + 1] * fp_srcCol) + fog_g;
                fp_b = (srcImage.Pixels[src + 2] * fp_srcCol) + fog_b;

                Pixels[dst] = (byte)(fp_r >> 8);
                Pixels[dst + 1] = (byte)(fp_g >> 8);
                Pixels[dst + 2] = (byte)(fp_b >> 8);
                Pixels[dst + 3] = 255;
                fp_Src += fpSrcStep;
                dst += Stride;
            }
        }

    }
}
