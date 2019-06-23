using Prometheus.Infrastructure.Component;
using System;

namespace Prometheus.Infrastructure.DbConfigurator
{
    public class EfDbConfiguratorFactory<TOptions> : IEfDbConfiguratorFactory<TOptions> where TOptions : ComponentOptions<TOptions>
    {
        private readonly TOptions _componentOptions;
        private readonly IComponentAssemblyResolver _componentAssemblyResolver;

        public EfDbConfiguratorFactory(TOptions componentOptions, IComponentAssemblyResolver componentAssemblyResolver)
        {
            _componentOptions = componentOptions ?? throw new ArgumentNullException(nameof(componentOptions));
            _componentAssemblyResolver = componentAssemblyResolver ?? throw new ArgumentNullException(nameof(componentAssemblyResolver));
        }

        public IEfDbConfigurator GetDbConfigurator()
        {
            Type dbConfigurationType = Type.GetType(_componentOptions.DbConfiguration.DbConfigurator, assemblyName =>
            {
                return _componentAssemblyResolver.AssemblyResolve(this, new ResolveEventArgs(assemblyName.Name));
            },
            (assembly, typeName, flag) =>
            {
                return assembly.GetType(typeName);
            });

            return (IEfDbConfigurator)Activator.CreateInstance(dbConfigurationType, _componentOptions.DbConfiguration.ConnectionString, _componentOptions.DbConfiguration.MigrationAssembly);
        }
    }
}
