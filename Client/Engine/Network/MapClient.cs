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
            map_hub_conn = new_hub_conn;

            map_hub_conn.On<Map>("receiveMap", (data) =>
            {
                Console.WriteLine("Received map!");
                level_map = new Map(data);
                Console.WriteLine("Textures?");
                Console.WriteLine("Textures: {0}", level_map.textures.Length);
                foreach (texture tex in level_map.textures)
                {
                    Console.WriteLine("Texture: {0}", tex.id);
                    if (tex.pb_data != null)
                    {
                        Console.WriteLine("Pixels: {0}", tex.pb_data.pixels.Length);
                        foreach (byte byt in tex.pb_data.pixels)
                        {
                            Console.Write("{0}, ");
                        }
                    }
                }
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
