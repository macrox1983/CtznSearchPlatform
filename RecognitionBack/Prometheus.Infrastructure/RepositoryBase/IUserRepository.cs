using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Prometheus.Model;
using System;

namespace Prometheus.Infrastructure.RepositoryBase
{
    public interface IUserRepository : IRepository<Guid, ApplicationUser>
    {
        Task<ApplicationUser> FindByLogin(string login);
    }
}
