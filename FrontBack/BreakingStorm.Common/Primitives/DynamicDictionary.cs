using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakingStorm.Common
{
    public class DynamicDictionary
    {
        private IDictionary<string, object> _lib = new Dictionary<string, object>();
        public DynamicDictionary() { }
        public void Add(string key, object obj) { this._lib.Add(key, obj); }
        public void Remove(string key) { this._lib.Remove(key); }
        public bool ContainsKey(string key) { return this._lib.ContainsKey(key); }        
        public object GetValue(string key) { if (_lib.ContainsKey(key)) return this._lib[key]; else return null; }
        public ICollection<string> Keys { get { return this._lib.Keys; } }
        public ICollection<object> Values { get { return this._lib.Values; } }
    }
}
