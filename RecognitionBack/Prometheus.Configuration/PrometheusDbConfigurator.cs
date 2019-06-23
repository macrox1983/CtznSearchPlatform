using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Prometheus.Infrastructure.Configuration;

namespace Prometheus.Configuration
{

    public abstract class PrometheusDbConfigurator : IPrometheusDbConfigurator
    {
        private readonly string _connectionString;

        public PrometheusDbConfigurator(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string ConnectionString => _connectionString;

        public virtual void ConfigureDatabase(DbContextOptionsBuilder builder)
        {
            ConfigureDatabaseImpl(builder);                       
        }

        public abstract void AddServicesForSpecifiedDatabase(ServiceCollection services);

        public abstract void ConfigureDatabaseImpl(DbContextOptionsBuilder builder);        

        public abstract void ConfigureModelCreating(ModelBuilder builder);
    }
}
