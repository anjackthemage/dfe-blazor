using dfe.Client.Engine.Network;
using dfe.Shared;
using dfe.Shared.Entities;
using dfe.Shared.Render;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dfe.Client.Engine
{
    public class GameClient
    {
        public static GameClient client;
        // Game logic, used for updating things in a ClientState
        public static ClientSimulation game_sim;
        // Current state of the client game world.
        public static ClientState game_state;
        // Handles high level rendering procedures.
        public static Renderer renderer;

        public IJSRuntime JsRuntime;

        private HubConnection chat_hub_conn;
        private HubConnection map_hub_conn;
        private HubConnection player_hub_conn;

        //public Renderer ray_tracer;
        public  ChatClient chat_client;
        private MapClient map_client;
        private PlayerClient player_client;


        public static bool b_is_running { get; private set; }

        public GameClient(Uri ph_uri, Uri mh_uri, Uri ch_uri)
        {
            client = this;
            // Create game simulation.
            game_sim = new ClientSimulation();
            // Create game state.
            game_state = new ClientState();
            // Create rendering instance
            renderer = new Renderer(320, 240);

            // Create connection Hubs
            player_hub_conn = new HubConnectionBuilder().WithUrl(ph_uri).Build();
            map_hub_conn = new HubConnectionBuilder().WithUrl(mh_uri).Build();
            chat_hub_conn = new HubConnectionBuilder().WithUrl(ch_uri).Build();


            player_client = new PlayerClient(player_hub_conn);
            map_client = new MapClient(map_hub_conn);

            map_client.loadMapFromServer();

            chat_client = new ChatClient(chat_hub_conn);
            //chat_client.onChatUpdated += refreshMessages;

            player_hub_conn.SendAsync("registerPlayerConnection", game_state.player);

            player_hub_conn.SendAsync("getSprites");
            player_hub_conn.SendAsync("getTextures");

            b_is_running = true;

            doGameLoop();
            
        }

        private async Task doGameLoop()
        {
            while (b_is_running)
            {

                // Pause execution on this thread to let main thread process for a bit
                await Task.Delay(1000);
            }
        }

        public async Task render()
        {
            renderer.render();

            // TODO: Move these calls to PlayerClient, don't call updateConnectedPlayers every frame.
            //player_hub_conn.SendAsync("updateConnectedPlayers");
            player_hub_conn.SendAsync("updatePlayerPosition", game_state.player.position);

        }
        public static double lastTime = 0;
        public async Task update(double time)
        {
            double deltaTime = time - lastTime;
            deltaTime = deltaTime / 1000; // Milliseconds to Seconds
            if (deltaTime >= (1f / 60f))
            {
                game_sim.process((float)deltaTime, game_state);
                lastTime = time;
            }
        }

        public void handleKeyboardInput(KeyboardEventArgs kbe_args)
        {
            switch(kbe_args.Type)
            {
                case "keyup":
                    switch(kbe_args.Key)
                    {
                        case "w":
                            game_sim.input_state.u = false;
                            break;
                        case "s":
                            game_sim.input_state.d = false;
                            break;
                        case "a":
                            game_sim.input_state.l = false;
                            break;
                        case "d":
                            game_sim.input_state.r = false;
                            break;
                        default:
                            break;
                    }
                    break;
                case "keydown":
                    switch (kbe_args.Key)
                    {
                        case "w":
                            game_sim.input_state.u = true;
                            break;
                        case "s":
                            game_sim.input_state.d = true;
                            break;
                        case "a":
                            game_sim.input_state.l = true;
                            break;
                        case "d":
                            game_sim.input_state.r = true;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("Unhandled KeyboardEventArgs.Type: {0}", kbe_args.Type);
                    break;
            }
        }

        
        public async void handleMouseInput(MouseEventArgs me_args)
        {
            float x = await JsRuntime.InvokeAsync<float>("pollMouse");
            game_sim.input_state.mouseDelta = x;
        }

        public void requestTexturePixels(int texture_id)
        {
            Console.WriteLine("Requesting texture..");
            player_hub_conn.SendAsync("getTexturePixels", texture_id);
        }

        public void requestSpritePixels(int sprite_id)
        {
            Console.WriteLine("Requesting sprite..");
            player_hub_conn.SendAsync("getSpritePixels", sprite_id);
        }


        public event Action OnPlayersUpdated;

        public void updatePlayers()
        {
            OnPlayersUpdated?.Invoke();
        }
    }
}
