using dfe.Shared;
using dfe.Shared.Render;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dfe.Server.Services;
using dfe.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

namespace dfe.Server.Engine
{
    public class GameServer
    {

        public static GameServer server;

        public IHubContext<PlayerHub> p_hub_ref;

        public static bool b_is_running { get; private set; }

        public Dictionary<int, Zone> world;
        public Dictionary<int, TextureDef> texture_assets;
        public Dictionary<int, SpriteDef> sprite_assets;

        public GameServer()
        {
            
            world = new Dictionary<int, Zone>();
            texture_assets = new Dictionary<int, TextureDef>();
            sprite_assets = new Dictionary<int, SpriteDef>();

            Zone zone = new Zone();
            world.Add(0, zone);

            server = this;
            b_is_running = true;


            loadAssetsFromFile("Assets/texture_directory.json", typeof(TextureDef));
            loadAssetsFromFile("Assets/sprite_directory.json", typeof(SpriteDef));

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
        public SpriteDef[] local_sprites;
        public TextureDef[] local_textures;

        /// <summary>
        /// Loads an image binary from the file system.
        /// </summary>
        /// <param name="file_path">Name of the file to load.</param>
        /// <returns>byte[] of raw image file data</returns>
        public byte[] loadImage(string file_path)
        {
            Stream image_fs = new FileStream(file_path, FileMode.Open, FileAccess.Read, FileShare.Read);

            Bitmap bmp = (Bitmap)Image.FromStream(image_fs);
            byte[] pixels = new byte[bmp.Width * bmp.Height * 4];
            int i = 0;
            Color c;
            for(int y = 0; y < bmp.Height; y++)
                for(int x = 0; x < bmp.Width; x++)
                {
                    c = bmp.GetPixel(x, y);
                    pixels[i] = c.R;
                    pixels[i + 1] = c.G;
                    pixels[i + 2] = c.B;
                    pixels[i + 3] = c.A;
                    i += 4;
                }
            Console.WriteLine("Bytes Read : " + pixels.Length);
            MemoryStream memStream = new MemoryStream();
            GZipStream gzip = new GZipStream(memStream, CompressionMode.Compress);
            gzip.Write(pixels, 0, pixels.Length);
            gzip.Close();
            byte[] output = memStream.ToArray();
            Console.WriteLine("Bytes Out : " + output.Length);

            return output;
        }

        public void loadAssetsFromFile(string file_path, Type asset_type)
        {
            Console.WriteLine("Loading {0} assets from {1}", asset_type.ToString(), file_path);
            if (File.Exists(file_path))
            {
                string json_str = File.ReadAllText(file_path);

                if (asset_type == typeof(SpriteDef))
                {
                    SpriteDef[] s_array = JsonSerializer.Deserialize<SpriteDef[]>(json_str);
                    loadAssets(ref s_array);
                    // Convert to dict
                    sprite_assets = s_array.Select((value, key) => new { value, key }).ToDictionary(element => element.key, element => element.value);
                }
                else if (asset_type == typeof(TextureDef))
                {
                    TextureDef[] t_array = JsonSerializer.Deserialize<TextureDef[]>(json_str);
                    loadAssets(ref t_array);
                    // Convert to dict
                    texture_assets = t_array.Select((value, key) => new { value, key }).ToDictionary(element => element.key, element => element.value);
                }
            }
            else
            {
                Console.WriteLine("File not found: {0}", file_path);
            }
            Console.WriteLine("Converting Sprites : ");
            foreach (SpriteDef spr in sprite_assets.Values)
            {
                Console.WriteLine(spr);
            }
            Console.WriteLine("Loaded Textures : ");
            foreach (TextureDef tex in texture_assets.Values)
            {
                Console.WriteLine(tex);
            }
        }

        public TextureDef[] loadAssets(ref TextureDef[] textures)
        {
            foreach (TextureDef tex in textures)
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

        public SpriteDef[] loadAssets(ref SpriteDef[] sprites)
        {
            foreach (SpriteDef spr in sprites)
            {
                string file_path = "Assets/Sprites/" + spr.file;
                if (File.Exists(file_path))
                {
                    spr.pixelBuffer = new PixelBuffer(16, 16);

                    spr.pixelBuffer.pixels = loadImage(file_path);
                }
            }
            return sprites;
        }
        #endregion
    }
}
