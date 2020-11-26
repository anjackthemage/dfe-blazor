using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using dfe.Shared;

namespace dfe.Client.Engine.Network
{
    public class MapClient
    {
        public HubConnection map_hub_conn;

        public Map level_map;

        private string img_test_string;

        public MapClient(HubConnection new_hub_conn)
        {
            level_map = new Map(16, 16);
            map_hub_conn = new_hub_conn;

            map_hub_conn.On<float[]>("receiveMap", (data) =>
            {
                level_map.map_contents = data;
            });


            map_hub_conn.On<byte[]>("receiveImage", (data) =>
            {
                string img_str = Convert.ToBase64String(data);
                //string img_str = System.Text.Encoding.UTF8.GetString(data);
                img_test_string = string.Format("data:{0};base64,{1}", "img/png", img_str);
                Console.WriteLine("Image: {0}", img_test_string);
            });

            map_hub_conn.StartAsync();

            map_hub_conn.SendAsync("getMap");

            map_hub_conn.SendAsync("getImage", "Assets/Sprites/Jonesy.png");
        }
        
    }
}
