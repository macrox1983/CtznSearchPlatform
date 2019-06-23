using Prometheus.Infrastructure.Component;
using Prometheus.Infrastructure.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Prometheus.Configuration
{
    internal class PrometheusConfigurator : IPrometheusConfigurator
    {
        private readonly DbConfiguration _dbConfiguration;
        private readonly IComponentManager _componentManager;

        public PrometheusConfigurator(DbConfiguration dbConfiguration, IComponentManager componentManager)
        {
            _componentManager = componentManager ?? throw new ArgumentNullException(nameof(componentManager));
            _dbConfiguration = dbConfiguration ?? throw new ArgumentNullException(nameof(dbConfiguration));
        }   

        public IPrometheusDbConfigurator GetDbConfigurator()
        {
            Type dbConfigurationType = Type.GetType(_dbConfiguration.DbConfigurator, assemblyName =>
            {
                return _componentManager.AssemblyResolve(this, new ResolveEventArgs(assemblyName.Name));
            }, (assembly, typeName, flag) =>
            {
                return assembly.GetType(typeName);
            });

            //TODO: костыли, надо переделать как-нибудь
            if (dbConfigurationType.GetInterfaces().Contains(typeof(IPrometheusDbConfiguratorWithMigration)))
                return (IPrometheusDbConfigurator)Activator.CreateInstance(dbConfigurationType, _dbConfiguration.ConnectionString, _dbConfiguration.MigrationAssembly);
            return (IPrometheusDbConfigurator)Activator.CreateInstance(dbConfigurationType, _dbConfiguration.ConnectionString);
        }

        private Assembly PrometheusDBContextFactory_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = args.Name.Split(", ")[0];
            var path = $"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{assemblyName}.dll";
            if (File.Exists(path))
            {
                var assembly = Assembly.LoadFile(path);
                return assembly;
            }
            return null;
        }
    }
}
