using Prometheus.Presentation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Prometheus.Services
{
    public interface IDictionaryAggregateCrudService:
        IEntityCrudService<CityVm, Guid>
        , IEntityCrudService<RoleVm, Guid>
        , IEntityCrudService<ApplicationUserVm, Guid>
    {
        Task<List<TEntity>> GetAll<TEntity, TKey>();

        Task<TEntity> Find<TEntity, TKey>(TKey guid);

        List<DictionaryVm> GetDictionaries();


        Task<TEntity> FindById<TEntity, TKey>(string name);
    }
}