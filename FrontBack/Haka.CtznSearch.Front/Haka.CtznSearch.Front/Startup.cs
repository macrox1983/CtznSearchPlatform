using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Haka.CtznSearch.Front.Models;


using System.Text.Encodings.Web;
using System;
using System.IO;
using System.Text.Unicode;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.WebEncoders;
using BreakingStorm.Common.Loggers;
using BreakingStorm.Common.Helpers;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Caching.Memory;


namespace Haka.CtznSearch.Front
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<WebEncoderOptions>(options => { options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All); });
            services.AddDbContext<DBContext>();

            services.AddMvc();

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.Name = "session";
                options.IdleTimeout = TimeSpan.FromSeconds(3600);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute("fastInfo", "i/{ticketId}", new { controller = "main", action = "ticketInfo" });
                routes.MapRoute("ajaxapi", "ajaxapi/{action}", new { controller = "ajaxapi", action = "index" });
                routes.MapRoute("default", "{action}/{id?}", new { controller = "main", action = "index" });
                
            });
            /*

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller}/{action}/{id?}", new { controller = "search", action = "index" });
                routes.MapRoute("default2", "{geocode}", new { controller = "search", action = "geocode" });
            });*/
        }
    }
}
