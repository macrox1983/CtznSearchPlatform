using Microsoft.Extensions.Options;
using Prometheus.Infrastructure;
using Prometheus.Infrastructure.Component;
using Prometheus.Infrastructure.Host;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Prometheus.ComponentLoader
{
    internal class ComponentLoader: IComponentLoader
    {
        private List<Assembly> _assembliesForResolve;

        public ComponentLoader(IOptionsMonitor<PrometheusHostOptions> hostConfiguration, IOptionsMonitor<List<ComponentConfiguration>> componentsConfiguration)
        {
            _assembliesForResolve = new List<Assembly>();
            ComponentsConfiguration = componentsConfiguration.CurrentValue;
            HostConfiguration = hostConfiguration.CurrentValue;
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
        }

        public List<ComponentConfiguration> ComponentsConfiguration { get; }

        public PrometheusHostOptions HostConfiguration { get; }

        public List<Assembly> LoadComponents()
        {
            var componentsFolder = string.IsNullOrWhiteSpace(HostConfiguration.ComponentsFolder) ?
                $"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}\\Components" :
                HostConfiguration.ComponentsFolder.StartsWith('.') && !HostConfiguration.ComponentsFolder.Contains(':')?
                $"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{HostConfiguration.ComponentsFolder}{Path.DirectorySeparatorChar}":
                $"{HostConfiguration.ComponentsFolder}{Path.DirectorySeparatorChar}";
            var componentAssemblies = ComponentsConfiguration.Select(m =>
            {
                //
                var assemblies = m.Dependencies.Select(d => Assembly.LoadFile($"{ componentsFolder }{d.AssemblyName}.dll")).ToList();

                var componentAssembly = Assembly.LoadFile($"{ componentsFolder }{m.AssemblyName}.dll");
                assemblies.Add(componentAssembly);
                _assembliesForResolve.AddRange(assemblies);
                return componentAssembly;
            }).ToList();
            return componentAssemblies;
        }

        public Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return _assembliesForResolve.FirstOrDefault(a => a.FullName.Contains(args.Name));
        }

        public Type GetTypeFromComponent(string typeName)
        {
            var split = typeName.Split(", ");
            var assemblyName = split[1];
            var shortTypeName = split[0];
            var assembly = AssemblyResolve(this, new ResolveEventArgs(assemblyName));
            var type = assembly.GetType(shortTypeName);
            return type;
        }
    }
}
