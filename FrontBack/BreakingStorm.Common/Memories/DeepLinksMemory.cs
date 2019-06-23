using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BreakingStorm.Common.Memories
{
    [Serializable]
    public class DeepLinksMemory
    {
        private uint currentNodeId = 1;
        private readonly object locker = new Object();
        public DeepLinksMemory()
        {
            this.InputLayer = new Dictionary<uint, DeepNode>();
            this.NodesById = new Dictionary<uint, DeepNode>();
            //this.NodesByPath = new Dictionary<string, DeepNode>();
        }        
        public IDictionary<uint,DeepNode> InputLayer { get; private set; }
        public IDictionary<uint, DeepNode> NodesById { get; private set; }
//        public IDictionary<string, DeepNode> NodesByPath { get; private set; }

        public void Add(uint objId, IList<uint> keys)
        {
            if (keys.Count == 0) return;
            foreach (uint key in keys)
            {
                DeepNode node;
                if (!this.InputLayer.TryGetValue(key, out node))
                {
                    node = new DeepNode(this.currentNodeId, key);
                    this.currentNodeId++;
                    this.NodesById.Add(node.NodeId, node);
                    this.InputLayer.Add(key, node);
                }

            }
            /*
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
            }*/
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
            if (currentKeyPosition < query.Keys.Count)
            {
                KeysRecord kr = query.Keys.ElementAt(currentKeyPosition);
                foreach (uint key in kr.Values)                
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
            if (currentKeyPosition < query.Keys.Count)
            {
                KeysRecord kr = query.Keys.ElementAt(currentKeyPosition);
                foreach (uint key in kr.Values)
                    if (this.Mems.TryGetValue(key, out memory)) memory.GetEntitiesWithoutDeep(query, currentKeyPosition + 1);
                return;
            }
            lock (this.locker)
            {
                if (this.Objs != null) query.Result.AddRange(this.Objs);                
            }
        }
        */
    }
}
