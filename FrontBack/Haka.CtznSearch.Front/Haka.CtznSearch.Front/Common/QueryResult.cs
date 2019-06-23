using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Haka.CtznSearch.Front
{
    public class QueryResult
    {
        public QueryResult(QueryResultEnum error, Object obj = null)
        {
            this.Error = error;
            this.Object = obj;            
        }
        public QueryResultEnum Error { get; set; }
        public Object Object { get; set; } = null;

        public bool IsExecuted() => Error == QueryResultEnum.Executed;
        
    }

    public class QueryResultTyped<T>
    {
        public QueryResultTyped(QueryResultEnum error, T obj = default)
        {
            this.Error = error;
            this.Object = obj;
        }
        public QueryResultEnum Error { get; set; }
        public T Object { get; set; } = default;

        public bool IsExecuted() => Error == QueryResultEnum.Executed;        
    }
}
