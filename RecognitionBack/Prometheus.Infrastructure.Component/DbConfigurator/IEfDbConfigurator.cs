using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Prometheus.Infrastructure.Component.DbConfigurator
{
    public interface IEfDbConfigurator
    {
        void ConfigureDatabase(DbContextOptionsBuilder builder);

        void ConfigureDatabaseMigration(MigrationBuilder builder);

        void ConfigureDatabaseModelCreating(ModelBuilder builder);

        Type GetSpecifiedMigrationSqlGeneratorType();
    }
}
