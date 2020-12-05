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
        public static MapClient map_client;

        public HubConnection map_hub_conn;

        // TODO: Refactor this out
        public Map level_map;

        public MapClient(HubConnection new_hub_conn)
        {
            map_hub_conn = new_hub_conn;

            map_hub_conn.On<Map>("receiveMap", (data) =>
            {
                level_map = new Map(data);

                GameClient.game_state.setMap(level_map);

                foreach (Texture tex in level_map.textures)
                {
                    map_hub_conn.SendAsync("getTexture", tex.id);
                }

                foreach (sprite spr in level_map.sprites)
                {
                    map_hub_conn.SendAsync("getSprite", spr.id);
                }
            });

            map_hub_conn.On<int, byte[]>("receiveSprite", (sprite_id, sprite_bytes) =>
            {
                level_map.sprites[sprite_id].pb_data = new PixelBuffer(16, 16);
                level_map.sprites[sprite_id].pb_data.pixels = sprite_bytes;
                Console.WriteLine("Received sprite!");
                // TODO: initMap should only be called once, after all sprites and textures have been received
                level_map.initMap();
            });

            map_hub_conn.On<int, byte[]>("receiveTexture", (texture_id, texture_bytes) =>
            {
                level_map.textures[texture_id].pixelBuffer = new PixelBuffer(64, 64);

                byte[] resized = new byte[64 * 64 * 4];
                // Covert to 4 bytes per pixel
                int src = 0;
                int dst = 0;
                for (int i = 0; i < 64 * 64; i++)
                {
                    resized[dst] = texture_bytes[src];
                    resized[dst + 1] = texture_bytes[src + 1];
                    resized[dst + 2] = texture_bytes[src + 2];
                    resized[dst + 3] = 255;
                    src += 3;
                    dst += 4;
                }

                level_map.textures[texture_id].pixelBuffer.pixels = resized;

                level_map.initMap();
            });

            map_hub_conn.StartAsync();

            map_client = this;

        }
        
        public void loadMapFromServer()
        {

            map_hub_conn.SendAsync("getMap");

            map_hub_conn.SendAsync("getImage", "Assets/Sprites/Jonesy.data");
        }
    }
}
