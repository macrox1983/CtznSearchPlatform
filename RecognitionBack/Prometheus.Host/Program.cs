using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Prometheus.Infrastructure.Component.DbMigration;

namespace Prometheus.Host
{
    class Program
    {
        public static void Main(string[] args)
        {
            var task = MainAsync(args);
            task.Wait();
        }

        public static async Task MainAsync(string[] args)
        {
            var webHost = await BuildWebHostBuilderAsync(args);
            await webHost.RunAsync();
        }

        public static async Task<IWebHost> BuildWebHostBuilderAsync(string[] args)
        {
            var webHost = WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseContentRoot(PlatformServices.Default.Application.ApplicationBasePath)
                .UseUrls("http://127.0.0.1:5012;")
                .ConfigureAppConfiguration((hostingContext, config) =>
                {                    
                    config.AddJsonFile("appsettings.json",
                                        optional: false,        // File is not optional.
                                        reloadOnChange: true);
                }).ConfigureLogging((hostingContext, logging) => 
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventSourceLogger();
                })
                //.ConfigureKestrel(options =>
                //{
                //    options.Listen(new IPEndPoint(IPAddress.Any, 5012), loptions =>
                //    {
                //        //loptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
                //    });
                //    //options.ListenAnyIP(5000, loptions =>
                //    //{                        
                //    //    loptions.IPEndPoint = new IPEndPoint(IPAddress.Any, 5000);
                //    //});
                //})
                //.UseSetting("https_port", "443")
                .UseStartup<Startup>()
                .Build();

            //Применяем все миграции на бд

            var migrator = (IDbMigrator)webHost.Services.GetService(typeof(IDbMigrator));
            await migrator?.ApplyMigrations();

            return webHost;
        }
    }
}
