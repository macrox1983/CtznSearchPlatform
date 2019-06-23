namespace Prometheus.Infrastructure.Configuration
{
    public interface IPrometheusConfiguratorWithDbMigration
    {
        IPrometheusDbConfiguratorWithMigration GetDbMigrationConfigurator();
    }
}
