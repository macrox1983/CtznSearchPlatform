using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Prometheus.Infrastructure.Component
{

    public interface IComponent
    {
        string ComponentName { get; }

        string ComponentDescription { get; }

        Assembly ComponentAssembly { get; set; }

        List<IComponentHostedService> HostedServices { get; set; }

        string GetComponentState();

        Task Start();

        Task Pause();

        Task Stop();
    }
}
