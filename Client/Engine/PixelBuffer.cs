using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dfe.Client.Engine
{
    public struct Color4i
    {
        public byte r, g, b, a;
        public Color4i(byte red, byte green, byte blue) {
            r = red;
            g = green;
            b = blue;
            a = 255; 
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

        public void RenderWall(int x, float distance, float texOfs, PixelBuffer srcImage)
        {
            int colHeight = Height;
            if (distance != 0) colHeight = (int)(Height / distance);
            if (colHeight > Height || distance == 0)
                colHeight = Height;

            int dst = (x * BPP) + (((Height - colHeight) >> 1) * Stride);

            int fpSrc = (int)((float)srcImage.Height * texOfs);
            fpSrc = fpSrc * srcImage.Width;
            fpSrc = fpSrc << 16;

            // int fpSrc = (int)((float)srcImage.Height * texOfs) << 16;

            int fpSrcStep = (srcImage.Width << 16) / colHeight; // fixed point maths.
            int src;
            for(int i = 0; i < colHeight; i++)
            {
                src = (fpSrc >> 16) << 2;
                Pixels[dst] = srcImage.Pixels[src];
                Pixels[dst + 1] = srcImage.Pixels[src + 1];
                Pixels[dst + 2] = srcImage.Pixels[src + 2];
                Pixels[dst + 3] = srcImage.Pixels[src + 3];
                fpSrc += fpSrcStep;
                dst += Stride;
            }
        }

    }
}
