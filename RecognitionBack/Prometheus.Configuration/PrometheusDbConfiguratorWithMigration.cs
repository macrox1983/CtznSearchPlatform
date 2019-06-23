using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Prometheus.Infrastructure.Configuration;
using Prometheus.Infrastructure.DbMigration;
using Prometheus.MigrationAssembly;
using System;

namespace Prometheus.Configuration
{
    public abstract class PrometheusDbConfiguratorWithMigration : PrometheusDbConfigurator, IPrometheusDbConfiguratorWithMigration
    {
        private readonly string _migrationAssembly;

        public PrometheusDbConfiguratorWithMigration(string connectionString, string migrationAssembly) : base(connectionString)
        {
            _migrationAssembly = migrationAssembly;
        }

        public string MigrationAssembly => _migrationAssembly;

        public override void ConfigureDatabase(DbContextOptionsBuilder builder)
        {
            var sc = new ServiceCollection();
            AddServicesForSpecifiedDatabase(sc);

            //Сделано так для того чтобы в класс миграции можно было инжектировать конфигуратор бд( IPrometheusDbMigrationConfiguration ) с 
            //возможностью конфигурирования миграции взависимости от провайдера бд
            Array.ForEach(this.GetType().GetInterfaces(), i => sc.AddSingleton(i, this));

            var sp = sc.AddScoped(typeof(IMigrationsAssembly), typeof(PrometheusMigrationAssembly))
                .AddScoped(typeof(IMigrationsSqlGenerator), typeof(PrometheusSqlGenerator))
                .BuildServiceProvider();

            base.ConfigureDatabase(builder);

            builder.UseInternalServiceProvider(sp);
        }

        public abstract void ConfigureMigration(MigrationBuilder builder);

    }
}
