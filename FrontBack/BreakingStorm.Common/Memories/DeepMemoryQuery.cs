using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BreakingStorm.Common.Memories
{
    public class DeepMemoryQuery<T>
    {
        public DeepMemoryQuery(LogicLinks links)
        {
            this.Links = links;
            this.Result = new List<T>();
        }

        public LogicLinks Links { get; private set; }
        public List<T> Result { get; private set; }

    }
}
