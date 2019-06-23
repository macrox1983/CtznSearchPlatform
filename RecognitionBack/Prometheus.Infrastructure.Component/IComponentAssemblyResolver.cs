using System;
using System.Reflection;

namespace Prometheus.Infrastructure.Component
{
    public interface IComponentAssemblyResolver
    {
        Assembly AssemblyResolve(object sender, ResolveEventArgs args);
    }
}
