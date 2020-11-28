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
            Console.WriteLine("Sending map!");
            await Clients.Caller.SendAsync("receiveMap", Map.current);
            Console.WriteLine("Map sent!");
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
    }
}
