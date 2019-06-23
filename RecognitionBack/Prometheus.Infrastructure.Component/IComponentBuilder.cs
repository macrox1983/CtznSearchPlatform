using Autofac;
using Prometheus.Infrastructure.Component.DbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Infrastructure.Component
{
    public interface IComponentBuilder
    {
        IComponentBuilder RegisterOptions<TOptions, TConfigureOptions>() where TOptions : ComponentOptions<TOptions> where TConfigureOptions: ComponentConfigureOptions<TOptions>;

        IComponentBuilder RegisterOptions<TOptions>() where TOptions : ComponentOptions<TOptions>;

        IComponentBuilder RegisterDependencies(Action<ContainerBuilder> registerAction);

        IComponentBuilder RegisterDbContext<TComponentOptions, TDbContext>()
            where TDbContext : ComponentDbContext<TDbContext>, IComponentDbContext
            where TComponentOptions : ComponentOptions<TComponentOptions>;

        IComponentBuilder RegisterDbContextAsImplementedInterfaces<TComponentDbOptions, TDbContext>()
            where TDbContext : ComponentDbContext<TDbContext>, IComponentDbContext
            where TComponentDbOptions : ComponentOptions<TComponentDbOptions>;

        IComponentBuilder RegisterHostedService<THostedService, TComponent>() where THostedService : IComponentHostedService;
    }
}
