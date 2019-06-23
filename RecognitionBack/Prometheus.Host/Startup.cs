using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus.Host.Component;
using Microsoft.AspNetCore.Mvc;
using Autofac.Core;
using Prometheus.Infrastructure;
using Prometheus.Infrastructure.Component;
using Prometheus.Infrastructure.Host;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Prometheus.Host.Controllers;
using System.Reflection;

namespace Prometheus.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.Configure<List<ComponentConfiguration>>(Configuration.GetSection("Components"));
            services.Configure<PrometheusHostOptions>(Configuration.GetSection("HostOptions"));

            #region jwt
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //        .AddJwtBearer(options =>
            //        {
            //            options.RequireHttpsMetadata = true;                        
            //            options.TokenValidationParameters = new TokenValidationParameters
            //            {
            //                // укзывает, будет ли валидироваться издатель при валидации токена
            //                ValidateIssuer = true,
            //                // строка, представляющая издателя
            //                ValidIssuer = AuthOptions.ISSUER,

            //                // будет ли валидироваться потребитель токена
            //                ValidateAudience = true,
            //                // установка потребителя токена
            //                ValidAudience = AuthOptions.AUDIENCE,
            //                // будет ли валидироваться время существования
            //                ValidateLifetime = true,

            //                // установка ключа безопасности
            //                IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            //                // валидация ключа безопасности
            //                ValidateIssuerSigningKey = true,
            //            };
            //            //options.LoginPath = new PathString("/Auth/Login");
            //            //options.AccessDeniedPath = new PathString("/Auth/Login");
            //        });

            //services.AddAuthorization(options =>
            //{
            //    options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)                
            //    .RequireAuthenticatedUser()
            //    .Build();
            //}); 
            #endregion

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<PrometheusHostComponent>();
            containerBuilder.Populate(services);            
            var container = containerBuilder.Build();

            #region загрузка компонентов и их зависимостей в память
            var componentLoader = container.Resolve<IComponentLoader>();

            containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<PrometheusHostComponent>();
            var assemblies = componentLoader.LoadComponents();
            // регистрация используемых типов из компонентов в глобальный контейнер
            containerBuilder.RegisterAssemblyModules(assemblies.ToArray());
            containerBuilder.RegisterInstance(componentLoader).AsImplementedInterfaces().SingleInstance();
            var mvcBuilder = services.AddMvc();
            // добавление API контроллеров из модулей в глобальный список
            assemblies.Add(Assembly.GetEntryAssembly());
            Array.ForEach(assemblies.ToArray(), assembly => mvcBuilder.AddApplicationPart(assembly));
            #endregion
            mvcBuilder
                .AddControllersAsServices()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            containerBuilder.Populate(services);
            container = containerBuilder.Build();

            return new AutofacServiceProvider(container);
        }        

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            //app.UseAuthentication();
            //app.UseHttpsRedirection();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}");
            });            
        }
    }
}
