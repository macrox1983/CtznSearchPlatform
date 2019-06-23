using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Prometheus.Infrastructure.Component.Configuration;
using Prometheus.Infrastructure.Component.DbMigration;
using System;

namespace Prometheus.Infrastructure.Component.DbConfigurator
{
    public abstract class EfDbConfigurator : IEfDbConfigurator
    {
        private readonly DbConfiguration _dbConfiguration;        

        public EfDbConfigurator(DbConfiguration dbConfiguration)
        {            
            _dbConfiguration = dbConfiguration ?? throw new ArgumentNullException(nameof(dbConfiguration));
        }

        public DbConfiguration DbConfiguration => _dbConfiguration;

        void IEfDbConfigurator.ConfigureDatabase(DbContextOptionsBuilder builder)
        {
            var sc = new ServiceCollection();
            AddServicesForSpecifiedDb(sc);

            //Сделано так для того чтобы в класс миграции можно было инжектировать конфигуратор бд( IPrometheusDbMigrationConfiguration ) с 
            //возможностью конфигурирования миграции в зависимости от типа бд
            Array.ForEach(this.GetType().GetInterfaces(), i => sc.AddSingleton(i, this));

            var sp = sc.AddScoped(typeof(IMigrationsAssembly), typeof(DbMigrationAssembly))
                .AddScoped(typeof(IMigrationsSqlGenerator), typeof(ComponentSqlGenerator))
                .AddTransient(typeof(IMigrator), typeof(ComponentDbMigrator))
                .AddSingleton(typeof(IEfDbConfigurator), this)
                .BuildServiceProvider();            

            ConfigureDb(builder);

            builder.UseInternalServiceProvider(sp);
        }

        void IEfDbConfigurator.ConfigureDatabaseMigration(MigrationBuilder builder)
        {
            ConfigureDbMigration(builder);
        }

        void IEfDbConfigurator.ConfigureDatabaseModelCreating(ModelBuilder builder)
        {
            ConfigureDbModelCreating(builder);
        }

        public Type GetSpecifiedMigrationSqlGeneratorType()
        {
            return GetSpecifiedMigrationsSqlGeneratorType();
        }

        public abstract void AddServicesForSpecifiedDb(ServiceCollection services);

        public abstract void ConfigureDb(DbContextOptionsBuilder builder);

        public abstract void ConfigureDbMigration(MigrationBuilder builder);

        public abstract void ConfigureDbModelCreating(ModelBuilder builder);

        public abstract Type GetSpecifiedMigrationsSqlGeneratorType();
    }
}
