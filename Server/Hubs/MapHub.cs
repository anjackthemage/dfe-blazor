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
            byte[] texture_bytes = null;

            string file_path = "Assets/Sprites/" + Map.current.textures[sprite_id].file + ".json";

            if (File.Exists(file_path))
            {
                texture_bytes = Map.current.loadImage(file_path);
            }

            await Clients.Caller.SendAsync("receiveSprite", sprite_id, texture_bytes);
        }

        public async Task getTexture(int texture_id)
        {
            byte[] texture_bytes = null;

            string file_path = "Assets/Textures/" + Map.current.textures[texture_id].file + ".json";

            if (File.Exists(file_path))
            {
                texture_bytes = Map.current.loadImage(file_path);
            }

            await Clients.Caller.SendAsync("receiveTexture", texture_id, texture_bytes);
        }
    }
}
