using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Numerics;
using dfe.Shared.Input;

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
            this.position = new Coord(x_initial, y_initial);
            this.position = new Coord();
            this.position.X = x_initial;
            this.position.Y = y_initial;
            this.angle = angle_initial;
        }

        public void update(float time, InputState input_state)
        {
            float velocity = 0.5f * time;
            if (input_state.u == true)
            {
                walk(velocity);
            }
            else if (input_state.d == true)
            {
                walk(-velocity);
            }

            if (input_state.l == true)
            {
                strafe(-velocity);
            }
            else if (input_state.r == true)
            {
                strafe(velocity);
            }

            if (input_state.mouseDelta != 0)
            {
                rotate(input_state.mouseDelta / 64);
                input_state.mouseDelta = 0;
            }
        }
    }
}
