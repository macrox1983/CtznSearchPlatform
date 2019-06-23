using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Prometheus.Infrastructure.Component.DbConfigurator;
using Prometheus.Infrastructure.Component.DbContext;
using System;

namespace Prometheus.Infrastructure.Component
{
    public sealed class ComponentBuilder : IComponentBuilder
    {
        private readonly ContainerBuilder _builder;
        private bool _optionsRegistred;
        private bool _dbOptionsRegistred;
        private bool _dbContextRegistred;

        public ComponentBuilder(ContainerBuilder builder)
        {
            _builder = builder;
        }

        public IComponentBuilder RegisterOptions<TComponentOptions, TConfigureOptions>() 
            where TComponentOptions : ComponentOptions<TComponentOptions> 
            where TConfigureOptions : ComponentConfigureOptions<TComponentOptions>
        {
            if(!_optionsRegistred)
            {
                _builder.RegisterType<TConfigureOptions>().As<IConfigureOptions<TComponentOptions>>().SingleInstance();
                _builder.Register(context =>
                {
                    var options = Activator.CreateInstance<TComponentOptions>();
                    var config = context.Resolve<IConfigureOptions<TComponentOptions>>();
                    config.Configure(options);
                    return options;
                }).SingleInstance();
                _builder.RegisterType<EfDbConfiguratorFactory<TComponentOptions>>().As<IEfDbConfiguratorFactory<TComponentOptions>>();
                _optionsRegistred = true;
            }
            return this;
        }

        public IComponentBuilder RegisterDependencies(Action<ContainerBuilder> registerAction)
        {
            registerAction?.Invoke(_builder);
            return this;
        }

        public IComponentBuilder RegisterOptions<TComponentOptions>() where TComponentOptions : ComponentOptions<TComponentOptions>
        {
            RegisterOptions<TComponentOptions, ComponentConfigureOptions<TComponentOptions>>();
            return this;
        }

        private void RegisterDbOptions<TComponentOptions, TDbContext>() 
            where TDbContext : ComponentDbContext<TDbContext>, IComponentDbContext 
            where TComponentOptions:ComponentOptions<TComponentOptions>
        {
            if (!_optionsRegistred)
                throw new Exception("Необходимо зарегистрировать в контайнер опции компонента!!!");
            if(!_dbOptionsRegistred)
            {
                _builder.Register(context =>
                {
                    var dbConfigurator = context.Resolve<IEfDbConfiguratorFactory<TComponentOptions>>().GetDbConfigurator();

                    var dbContextOptions = new DbContextOptions<TDbContext>();
                    var dbContextOptionsBuilder = new DbContextOptionsBuilder(dbContextOptions);
                    dbConfigurator.ConfigureDatabase(dbContextOptionsBuilder);
                    return (ComponentDbContextOptions<TDbContext>)Activator.CreateInstance(typeof(ComponentDbContextOptions<TDbContext>), dbContextOptionsBuilder.Options);
                });
                _dbOptionsRegistred = true;
            }            
        }

        private void RegisterDbContext<TDbContext>(bool asImplementedInterfaces)
        {
            if(!_dbContextRegistred)
            {
                var regBuilder = _builder.RegisterType<TDbContext>();
                if (asImplementedInterfaces)
                    regBuilder.AsImplementedInterfaces();
                _dbContextRegistred = true;
            }
        }

        public IComponentBuilder RegisterDbContext<TComponentOptions, TDbContext>() 
            where TDbContext : ComponentDbContext<TDbContext>, IComponentDbContext 
            where TComponentOptions:ComponentOptions<TComponentOptions>
        {
            RegisterDbOptions<TComponentOptions,TDbContext>();
            RegisterDbContext<TDbContext>(false);
            _builder.RegisterType<DbContextFactory<TDbContext>>().As<IComponentDbContextFactory<TDbContext>>();
            return this;
        }

        public IComponentBuilder RegisterDbContextAsImplementedInterfaces<TComponentDbOptions, TDbContext>()
            where TDbContext : ComponentDbContext<TDbContext>, IComponentDbContext
            where TComponentDbOptions : ComponentOptions<TComponentDbOptions>
        {

            RegisterDbOptions<TComponentDbOptions, TDbContext>();
            RegisterDbContext<TDbContext>(true);
            
            _builder.RegisterGeneric(typeof(DbContextFactory<>)).AsImplementedInterfaces();
            return this;
        }

        public IComponentBuilder RegisterHostedService<THostedService, TComponent>() where THostedService : IComponentHostedService
        {
            _builder.RegisterType<THostedService>().AsSelf().As<IComponentHostedService>().As<IHostedService>().Named<IComponentHostedService>(typeof(TComponent).Name).SingleInstance();
            return this;
        }
    }
}
