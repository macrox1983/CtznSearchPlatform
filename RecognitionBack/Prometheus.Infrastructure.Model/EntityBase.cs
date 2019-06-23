using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Infrastructure.Model
{
    /// <summary>
    /// Базовый класс для всех сущностей системы
    /// </summary>
    /// <typeparam name="TKey">Тип первичного ключа сущности</typeparam>
    public abstract class EntityBase<TKey> : IEntity<TKey>
    {
        private TKey _key;
        protected EntityBase(TKey key)
        {
            _key = key;
            if (_key == null)
            {
                _key = NewKey();
            }            
        }

        public virtual TKey Key => _key;

        public abstract TKey NewKey();        
    }

    /// <summary>
    /// Базовый класс для всех сущностей системы имеющих ключ типа GUID
    /// </summary>
    public abstract class EntityBase : EntityBase<Guid>
    {
        protected EntityBase(Guid key) : base(key)
        {
        }

        public override Guid NewKey()
        {
            return Guid.NewGuid();
        }
    }
}
