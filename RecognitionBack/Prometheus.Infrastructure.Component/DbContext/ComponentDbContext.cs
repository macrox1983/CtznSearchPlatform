
using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.Infrastructure.Component.DbContext
{

    public class ComponentDbContext<TDbContext> : Microsoft.EntityFrameworkCore.DbContext where TDbContext : Microsoft.EntityFrameworkCore.DbContext, IComponentDbContext
    {
        public ComponentDbContext(ComponentDbContextOptions<TDbContext> componentDbContextOptions):base(componentDbContextOptions.CurrentDbContextOptions)
        {            
        }

        public async Task<int> SaveChanges(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SaveChangesAsync(cancellationToken);
        }

        public async Task<int> SaveChanges(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
