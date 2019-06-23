using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prometheus.Infrastructure.Component
{
    public interface IComponentHostedService : IHostedService
    {
        string ComponentName { get; set; }

        string ServiceVersion();

        string GetState();

        Task PauseAsync();
    }
}
