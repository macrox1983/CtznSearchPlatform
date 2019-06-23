using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Haka.CtznSearch.Front
{
    public class Program
    {
        private static IWebHost host;
      

        public static void Main(string[] args)
        {
          //  System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        
            host = BuildWebHost(args);
            host.Run();

         
        }


        public static IWebHost BuildWebHost(string[] args) =>
          WebHost.CreateDefaultBuilder(args)
         .UseConfiguration(new ConfigurationBuilder()
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("hosting.json", optional: true)
       .Build())
         .UseKestrel(c => c.AddServerHeader = false)
         .UseStartup<Startup>()
         .Build();
    }
}
