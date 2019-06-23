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
    public class CityCache : ICache<City>
    {
        private HashSet<City> _cachedItems;
        public HashSet<City> CachedItems { get => _cachedItems; set => _cachedItems = value; }

        public City FirstOrDefault(Func<City, bool> predicate)
        {
            return _cachedItems.FirstOrDefault(predicate);
        }

        public void InitialCache(IComponentDbContextFactory<IPrometheusDbContext> dbContextFactory)
        {
            using (var context = dbContextFactory.Create())
            {
                _cachedItems = context.City.ToHashSet();
            }
        }
    }

    public class CityRepository : ICityRepository
    {
        private readonly IComponentDbContextFactory<IPrometheusDbContext> _dbContextFactory;

        private ICache<City> _cache;

        public CityRepository(ICache<City> cache, IComponentDbContextFactory<IPrometheusDbContext> dbContextFactory)
        {
            _cache = cache;
            _dbContextFactory = dbContextFactory ??
                throw new ArgumentNullException(nameof(dbContextFactory));
        }

        public async Task<City> Add(City item)
        {
            using (var context = _dbContextFactory.GetDbContext())
            {
                context.City.Add(item);
                await context.SaveChanges();
                return item;
            }
        }

        public async Task<IList<City>> FindAllAsync()
        {
            using (var context = _dbContextFactory.GetDbContext())
            {
                return await context.City                  
                    .Where(s => !s.IsDeleted).ToListAsync();
            }
        }

        public async Task<City> FindBy(Guid key)
        {
            using (var context = _dbContextFactory.GetDbContext())
            {
                var user = await context.City.FirstOrDefaultAsync(u => u.CityId.Equals(key));
                return user;
            }
        }

        public async Task<City> FindByName(string name)
        {
            var cachedItem = _cache.FirstOrDefault(i => i.Name.ToLower() == name.ToLower());
            if (cachedItem != null)
                return cachedItem;

            using (var context = _dbContextFactory.GetDbContext())
            {
                return await context.City.FirstOrDefaultAsync(u => u.Name.ToLower() == name.ToLower());
            }
        }

        public async Task<bool> Delete(City item)
        {
            using (var context = _dbContextFactory.GetDbContext())
            {
                item.IsDeleted = true;
                context.City.Update(item);
                await context.SaveChanges();
                return true;
            }
        }

        public async Task<City> Update(City item)
        {
            using (var context = _dbContextFactory.GetDbContext())
            {
                context.City.Update(item);
                await context.SaveChanges();
                return item;
            }
        }

        public async Task Add(List<City> items)
        {
            using (var context = _dbContextFactory.GetDbContext())
            {
                context.City.AddRange(items);
                await context.SaveChanges();
            }
        }
    }
}
