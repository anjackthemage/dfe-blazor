using dfe.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dfe.Shared.Render
{
    /// <summary>
    /// Represents a sprite's visible properties.
    /// Used during rendering to cull, position, and scale a sprite.
    /// </summary>
    public class SpriteVis
    {
        // X coordinate of sprite
        public float x;
        // Y coordinate of sprite
        public float y;
        // Distance from observer
        public float distance;
        // X position on the screen
        public int screen_x;
        // The entity to reference. Probably deprecate this.
        public int sprite_id;

        public SpriteVis(Entity ent)
        {
            x = ent.position.X;
            y = ent.position.Y;
            sprite_id = ent.sprite_id;
        }
    }
}
