using dfe.Shared.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dfe.Client.Engine
{
    public class ClientSimulation
    {
        public InputState input_state;
        public ClientSimulation() { }
        /// <summary>
        /// Process one frame of game logic.
        /// </summary>
        /// <param name="time">The amount of time that has passed since the last process.</param>
        public void process(float time, ClientState state)
        {
            // Very basic input handling.
            state.player.update(time, input_state);
            input_state.mouseDelta = 0;
        } 
    }
}
