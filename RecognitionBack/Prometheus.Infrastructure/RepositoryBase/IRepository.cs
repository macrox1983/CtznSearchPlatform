using Prometheus.Infrastructure.DbContextBase;
using Prometheus.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Prometheus.Infrastructure.RepositoryBase
{
    public interface IForeignKeyValueExtractor<TDependencyDbEntity, TDependencyKey> where TDependencyDbEntity : class
    {
        Task<TDependencyKey> GetForeignKeyValue(ForeignKeyReferenceValueExtractor<TDependencyDbEntity, TDependencyKey> foreignKeyDescriptor);
    }

    public interface IRepository<TKey,TEntity> where TEntity : IEntity<TKey>
    {       
        Task<TEntity> FindBy(TKey key);

        Task<IList<TEntity>> FindAllAsync();

        Task<TEntity> FindByName(string name);

        Task<TEntity> Add(TEntity item);

        Task Add(List<TEntity> items);

        Task<TEntity> Update(TEntity item);

        //Task<TEntity> this[TKey key] {get; set;}

        Task<bool> Delete(TEntity item);
        
        //Task<TEntity> FindByPredicate(Func<TEntity, bool> predicate);
    }
}
