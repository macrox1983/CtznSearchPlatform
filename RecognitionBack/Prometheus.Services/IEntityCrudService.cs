using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Prometheus.Services
{
    public interface IEntityCrudService<TViewEntity, TKey>
    {
        Task<List<TViewEntity>> GetAll();

        Task<TViewEntity> Create(TViewEntity viewEntity);

        Task<TViewEntity> Update(TViewEntity viewEntity);

        Task<bool> Delete(TViewEntity viewEntity);

        Task<TViewEntity> Find(TKey id);

        Task<TKey> FindByName(string name);
    }
}