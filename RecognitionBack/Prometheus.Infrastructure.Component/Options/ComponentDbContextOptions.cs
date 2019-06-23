using Microsoft.EntityFrameworkCore;

namespace Prometheus.Infrastructure.Component.DbContext
{
    public class ComponentDbContextOptions<TDbContext> where TDbContext:Microsoft.EntityFrameworkCore.DbContext
    {
        public ComponentDbContextOptions(DbContextOptions<TDbContext> dbContextOptions)
        {
            CurrentDbContextOptions = dbContextOptions;
        }

        public DbContextOptions<TDbContext> CurrentDbContextOptions { get; }
    }
}
