using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.EntityFrameworkCore.Extensions;
using Prometheus.Infrastructure.Component.Configuration;
using Prometheus.Infrastructure.Component.DbConfigurator;

namespace Prometheus.DbConfigurator.MySQL
{
    public class MySQLDbConfigurator : EfDbConfigurator
    {
        public MySQLDbConfigurator(DbConfiguration dbConfiguration) : base(dbConfiguration)
        {
        }

        public override void AddServicesForSpecifiedDb(ServiceCollection services)
        {
            services.AddEntityFrameworkMySQL();
        }

        public override void ConfigureDb(DbContextOptionsBuilder builder)
        {
            builder.UseMySQL(DbConfiguration.ConnectionString, options => options.MigrationsAssembly(DbConfiguration.MigrationAssembly));
        }

        public override void ConfigureDbMigration(MigrationBuilder builder)
        {
            //new MySqlMigrationsSqlGenerator
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
