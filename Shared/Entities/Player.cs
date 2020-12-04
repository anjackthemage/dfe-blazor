using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Numerics;


namespace dfe.Shared.Entities
{
    public class Player : Mob
    {
        // For client-side, this is where we will store the local player, server can ignore this
        public static Player local;

        [JsonInclude]
        public String player_name;

        public Player()
        {

        }
        public Player(float x_initial, float y_initial, float angle_initial)
        {
            this.position = new Vector2(x_initial, y_initial);
            this.coord = new Coord();
            this.coord.X = x_initial;
            this.coord.Y = y_initial;
            this.angle = angle_initial;
        }
    }
}
