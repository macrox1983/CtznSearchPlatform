using Microsoft.Extensions.Logging;
using Prometheus.Infrastructure.Component;
using System;
using System.IO;
using System.Reflection;

namespace Prometheus.Infrastructure.Component.DbConfigurator
{
    public class DefaultAssemblyResolver : IComponentAssemblyResolver
    {
        public DefaultAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
        }

        public Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var names = args.Name.Split(", ");
            var dllFilename = $"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{names[0]}.dll"; 
            if (File.Exists(dllFilename))
                return Assembly.LoadFile(dllFilename);
            return null;
        }
    }

    public class EfDbConfiguratorFactory<TOptions> : IEfDbConfiguratorFactory<TOptions> where TOptions : ComponentOptions<TOptions>
    {
        private readonly TOptions _componentOptions;
        private readonly IComponentAssemblyResolver _componentAssemblyResolver;
        private readonly ILogger _logger;

        public EfDbConfiguratorFactory(TOptions componentOptions, ILoggerFactory loggerFactory, IComponentAssemblyResolver componentAssemblyResolver=null)
        {
            _componentOptions = componentOptions ?? throw new ArgumentNullException(nameof(componentOptions));
            _componentAssemblyResolver = componentAssemblyResolver ?? new DefaultAssemblyResolver();
            _logger = loggerFactory.CreateLogger(nameof(EfDbConfiguratorFactory<TOptions>));
        }

        public IEfDbConfigurator GetDbConfigurator()
        {
            try
            {
                Type dbConfigurationType = Type.GetType(_componentOptions.DbConfiguration.DbConfigurator, assemblyName =>
                    {
                        var assembly = _componentAssemblyResolver.AssemblyResolve(this, new ResolveEventArgs(assemblyName.Name));
                        if (assembly == null)
                            assembly = new DefaultAssemblyResolver().AssemblyResolve(this, new ResolveEventArgs(assemblyName.Name));
                        return assembly;
                    },
                    (assembly, typeName, flag) =>
                    {
                        return assembly.GetType(typeName);
                    });

                return (IEfDbConfigurator)Activator.CreateInstance(dbConfigurationType, _componentOptions.DbConfiguration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка загрузки библиотеки {_componentOptions.DbConfiguration.DbConfigurator} конфигуратора бд");
            }
            return null;
        }
    }
}
