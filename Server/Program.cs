using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
using dfe.Server.Engine;
using dfe.Server.Hubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
//using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;

namespace dfe.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // This is how we get a reference to a hub object, for use on the server side
            var hub_context = host.Services.GetService(typeof(IHubContext<PlayerHub>));
            GameServer.server = new GameServer();
            GameServer.server.p_hub_ref = hub_context as IHubContext<PlayerHub>;

            host.Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
