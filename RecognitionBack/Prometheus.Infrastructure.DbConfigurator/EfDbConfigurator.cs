using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Prometheus.Infrastructure.DbMigration;
using Prometheus.MigrationAssembly;
using System;

namespace Prometheus.Infrastructure.DbConfigurator
{
    public abstract class EfDbConfigurator : IEfDbConfigurator
    {
        private readonly string _connectionString;
        private readonly string _migrationAssembly;

        public EfDbConfigurator(string connectionString, string migrationAssembly)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _migrationAssembly = migrationAssembly;
        }

        public string ConnectionString => _connectionString;

        public string MigrationAssembly => _migrationAssembly;

        void IEfDbConfigurator.ConfigureDatabase(DbContextOptionsBuilder builder)
        {
            var sc = new ServiceCollection();
            AddServicesForSpecifiedDb(sc);

            //Сделано так для того чтобы в класс миграции можно было инжектировать конфигуратор бд( IPrometheusDbMigrationConfiguration ) с 
            //возможностью конфигурирования миграции в зависимости от типа бд
            Array.ForEach(this.GetType().GetInterfaces(), i => sc.AddSingleton(i, this));

            var sp = sc.AddScoped(typeof(IMigrationsAssembly), typeof(PrometheusMigrationAssembly))
                .AddScoped(typeof(IMigrationsSqlGenerator), typeof(PrometheusSqlGenerator))
                .BuildServiceProvider();

            ConfigureDb(builder);

            builder.UseInternalServiceProvider(sp);
        }

        void IEfDbConfigurator.ConfigureDatabaseMigration(MigrationBuilder builder)
        {
            ConfigureDbMigration(builder);
        }

        void IEfDbConfigurator.CongigureDatabaseModelCreating(ModelBuilder builder)
        {
            ConfigureDbModelCreating(builder);
        }

        public abstract void AddServicesForSpecifiedDb(ServiceCollection services);

        public abstract void ConfigureDb(DbContextOptionsBuilder builder);

        public abstract void ConfigureDbMigration(MigrationBuilder builder);

        public abstract void ConfigureDbModelCreating(ModelBuilder builder);
    }
}
