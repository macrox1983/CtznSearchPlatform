using System;
using System.Collections.Generic;
using System.Text;
using Prometheus.Model;


namespace Prometheus.Infrastructure.RepositoryBase
{
    public interface ICityRepository : IRepository<Guid, City>
    {
    }
}
