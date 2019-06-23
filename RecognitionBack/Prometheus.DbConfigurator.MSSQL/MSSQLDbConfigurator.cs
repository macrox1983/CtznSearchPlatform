using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Prometheus.Infrastructure.Component.Configuration;
using Prometheus.Infrastructure.Component.DbConfigurator;
using System;

namespace Prometheus.DbConfigurator.MSSQL
{
    public class MSSQLDbConfigurator : EfDbConfigurator
    {
        public MSSQLDbConfigurator(DbConfiguration dbConfiguration) : base(dbConfiguration)
        {
        }

        public override void AddServicesForSpecifiedDb(ServiceCollection services)
        {
            services.AddEntityFrameworkSqlServer();
        }

        public override void ConfigureDb(DbContextOptionsBuilder builder)
        {
            builder.UseSqlServer(DbConfiguration.ConnectionString, options => options.MigrationsAssembly(DbConfiguration.MigrationAssembly));
        }

        public override void ConfigureDbMigration(MigrationBuilder builder)
        {            
        }

        public override void ConfigureDbModelCreating(ModelBuilder builder)
        {
            
        }

        public override Type GetSpecifiedMigrationsSqlGeneratorType()
        {
            return typeof(SqlServerMigrationsSqlGenerator);
        }
    }
}
