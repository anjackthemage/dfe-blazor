using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using dfe.Shared;
using System.Text.Json;

namespace dfe.Client.Engine.Network
{
    public class MapClient
    {
        public Tracer ray_tracer;
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
                Console.WriteLine("BEFORE COPY: {0}", ray_tracer.lvl_map.map_contents.Length);
                //foreach (byte pxl in ray_tracer.lvl_map.map_contents)
                //{
                //    Console.WriteLine(pxl);
                //}
                
                ray_tracer.lvl_map = level_map;
                Console.WriteLine("AFTER COPY: {0}", ray_tracer.lvl_map.map_contents.Length);
                //foreach (byte pxl in ray_tracer.lvl_map.map_contents)
                //{
                //    Console.WriteLine(pxl);
                //}
            });

            map_hub_conn.On<byte[]>("receiveImage", (data) =>
            {
                //string img_str = Convert.ToBase64String(data);
                //img_test_string = string.Format("data:{0};base64,{1}", "img/bmp", img_str);
                //Console.WriteLine("Image: {0}", img_test_string);
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
