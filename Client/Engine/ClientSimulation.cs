using dfe.Shared.Input;
using dfe.Shared.Entities;

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
            // Handle input for this frame.
            state.player.update(time, input_state);
            input_state.mouseDelta = 0;
            // Update local entities.
            foreach (Mob mob in state.local_mobs.Values)
                mob.update(time);
        } 
    }
}