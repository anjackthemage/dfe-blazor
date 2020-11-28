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
                level_map = new Map(data);
                foreach (texture tex in level_map.textures)
                {
                    map_hub_conn.SendAsync("getTexture", tex.id);
                }

                foreach (sprite spr in level_map.sprites)
                {
                    map_hub_conn.SendAsync("getSprite", spr.id);
                }
            });

            map_hub_conn.On<byte[]>("receiveImage", (data) =>
            {
                ray_tracer.s_tex.pixels = data;

            });

            map_hub_conn.On<int, byte[]>("receiveSprite", (sprite_id, sprite_bytes) =>
            {
                level_map.sprites[sprite_id].pb_data.pixels = sprite_bytes;
            });

            map_hub_conn.On<int, byte[]>("receiveTexture", (texture_id, texture_bytes) =>
            {
                level_map.textures[texture_id].pb_data.pixels = texture_bytes;
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
