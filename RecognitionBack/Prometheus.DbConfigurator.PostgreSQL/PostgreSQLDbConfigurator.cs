using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Prometheus.Infrastructure.Component.Configuration;
using Prometheus.Infrastructure.Component.DbConfigurator;

namespace Prometheus.DbConfigurator.PostgreSQL
{
    public class PostgreSQLDbConfigurator : EfDbConfigurator
    {
        public PostgreSQLDbConfigurator(DbConfiguration dbConfiguration) : base(dbConfiguration)
        {
        }

        public override void AddServicesForSpecifiedDb(ServiceCollection services)
        {
            services.AddEntityFrameworkNpgsql();
        }

        public override void ConfigureDb(DbContextOptionsBuilder builder)
        {            
            builder.UseNpgsql(DbConfiguration.ConnectionString, options => options.MigrationsAssembly(DbConfiguration.MigrationAssembly));
        }

        public override void ConfigureDbMigration(MigrationBuilder builder)
        {            
        }

        public override void ConfigureDbModelCreating(ModelBuilder builder)
        {
        }

        public override Type GetSpecifiedMigrationsSqlGeneratorType()
        {
            return null;
        }
    }
}
