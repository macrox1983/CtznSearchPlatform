using Prometheus.Model;
using System;

namespace Prometheus.Infrastructure.RepositoryBase
{
    public interface IConstantRepository : IRepository<Guid, Constant>
    {
    }
}
