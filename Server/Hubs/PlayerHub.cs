using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using dfe.Shared.Entities;
using dfe.Server.Engine;
using dfe.Shared.Render;

namespace dfe.Server.Hubs
{
    public class PlayerHub : Hub
    {

        public static Dictionary<String, Player> connected_players = new Dictionary<string, Player>();

        public async Task registerPlayerConnection(Player player_ref)
        {
            Console.WriteLine("Registering client...");

            Player temp_player = player_ref;

            temp_player.guid = Guid.NewGuid();
            // TODO: Check for reconnecting players an process accordingly
            if (!connected_players.ContainsKey(Context.ConnectionId))
            {
                connected_players.Add(Context.ConnectionId, temp_player);
                Console.WriteLine(connected_players[Context.ConnectionId].position.ToString());
                // TODO: Here we call the client and initiate map transfer.
                await Clients.Client(Context.ConnectionId).SendAsync("receiveRegistrationResponse", true, temp_player.guid);
                Console.WriteLine("Connected clients: {0}", connected_players.Count);

                updateConnectedPlayers();
            }
        }

        public async Task updateConnectedPlayers()
        {
            await Clients.All.SendAsync("updateConnectedPlayers", connected_players.Values);
        }

        public async Task deregisterPlayerConnection()
        {
            Console.WriteLine("Deregistering client...");
            connected_players.Remove(Context.ConnectionId);
        }

        public async Task updatePlayerPosition(Coord new_position)
        {
            Player temp_player = connected_players[Context.ConnectionId];

            temp_player.position.X = new_position.X;
            temp_player.position.Y = new_position.Y;

            Dictionary<Guid, Coord> player_locations = new Dictionary<Guid, Coord>();

            foreach (KeyValuePair<string, Player> player_conn in connected_players)
            {
                Coord coords = new Coord();
                Player plyr = player_conn.Value;

                coords.X = plyr.position.X;
                coords.Y = plyr.position.Y;

                player_locations.Add(plyr.guid, coords);
            }

            await Clients.All.SendAsync("updatePlayerPositions", player_locations);
        }

        // get player state from client

        // send client new map state

        // send client updated/new entities

        // send client updated/new mobs

        // send client updated/new players

        #region Heartbeat
        public void initiateHeartbeat()
        {
            Clients.All.SendAsync("doHeartbeat");
        }

        public async Task receivePlayerHeartbeat(Player connected_player)
        {
            if (connected_player.guid != connected_players[Context.ConnectionId].guid)
            {
                Console.WriteLine("Heartbeat: Out of sync: Bad GUID");

                Console.WriteLine("Player GUID (Server): {0}", connected_players[Context.ConnectionId].guid);
                Console.WriteLine("Player GUID (Client): {0}", connected_player.guid);
                // New player on same connection
                // Force authentication
            }
            else if (connected_player.position != connected_players[Context.ConnectionId].position)
            {
                Console.WriteLine("Heartbeat: Out of sync: Bad position.");

                Console.WriteLine("Player {0} Position (server): {1}", connected_players[Context.ConnectionId].player_name, connected_players[Context.ConnectionId].position.ToString());
                Console.WriteLine("Player {0} Position (client): {1}", connected_player.player_name, connected_player.position.ToString());
                // We're out of sync
                // Send map data
            }
            else
            {
                Console.WriteLine("Heartbeat: N'sync.");
            }
        }
        #endregion

        #region asset transfer
        public async Task getTextures()
        {
            await Clients.Client(Context.ConnectionId).SendAsync("receiveTextureDefs", GameServer.server.texture_assets);
        }

        public async Task getSprites()
        {
            await Clients.Client(Context.ConnectionId).SendAsync("receiveSpriteDefs", GameServer.server.sprite_assets);
        }

        public async Task getTexturePixels(int id)
        {
            TextureDef tex = null;

            if (GameServer.server.texture_assets.TryGetValue(id, out tex) == false)
            {
                Console.WriteLine("Unable to fetch requested texture : " + id);
                return;
            }
            else
            {
                PixelBuffer buffer = tex.pixelBuffer;
                await Clients.Client(Context.ConnectionId).SendAsync("receiveTexturePixels", id, buffer.width, buffer.height, buffer.pixels);
            }
        }

        public async Task getSpritePixels(int id)
        {
            SpriteDef spr = null;
            if (GameServer.server.sprite_assets.TryGetValue(id, out spr) == false)
            {
                Console.WriteLine("Unable to fetch requested sprite : " + id);
                return;
            }
            else
            {
                PixelBuffer buffer = spr.pixelBuffer;
                await Clients.Client(Context.ConnectionId).SendAsync("receiveSpritePixels", id, buffer.width, buffer.height, buffer.pixels);
            }
        }
        #endregion

    }
}
