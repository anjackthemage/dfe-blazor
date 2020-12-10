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
        [JsonInclude]
        public String player_name = "Test Player";

        [JsonInclude]
        public bool b_is_disconnect = false;

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
            this.sprite_id = 1;
        }

        public void update(float time, InputState input_state)
        {
            float velocity = 32 * time;
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
            }
        }
    }
}
