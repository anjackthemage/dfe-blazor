using dfe.Client.Engine.Network;
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

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        private HubConnection chat_hub_conn;
        private HubConnection map_hub_conn;
        private HubConnection player_hub_conn;

        public Tracer ray_tracer;
        public  ChatClient chat_client;
        private MapClient map_client;
        private PlayerClient player_client;

        public static bool b_is_running { get; private set; }

        public GameClient(Uri ph_uri, Uri mh_uri, Uri ch_uri)
        {
            client = this;

            ray_tracer = new Tracer();

            player_hub_conn = new HubConnectionBuilder().WithUrl(ph_uri).Build();

            map_hub_conn = new HubConnectionBuilder().WithUrl(mh_uri).Build();

            chat_hub_conn = new HubConnectionBuilder().WithUrl(ch_uri).Build();


            player_client = new PlayerClient(player_hub_conn);

            map_client = new MapClient(map_hub_conn);

            map_client.loadMapFromServer();

            chat_client = new ChatClient(chat_hub_conn);
            //chat_client.onChatUpdated += refreshMessages;

            player_hub_conn.SendAsync("registerPlayerConnection", "TestPlayer", ray_tracer.self);



            b_is_running = true;

            //doGameLoop();
            //while (b_is_running)
            //{
            //    // do client stuff
            //}
        }

        public async Task doGameLoop()
        {
            while (b_is_running)
            {
                
            }
        }

        public async Task render()
        {
            ray_tracer.updateObserver();

            // TODO: Move these calls to PlayerClient, don't call updateConnectedPlayers every frame.
            player_hub_conn.SendAsync("updateConnectedPlayers");
            player_hub_conn.SendAsync("updatePlayerPosition", ray_tracer.self.coord);
            map_client.level_map.render();

            foreach (KeyValuePair<Guid, Player> player_conn in player_client.connected_players)
            {
                try
                {
                    player_conn.Value.render();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: {0}", e.ToString());
                }
            }

            //presentScreen(JsRuntime, ray_tracer.frameBuffer);
        }

        public void handleKeyboardInput(KeyboardEventArgs kb_args)
        {
            switch(kb_args.Type)
            {
                case "keyup":
                    switch(kb_args.Key)
                    {
                        case "w":
                            ray_tracer.input.u = false;
                            break;
                        case "s":
                            ray_tracer.input.d = false;
                            break;
                        case "a":
                            ray_tracer.input.l = false;
                            break;
                        case "d":
                            ray_tracer.input.r = false;
                            break;
                        default:
                            break;
                    }
                    break;
                case "keydown":
                    switch (kb_args.Key)
                    {
                        case "w":
                            ray_tracer.input.u = true;
                            break;
                        case "s":
                            ray_tracer.input.d = true;
                            break;
                        case "a":
                            ray_tracer.input.l = true;
                            break;
                        case "d":
                            ray_tracer.input.r = true;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("Unhandled KeyboardEventArgs.Type: {0}", kb_args.Type);
                    break;
            }
        }
    }
}
