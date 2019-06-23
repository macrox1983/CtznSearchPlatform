using Autofac;
using Microsoft.EntityFrameworkCore;
using Prometheus.ComponentLoader.DI;
using Prometheus.DAL.DI;
using Prometheus.DbContext;
using Prometheus.DbContext.DI;
using Prometheus.DbMirgator.DI;
using Prometheus.Infrastructure.Component;
using Prometheus.Infrastructure.Component.DbConfigurator;
using Prometheus.Infrastructure.Component.DbContext;
using Prometheus.Infrastructure.DbContextBase;
using Prometheus.Infrastructure.Host;
using System;

namespace Prometheus.Host.Component
{
    public class PrometheusHostComponent : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {           
            var componentBuilder = new ComponentBuilder(builder);
            componentBuilder.RegisterOptions<PrometheusHostOptions>()
                .RegisterDbContextAsImplementedInterfaces<PrometheusHostOptions, PrometheusDbContext>()
                .RegisterDependencies(containerBuilder=> 
                {
                    containerBuilder.RegisterModule<DbMigratorModule>();
                    containerBuilder.RegisterModule<ComponentManagerModule>();
                    containerBuilder.RegisterModule<PrometheusDALModule>();
                });
        }
    }
}
