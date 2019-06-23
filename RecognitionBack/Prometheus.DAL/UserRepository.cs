using Microsoft.EntityFrameworkCore;
using Prometheus.Infrastructure.Component;
using Prometheus.Infrastructure.DbContextBase;
using Prometheus.Infrastructure.RepositoryBase;
using Prometheus.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prometheus.DAL
{
    public class UserRepository : IUserRepository
    {
        private readonly IComponentDbContextFactory<IPrometheusDbContext> _dbContextFactory;

        public UserRepository(IComponentDbContextFactory<IPrometheusDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory??
                throw new ArgumentNullException(nameof(dbContextFactory));
        }
        //public ApplicationUser this[Guid key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }        

        public async Task<ApplicationUser> Add(ApplicationUser item)
        {
            using (var context = _dbContextFactory.GetDbContext())
            {
                context.ApplicationUser.Add(item);
                await context.SaveChanges();
                return item;
            }            
        }

        public async Task<IList<ApplicationUser>> FindAllAsync()
        {
            using (var context = _dbContextFactory.GetDbContext())
            {
                var users = await context.ApplicationUser
                    //.Include(app => app.Role)
                    //.Select(c => new { c.Login, c.})
                    .Where(s => !s.IsDeleted).ToListAsync();
                return users;
            }
        }

        public async Task<ApplicationUser> FindBy(Guid key)
        {
            using (var context = _dbContextFactory.GetDbContext())
            {
                var user = await context.ApplicationUser.FirstOrDefaultAsync(u => u.UserId.Equals(key));
                return user;
            }
        }

        public async Task<ApplicationUser> FindByLogin(string login)
        {
            using (var context = _dbContextFactory.GetDbContext())
            {
                return await context.ApplicationUser
                    //.Include(u=>u.Role)
                    .FirstOrDefaultAsync(u => u.Login.ToLower() == login.ToLower());
            }
        }

        public async Task<bool> Delete(ApplicationUser item)
        {
            using (var context = _dbContextFactory.GetDbContext())
            {
                item.IsDeleted = true;
                context.ApplicationUser.Update(item);
                await context.SaveChanges();
                return true;
            }
        }

        public async Task<ApplicationUser> Update(ApplicationUser item)
        {
            using (var context = _dbContextFactory.GetDbContext())
            {
                context.ApplicationUser.Update(item);
                await context.SaveChanges();
                return item;
            }
        }

        public Task<ApplicationUser> FindByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task Add(List<ApplicationUser> items)
        {
            throw new NotImplementedException();
        }
    }
}
