using System;
//using System.Collections.Generic;
//using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using dfe.Shared;
using System.IO;

namespace dfe.Server.Hubs
{
    public class MapHub : Hub
    {
        public async Task getMap()
        {
            await Clients.Caller.SendAsync("receiveMap", Map.current);
            Map.current.loadTextures();
            Map.current.loadSprites();
        }

        public async Task getImage(string image_name)
        {
            Stream image_fs = new FileStream(image_name, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] image_bytes = new byte[image_fs.Length];
            image_fs.Read(image_bytes);
            //string image_str = System.Text.Encoding.UTF8.GetString(image_bytes);
            //Console.WriteLine("Image: {0}", image_str);
            await Clients.Caller.SendAsync("receiveImage", image_bytes);
            
        }

        public async Task getSprite(int sprite_id)
        {
            byte[] sprite_bytes;
            if (Map.current.sprites[sprite_id].pb_data != null)
            {
                sprite_bytes = Map.current.sprites[sprite_id].pb_data.pixels;
            }
            else
            {
                sprite_bytes = new byte[0];
            }
            await Clients.Caller.SendAsync("receiveSprite", sprite_id, sprite_bytes);
        }

        public async Task getTexture(int texture_id)
        {
            byte[] texture_bytes;
            if (Map.current.textures[texture_id].pb_data != null)
            {
                texture_bytes = Map.current.textures[texture_id].pb_data.pixels;
            }
            else
            {
                texture_bytes = new byte[0];
            }
            await Clients.Caller.SendAsync("receiveTexture", texture_id, texture_bytes);
        }
    }
}
