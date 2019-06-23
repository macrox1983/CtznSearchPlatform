using Microsoft.EntityFrameworkCore.Migrations;

namespace Prometheus.Infrastructure.Configuration
{
    public interface IPrometheusDbConfiguratorWithMigration
    {
        string MigrationAssembly { get; }

        void ConfigureMigration(MigrationBuilder builder);
    }
}
