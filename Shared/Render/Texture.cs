using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dfe.Shared.Render
{
    public class Texture
    {
        // Lookup Id number for this texture.
        public int id { get; set; }
        // Filename for this texture, used on the server side.
        public string file { get; set; }
        // The pixel buffer loaded into this texture.
        public PixelBuffer pixelBuffer { get; set; }
    }
}
