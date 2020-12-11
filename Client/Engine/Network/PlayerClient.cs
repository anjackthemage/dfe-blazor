using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using dfe.Shared.Entities;
using dfe.Shared.Render;
using dfe.Shared;
using System.IO;
using System.IO.Compression;

namespace dfe.Client.Engine.Network
{
    public class PlayerClient
    {
        public static PlayerClient player_client;

        public static bool b_is_player_registered = false;

        public HubConnection player_hub_conn;

        public Guid local_player_guid;

        public Dictionary<Guid, Player> connected_players = new Dictionary<Guid, Player>();

        public PlayerClient(HubConnection new_hub_conn)
        {
            player_hub_conn = new_hub_conn;

            /// Server's response to our registration attempt
            player_hub_conn.On<bool, Guid>("receiveRegistrationResponse", (b_is_registered, player_guid) =>
            {
                local_player_guid = player_guid;
                if (b_is_registered)
                {


                    GameClient.game_state.player.guid = local_player_guid;
                    b_is_player_registered = true;


                }
                else
                {
                    Console.WriteLine("Client registration failed.");
                }


            });

            /// When a player connects or disconnects, a signal is sent out to all other players to update their info
            player_hub_conn.On<Player[]>("updateConnectedPlayers", (player_connections) =>
            {
                foreach (Player plyr in player_connections)
                {
                    if (plyr.guid != local_player_guid)
                    {
                        Console.WriteLine("Loading player: {0}", plyr.guid);
                        if (GameClient.game_state.local_players.Count > 0 && GameClient.game_state.local_players.ContainsKey(plyr.guid))
                        {
                            GameClient.game_state.local_players[plyr.guid] = plyr;
                        }
                        else
                        {
                            Console.WriteLine("New player!");
                            Player temp_player = new Player(plyr.position.X, plyr.position.Y, 0.0f);
                            temp_player = plyr;
                            GameClient.game_state.local_players.Add(temp_player.guid, temp_player);
                        }

                        if(GameClient.game_state.local_players[plyr.guid].b_is_disconnect)
                        {
                            Console.WriteLine("Player disconnected: {0}", plyr.guid);
                            GameClient.game_state.local_players.Remove(plyr.guid);
                        }

                        GameClient.client.updateRemotePlayerList();
                    }
                }
            });

            /// Update the position information for all other players
            player_hub_conn.On<Dictionary<Guid, Coord>>("updatePlayerPositions", (player_positions) =>
            {
                foreach (KeyValuePair<Guid, Coord> player_conn in player_positions)
                {
                    Guid plyr_id = player_conn.Key;
                    Coord position = player_conn.Value;
                    if (GameClient.game_state.local_players.ContainsKey(plyr_id) && !GameClient.game_state.local_players[plyr_id].b_is_disconnect)
                    {
                        GameClient.game_state.local_players[plyr_id].position.X = position.X;
                        GameClient.game_state.local_players[plyr_id].position.Y = position.Y;
                        // TODO: Add heading info to this communication
                    }
                    // TODO: If we get a position update for an untracked player, request player info from server
                }
            });

            /// Periodic challenge/response to maintain sync
            player_hub_conn.On("doHeartbeat", () =>
            {
                if (b_is_player_registered)
                {
                    player_hub_conn.SendAsync("receivePlayerHeartbeat", GameClient.game_state.player);
                }
            });

            /// If the server thinks we're where we are not suppposed to be, it will send us this
            player_hub_conn.On<Player>("updatePlayerFromServer", (player) =>
            {
                if (player.guid == GameClient.game_state.player.guid)
                {
                    // This is where we update our position to match the server
                    //GameClient.game_state.player.heading = player.heading;
                    //GameClient.game_state.player.position = player.position;
                }
            });

            player_hub_conn.StartAsync();

            player_client = this;

            #region asset transfer
            player_hub_conn.On<Dictionary<int, TextureDef>>("receiveTextureDefs", (t_dict) =>
            {
                Console.WriteLine("Textures received!");
                GameClient.renderer.textures = t_dict;
                GameClient.renderer.verifyTextures();
            });

            player_hub_conn.On<Dictionary<int, SpriteDef>>("receiveSpriteDefs", (s_dict) =>
            {
                Console.WriteLine("Sprites received!");
                GameClient.renderer.sprites = s_dict;
                GameClient.renderer.verifySprites();
            });

            player_hub_conn.On<int, int, int, byte[]>("receiveSpritePixels", (id, width, height, data) =>
            {
                Console.WriteLine("Receiving sprite : " + id);
                PixelBuffer pixelBuffer = new PixelBuffer(width, height);
                pixelBuffer.pixels = data;
                pixelBuffer.decompressPixels();
                GameClient.renderer.sprites[id].pixelBuffer = pixelBuffer;
                GameClient.renderer.missing_sprites.Remove(id);
            });

            player_hub_conn.On<int, int, int, byte[]>("receiveTexturePixels", (id, width, height, data) =>
            {
                Console.WriteLine("Receiving textured : " + id);
                PixelBuffer pixelBuffer = new PixelBuffer(width, height);
                pixelBuffer.pixels = data;
                pixelBuffer.decompressPixels();
                GameClient.renderer.textures[id].pixelBuffer = pixelBuffer;
                GameClient.renderer.missing_textures.Remove(id);
            });
            #endregion
        }
    }
}
