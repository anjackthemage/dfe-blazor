using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using dfe.Shared;
using dfe.Shared.Entities;
using dfe.Shared.Render;

namespace dfe.Server.Engine
{
    /// <summary>
    /// Container class for a game Zone
    /// </summary>
    public class Zone
    {
        #region init
        public Guid guid;
        public Dictionary<Guid, Player> local_players;
        public Dictionary<Guid, Mob> local_mobs;
        public Dictionary<Guid, Entity> local_props;
        public Map map;

        public Zone() {
            guid = Guid.NewGuid();
            local_players = new Dictionary<Guid, Player>();
            local_mobs = new Dictionary<Guid, Mob>();
            local_props = new Dictionary<Guid, Entity>();

            // Default zone settings.
            // Generate a template map.
            map = new Map("level_test");
            initZone();
        }

        public void initZone()
        {
            Console.WriteLine("Initializing zone: {0}", this.guid);
            loadAssetsFromFile("Assets/texture_directory.json", typeof(Texture));
            loadAssetsFromFile("Assets/sprite_directory.json", typeof(sprite));

            Console.WriteLine("Sprite 1 pixels: {0}", local_sprites[1].pb_data.pixels.Length);
        }
        #endregion

        #region simulation
        /// <summary>
        /// Function to handle processing non-render calculations for each object in this zone.
        /// </summary>
        public async Task simulate()
        {
            // Cycle through all the mobs and ents in the map
            //      process movement
            //      process ai
            //      process environmental effects?
            //Console.WriteLine("Simulating zone: {0}", this.guid);

            // Check for updated entities
            
        }
        #endregion

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
            if(File.Exists(file_path))
            {
                string json_str = File.ReadAllText(file_path);

                if(asset_type == typeof(sprite))
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
