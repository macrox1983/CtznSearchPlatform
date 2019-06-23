using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Prometheus.Infrastructure.DbConfigurator
{
    public interface IEfDbConfigurator
    {
        void ConfigureDatabase(DbContextOptionsBuilder builder);

        void ConfigureDatabaseMigration(MigrationBuilder builder);

        void CongigureDatabaseModelCreating(ModelBuilder builder);
    }
}
