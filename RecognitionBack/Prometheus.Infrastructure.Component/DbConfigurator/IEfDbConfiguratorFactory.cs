using Prometheus.Infrastructure.Component;

namespace Prometheus.Infrastructure.Component.DbConfigurator
{
    public interface IEfDbConfiguratorFactory<TComponentOptions> where TComponentOptions : ComponentOptions<TComponentOptions>
    {
        IEfDbConfigurator GetDbConfigurator();
    }
}
