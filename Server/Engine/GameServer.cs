using dfe.Shared;
using dfe.Shared.Render;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dfe.Server.Services;
using dfe.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.IO;
using System.Text.Json;

namespace dfe.Server.Engine
{
    public class GameServer
    {

        public static GameServer server;

        public IHubContext<PlayerHub> p_hub_ref;

        public static bool b_is_running { get; private set; }

        public Dictionary<int, Zone> world;
        public Dictionary<int, Texture> texture_assets;
        public Dictionary<int, sprite> sprite_assets;

        public GameServer()
        {
            world = new Dictionary<int, Zone>();
            texture_assets = new Dictionary<int, Texture>();
            sprite_assets = new Dictionary<int, sprite>();

            Zone zone = new Zone();
            world.Add(0, zone);

            server = this;
            b_is_running = true;


            loadAssetsFromFile("Assets/texture_directory.json", typeof(Texture));
            loadAssetsFromFile("Assets/sprite_directory.json", typeof(sprite));

            doGameLoop();
        }

        private async Task doGameLoop()
        {
            while (b_is_running)
            {
                // This will hold all of the simulate tasks we're about to create
                Task[] zone_sims = new Task[world.Count];

                // Call `simulation` code on each zone in world:
                foreach (KeyValuePair<int, Zone> element in world)
                {
                    Task zone_sim_task = element.Value.simulate();  // assign task
                    zone_sims[element.Key] = zone_sim_task;         // put task in array
                }

                Task.WhenAll(zone_sims); // Wait for all zone sims to complete


                // Initiate heartbeat via PlayerHub
                if (p_hub_ref != null)
                {
                    p_hub_ref.Clients.All.SendAsync("doHeartbeat");
                }

                // Pause execution on this thread to let main thread process for a bit
                await Task.Delay(1000);
            }
        }


        #region asset loading
        public sprite[] local_sprites;
        public Texture[] local_textures;

        public byte[] loadImage(string file_path)
        {
            Stream image_fs = new FileStream(file_path, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] image_bytes = new byte[image_fs.Length];
            image_fs.Read(image_bytes);

            return image_bytes;
        }

        public void loadAssetsFromFile(string file_path, Type asset_type)
        {
            Console.WriteLine("Loading {0} assets from {1}", asset_type.ToString(), file_path);
            if (File.Exists(file_path))
            {
                string json_str = File.ReadAllText(file_path);

                if (asset_type == typeof(sprite))
                {
                    local_sprites = JsonSerializer.Deserialize<sprite[]>(json_str);
                    loadAssets(ref local_sprites);
                }
                else if (asset_type == typeof(Texture))
                {
                    local_textures = JsonSerializer.Deserialize<Texture[]>(json_str);
                    loadAssets(ref local_textures);
                }
            }
            else
            {
                Console.WriteLine("File not found: {0}", file_path);
            }
        }

        public Texture[] loadAssets(ref Texture[] textures)
        {
            foreach (Texture tex in textures)
            {
                string file_path = "Assets/Textures/" + tex.file;
                if (File.Exists(file_path))
                {
                    tex.pixelBuffer = new PixelBuffer(64, 64);

                    tex.pixelBuffer.pixels = loadImage(file_path);
                }
            }
            return textures;
        }

        public sprite[] loadAssets(ref sprite[] sprites)
        {
            foreach (sprite spr in sprites)
            {
                string file_path = "Assets/Sprites/" + spr.file;
                if (File.Exists(file_path))
                {
                    spr.pb_data = new PixelBuffer(16, 16);

                    spr.pb_data.pixels = loadImage(file_path);
                }
            }
            return sprites;
        }
        #endregion
    }
}
