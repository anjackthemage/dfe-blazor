using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dfe.Shared.Render;
using System.Numerics;
using dfe.Shared.Utils.ExtensionMethods;
using System.Text.Json.Serialization;

namespace dfe.Shared.Entities
{
    public class Coord
    {
        [JsonInclude]
        public float X;
        [JsonInclude]
        public float Y;
        public Coord()
        {
            X = 0;
            Y = 0;
        }
        public Coord(float x, float y)
        {
            X = x;
            Y = y;
        }

        // Override .ToString
        public override string ToString()
        {
            return String.Format("X: {0}, Y: {1}", this.X, this.Y);
        }

        // Override .Equals
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Coord);
        }

        // Define actual check for equality on this type
        public bool Equals(Coord other)
        {
            // False if the other object is null
            if(Object.ReferenceEquals(other, null))
            {
                return false;
            }

            // True if the default process (more optimized) shows equality, 
            // such as when comparing object instance to itself?
            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            // If none of the shortcuts worked, do the actual comparison I guess.
            return (X == other.X) && (Y == other.Y);
        }

        public static bool operator ==(Coord left_coord, Coord right_coord)
        {
            // Check for nulls
            if(Object.ReferenceEquals(left_coord, null))
            {
                if(Object.ReferenceEquals(right_coord, null))
                {
                    // if both are null, they're equal
                    return true;
                }
                else
                {
                    // The case where left is not null but right is null
                    // is already covered in our .Equals method
                    return false;
                }
            }

            // Actually check, I guess
            return left_coord.Equals(right_coord);
        }

        public static bool operator !=(Coord left_coord, Coord right_coord)
        {
            // No need to duplicate code
            return !(left_coord == right_coord);
        }
    }

    public class Entity
    {
        public int id;

        [JsonInclude]
        public Guid guid;

        [JsonInclude]
        public Coord position = new Coord();

        [JsonInclude]
        public bool b_state_has_changed = false;
        
        public PixelBuffer sprite { get; set; }

        [JsonInclude]
        public int type;
        [JsonInclude]
        public int sprite_id;

        public Entity()
        {
            
        }
        public void update(float time)
        {

        }

    }
}
