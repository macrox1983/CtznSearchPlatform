using Microsoft.Extensions.Options;
using Prometheus.Infrastructure;
using Prometheus.Infrastructure.Component;
using Prometheus.Infrastructure.Configuration;

namespace Prometheus.Configuration
{
    internal class PrometheusConfiguratorWithMigration : PrometheusConfigurator, IPrometheusConfiguratorWithDbMigration, IPrometheusConfigurator
    {
        public PrometheusConfiguratorWithMigration(DbConfiguration dbConfiguration, IComponentManager componentManager) : base(dbConfiguration, componentManager)
        {
        }

        public IPrometheusDbConfiguratorWithMigration GetDbMigrationConfigurator()
        {
            return (IPrometheusDbConfiguratorWithMigration)GetDbConfigurator();
        }
    }
}
