using System.Runtime.InteropServices;

namespace dfe.Shared.Render
{
    /// <summary>
    /// Rgba - Represents an RGBA color.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct Rgba
    {
        // RGBA Color int union
        [FieldOffset(0)]
        public uint color;
        // Red Component
        [FieldOffset(3)]
        public byte r;
        // Green Component
        [FieldOffset(2)]
        public byte g;
        // Blue Componenet
        [FieldOffset(1)]
        public byte b;
        // Alpha Component
        [FieldOffset(0)]
        public byte a;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="red">Red Component (0-255)</param>
        /// <param name="green">Green Component (0-255)</param>
        /// <param name="blue">Blue Component (0-255)</param>
        public Rgba(int red, int green, int blue)
        {
            color = 0;
            r = (byte)red;
            g = (byte)green;
            b = (byte)blue;
            a = 0xFF;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="red">Red Component (0-255)</param>
        /// <param name="green">Green Component (0-255)</param>
        /// <param name="blue">Blue Component (0-255)</param>
        /// <param name="alpha">Alpha Component (0-255)</param>
        public Rgba(int red, int green, int blue, int alpha)
        {
            color = 0;
            r = (byte)red;
            g = (byte)green;
            b = (byte)blue;
            a = (byte)alpha;
        }
        /// <summary>
        /// Set the color components of this struct.
        /// </summary>
        /// <param name="red">Red Component (0-255)</param>
        /// <param name="green">Green Component (0-255)</param>
        /// <param name="blue">Blue Component (0-255)</param>
        public void set(int red, int green, int blue)
        {
            r = (byte)red;
            g = (byte)green;
            b = (byte)blue;
        }
        /// <summary>
        /// Set the color components of this struct.
        /// </summary>
        /// <param name="red">Red Component (0-255)</param>
        /// <param name="green">Green Component (0-255)</param>
        /// <param name="blue">Blue Component (0-255)</param>
        /// <param name="alpha">Alpha Component (0-255)</param>
        public void set(int red, int green, int blue, int alpha)
        {
            r = (byte)red;
            g = (byte)green;
            b = (byte)blue;
            a = (byte)alpha;
        }
    }
}
