using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.Infrastructure.Component.DbContext
{
    public interface IComponentDbContext : IDisposable
    {
        Task<int> SaveChanges(CancellationToken cancellationToken = default(CancellationToken));

        Task<int> SaveChanges(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
    }
}
