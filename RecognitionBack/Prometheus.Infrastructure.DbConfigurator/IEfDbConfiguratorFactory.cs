using Prometheus.Infrastructure.Component;

namespace Prometheus.Infrastructure.DbConfigurator
{
    public interface IEfDbConfiguratorFactory<TOptions> where TOptions : ComponentOptions<TOptions>
    {
        IEfDbConfigurator GetDbConfigurator();
    }
}
