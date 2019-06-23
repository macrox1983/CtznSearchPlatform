using System;
using System.Collections.Generic;
using System.Reflection;

namespace Prometheus.Infrastructure.Component
{
    public interface IComponentLoader:IComponentAssemblyResolver
    {
        List<Assembly> LoadComponents();        

        Type GetTypeFromComponent(string typeName);
    }
}
