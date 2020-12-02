using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using dfe.Shared.Entities;

namespace dfe.Server.Hubs
{
    public class PlayerHub : Hub
    {

        public static Dictionary<String, Player> connected_players = new Dictionary<string, Player>();

        public async Task registerPlayerConnection(string player_name)
        {
            Console.WriteLine("Registering client...");
            //Context.ConnectionId
            Player temp_player = new Player(0.0f, 0.0f, 0.0f);

            temp_player.player_name = player_name;
            temp_player.guid = Guid.NewGuid();
            // TODO: Check for reconnecting players an process accordingly
            if (!connected_players.ContainsKey(Context.ConnectionId))
            {
                connected_players.Add(Context.ConnectionId, temp_player);


                // TODO: Here we call the client and initiate map transfer.
                await Clients.Client(Context.ConnectionId).SendAsync("receiveRegistrationResponse", true, temp_player.guid);
                Console.WriteLine("Connected clients: {0}", connected_players.Count);

                await Clients.All.SendAsync("updateConnectedPlayers", connected_players.Values);
            }
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
            temp_player.coord.X = new_position.X;
            temp_player.coord.Y = new_position.Y;

            Dictionary<Guid, Coord> player_locations = new Dictionary<Guid, Coord>();

            foreach(KeyValuePair<string, Player> player_conn in connected_players)
            {
                Coord coords = new Coord();
                Player plyr = player_conn.Value;

                coords.X = plyr.position.X;
                coords.Y = plyr.position.Y;

                player_locations.Add(plyr.guid, coords);
            }

            await Clients.All.SendAsync("updatePlayerPositions", player_locations);
        }
    }
}
