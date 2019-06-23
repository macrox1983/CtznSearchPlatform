using Microsoft.EntityFrameworkCore;

namespace Prometheus.Infrastructure.Configuration
{

    public interface IPrometheusDbConfigurator
    {
        string ConnectionString { get; }        

        void ConfigureDatabase(DbContextOptionsBuilder builder);

        void ConfigureModelCreating(ModelBuilder builder);
    }
}
