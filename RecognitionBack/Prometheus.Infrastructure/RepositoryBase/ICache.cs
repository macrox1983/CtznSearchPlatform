using Prometheus.Infrastructure.Component;
using Prometheus.Infrastructure.DbContextBase;
using Prometheus.Infrastructure.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Infrastructure.RepositoryBase
{
    public interface ICache<TEntity>
    {
        HashSet<TEntity> CachedItems { get; set; }

        void InitialCache(IComponentDbContextFactory<IPrometheusDbContext> dbContextFactory);

        TEntity FirstOrDefault(Func<TEntity, bool> predicate);
    }
}
