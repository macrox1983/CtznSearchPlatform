using Microsoft.EntityFrameworkCore;
using Prometheus.Infrastructure.Component;
using Prometheus.Infrastructure.DbContextBase;
using Prometheus.Infrastructure.RepositoryBase;
using Prometheus.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prometheus.DAL
{
    public class ConstantRepository : IConstantRepository
    {
        private readonly IComponentDbContextFactory<IPrometheusDbContext> _dbContextFactory;

        public ConstantRepository(IComponentDbContextFactory<IPrometheusDbContext> dbContextFactory)
        {

            _dbContextFactory = dbContextFactory ??
                throw new ArgumentNullException(nameof(dbContextFactory));
        }

        public async Task<Constant> Add(Constant item)
        {
            using (var context = _dbContextFactory.GetDbContext())
            {
                context.Constant.Add(item);
                await context.SaveChanges();
                return item;
            }
        }

        public async Task<IList<Constant>> FindAllAsync()
        {
            using (var context = _dbContextFactory.GetDbContext())
            {
                return await context.Constant.ToListAsync();
            }
        }

        public async Task<Constant> FindBy(Guid key)
        {
            using (var context = _dbContextFactory.GetDbContext())
            {
                var user = await context.Constant.FirstOrDefaultAsync(u => u.ConstantId.Equals(key));
                return user;
            }
        }

        public async Task<Constant> FindByName(string name)
        {
            using (var context = _dbContextFactory.GetDbContext())
            {
                return await context.Constant.FirstOrDefaultAsync(u => u.ConstantName.ToLower() == name.ToLower());
            }
        }

        public async Task<bool> Delete(Constant item)
        {
            using (var context = _dbContextFactory.GetDbContext())
            {
                context.Constant.Remove(item);
                await context.SaveChanges();
                return true;
            }
        }

        public async Task<Constant> Update(Constant item)
        {
            using (var context = _dbContextFactory.GetDbContext())
            {
                context.Constant.Update(item);
                await context.SaveChanges();
                return item;
            }
        }

        public async Task<Constant> FindByConstantCode(string airlineCode)
        {
            using (var context = _dbContextFactory.GetDbContext())
            {
                return await context.Constant.FirstOrDefaultAsync(u => u.ConstantName.ToLower() == airlineCode.ToLower());
            }
        }

        public async Task Add(List<Constant> items)
        {
            using (var context = _dbContextFactory.GetDbContext())
            {
                context.Constant.AddRange(items);
                await context.SaveChanges();
            }
        }
    }
}
