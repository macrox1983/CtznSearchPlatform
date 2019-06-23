using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BreakingStorm.Common.Memories
{    
    public class TreeMemoryCell<T>
    {
        public static int Calc = 0;

        private readonly object locker = new Object();

        protected IList<T> _objsInDeep { get; private set; } //Включая те, что находятся на данном уровне (сделано чтобы не было дублежа)
        protected IList<T> _objs { get; private set; }
        public IDictionary<uint, TreeMemoryCell<T>> Cells { get; private set; } = new Dictionary<uint, TreeMemoryCell<T>>();

        public TreeMemoryCell(uint key)
        {
            Calc++;
            this.Key = key;            
            this._objsInDeep = new List<T>();
        }
        public uint Key { get; private set; }       

        public void Add(T obj, IList<uint> keys)
        {
            lock (this.locker)
            {
                if (keys.Count == 0)
                {
                    if (this._objs == null) this._objs = new List<T>();
                    this._objs.Add(obj);
                    return;
                }
                foreach (uint key in keys)
                {                     
                    if (!this.Cells.TryGetValue(key, out TreeMemoryCell<T> cell))
                    {
                        cell = new TreeMemoryCell<T>(key);
                        this.Cells.Add(key, cell);
                    }
                    cell.Add(obj, keys.Where(k => k > key).ToList<uint>());
                }
                if (!this._objsInDeep.Contains(obj)) _objsInDeep.Add(obj);
            }
        }
        
        public bool Remove(T obj, IList<uint> keys)
        {
            lock (this.locker)
            {
                if (keys.Count == 0)
                {
                    this._objs.Remove(obj);
                    return this._objsInDeep.Contains(obj);
                }                 
                bool hasInDeep = false;
                foreach (uint key in keys)
                    if (this.Cells.TryGetValue(key, out TreeMemoryCell<T> cell))
                    {
                        if (cell.Remove(obj, keys.Where(k => k > key).ToList<uint>())) hasInDeep = true;
                        if (cell._objsInDeep.Count == 0) this.Cells.Remove(key);
                    }
                if (!hasInDeep) _objsInDeep.Remove(obj);
                return this._objsInDeep.Contains(obj);
            }
        }
         
        public IList<T> ObjectsFromDeep { get { return this._objsInDeep.ToList(); } }
        public IList<T> ObjectsFromCell { get { return this._objs.ToList(); } }
    }
}
