using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using dfe.Shared;
using System.Text.Json;
using dfe.Shared.Render;

namespace dfe.Client.Engine.Network
{
    public class MapClient
    {
        public Tracer ray_tracer;
        public HubConnection map_hub_conn;

        public Map level_map;

        public MapClient(HubConnection new_hub_conn)
        {
            level_map = new Map(16, 16);
            map_hub_conn = new_hub_conn;

            map_hub_conn.On<float[]>("receiveMap", (data) =>
            {
                level_map.map_contents = data;
                ray_tracer.lvl_map = level_map;
            });

            map_hub_conn.On<byte[]>("receiveImage", (data) =>
            {
                ray_tracer.s_tex.pixels = data;

            });

            map_hub_conn.StartAsync();

        }
        
        public void loadMapFromServer()
        {

            map_hub_conn.SendAsync("getMap");

            map_hub_conn.SendAsync("getImage", "Assets/Sprites/Jonesy.data");
        }
    }
}
