using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BreakingStorm.Common.Memories
{
    [Serializable]
    public class DeepMemory<T>
    {
        private readonly object locker = new Object();
        public DeepMemory(uint key = 0)
        {
            this.Key = key;
            this.Mems = new Dictionary<uint, DeepMemory<T>>();
            this.ObjsInDeep = new List<T>();
        }
        public uint Key { get; private set; }        
        public IList<T> ObjsInDeep { get; private set; } //Включая те, что находятся на данном уровне (сделано чтобы не было дублежа)
        public IList<T> Objs { get; private set; }
        public IDictionary<uint, DeepMemory<T>> Mems { get; private set; }

        public void Add(T obj, IList<uint> keys)
        {
            lock (this.locker)
            {
                if (keys.Count == 0)
                {
                    if (this.Objs == null) this.Objs = new List<T>();
                    this.Objs.Add(obj);                    
                    return;
                }
                foreach (uint key in keys)
                {
                    DeepMemory<T> record;
                    if (!this.Mems.TryGetValue(key, out record))
                    {
                        record = new DeepMemory<T>(key);
                        this.Mems.Add(key, record);
                    }
                    record.Add(obj, keys.Where(k => k != key).ToList<uint>());
                }
                if (!this.ObjsInDeep.Contains(obj)) ObjsInDeep.Add(obj);                
            }
        }

        public void Remove(T obj, IList<uint> keys)
        {
            this.insideRemove(obj, keys);
        }

        protected bool insideRemove(T obj, IList<uint> keys)
        {
            lock (this.locker)
            {
                if (keys.Count == 0)
                {
                    this.Objs.Remove(obj);
                    return this.ObjsInDeep.Contains(obj);                    
                }
                DeepMemory<T> mem;
                bool hasInDeep = false;
                foreach (uint key in keys)
                    if (this.Mems.TryGetValue(key, out mem))
                    {
                        if (mem.insideRemove(obj, keys.Where(k => k != key).ToList<uint>())) hasInDeep = true;                        
                        if (mem.ObjsInDeep.Count == 0) this.Mems.Remove(key);
                    }
                if (!hasInDeep) ObjsInDeep.Remove(obj);
                return this.ObjsInDeep.Contains(obj);
            }
        }


        public void GetEntitiesFromDeep(DeepMemoryQuery<T> query, int currentKeyPosition = 0)
        {
            DeepMemory<T> memory;            
            if (currentKeyPosition < query.Links.Count)
            {
                LogicGroup lr = query.Links.GetRecord(currentKeyPosition);                
                foreach (uint key in lr.List)
                    if (this.Mems.TryGetValue(key, out memory)) memory.GetEntitiesFromDeep(query, currentKeyPosition + 1);
                return;
            }
            lock (this.locker)
            {
                if (this.ObjsInDeep != null) query.Result.AddRange(this.ObjsInDeep);                
            }
        }

        public void GetEntitiesWithoutDeep(DeepMemoryQuery<T> query, int currentKeyPosition = 0)
        {
            DeepMemory<T> memory;
            if (currentKeyPosition < query.Links.Count)
            {
                LogicGroup lr = query.Links.GetRecord(currentKeyPosition);
                foreach (uint key in lr.List)
                    if (this.Mems.TryGetValue(key, out memory)) memory.GetEntitiesWithoutDeep(query, currentKeyPosition + 1);                
                return;
            }
            lock (this.locker)
            {
                if (this.Objs != null) query.Result.AddRange(this.Objs);                
            }
        }

    }
}
