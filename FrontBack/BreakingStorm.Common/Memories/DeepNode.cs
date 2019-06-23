using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BreakingStorm.Common.Memories
{
    [Serializable]
    public class DeepNode
    {
        private readonly object locker = new Object();
        public DeepNode(uint nodeId, uint key, string linksPath="")
        {
            this.NodeId = nodeId;
            this.Key = key;
      //      this.LinksPath = linksPath;
            this.ObjsIdsInDeep = new List<uint>();
            this.ObjsIdsAtNode = new List<uint>();
            this.Links = new Dictionary<uint, DeepNode> ();
        }
        public uint NodeId { get; private set; }
        public uint Key { get; private set; }
    //    public string LinksPath { get; private set; }
        public IList<uint> ObjsIdsInDeep { get; private set; } //Включая те, что находятся на данном уровне (сделано чтобы не было дублежа)
        public IList<uint> ObjsIdsAtNode { get; private set; }
        public IDictionary<uint, DeepNode> Links { get; private set; }
        /*
        public void Add(uint objId, IList<uint> keys)
        {
            if (keys.Count == 0)
            lock (this.locker)
            {
                
                {
                    if (this.Objs == null) this.Objs = new List<T>();
                    this.Objs.Add(obj);                    
                    return;
                }
                foreach (uint key in keys)
                {
                    DeepMemory<T> record;
                    if (!this.Links.TryGetValue(key, out record))
                    {
                        record = new DeepMemory<T>(key);
                        this.Links.Add(key, record);
                    }
                    record.Add(obj, keys.Where(k => k != key).ToList<uint>());
                }
                if (!this.ObjsInDeep.Contains(obj)) ObjsInDeep.Add(obj);                
            }
        }
        /*
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
                    if (this.Links.TryGetValue(key, out mem))
                    {
                        if (mem.insideRemove(obj, keys.Where(k => k != key).ToList<uint>())) hasInDeep = true;                        
                        if (mem.ObjsInDeep.Count == 0) this.Links.Remove(key);
                    }
                if (!hasInDeep) ObjsInDeep.Remove(obj);
                return this.ObjsInDeep.Contains(obj);
            }
        }


        public void GetEntitiesFromDeep(DeepMemoryQuery<T> query, int currentKeyPosition = 0)
        {
            DeepMemory<T> memory;            
            if (currentKeyPosition < query.Keys.Count)
            {
                KeysRecord kr = query.Keys.ElementAt(currentKeyPosition);
                foreach (uint key in kr.Values)                
                    if (this.Links.TryGetValue(key, out memory)) memory.GetEntitiesFromDeep(query, currentKeyPosition + 1);
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
            if (currentKeyPosition < query.Keys.Count)
            {
                KeysRecord kr = query.Keys.ElementAt(currentKeyPosition);
                foreach (uint key in kr.Values)
                    if (this.Links.TryGetValue(key, out memory)) memory.GetEntitiesWithoutDeep(query, currentKeyPosition + 1);
                return;
            }
            lock (this.locker)
            {
                if (this.Objs != null) query.Result.AddRange(this.Objs);                
            }
        }*/

    }
}
