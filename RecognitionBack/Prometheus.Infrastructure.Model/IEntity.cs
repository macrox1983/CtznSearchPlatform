using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Infrastructure.Model
{
    public interface IEntity<TKey>
    {
        TKey Key { get; }
    }
}
