using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.Infrastructure.Component
{
    public abstract class ComponentHostedService : IComponentHostedService
    {
        public string ComponentName { get; set; }

        public abstract string GetState();

        public abstract Task PauseAsync();

        public abstract string ServiceVersion();

        public abstract Task StartAsync(CancellationToken cancellationToken);

        public abstract Task StopAsync(CancellationToken cancellationToken);
    }
}
