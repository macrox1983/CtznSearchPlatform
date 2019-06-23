using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BreakingStorm.Common.Memories
{    
    public class TreeMemory<T>
    {
        private readonly object locker = new Object();
        private readonly TreeMemoryCell<T> _startCell = new TreeMemoryCell<T>(0);
        public TreeMemory()
        {            

        }

        public void Add(T obj, IList<uint> keys)
        {
            this._startCell.Add(obj, keys);
        }
        public void Remove(T obj, IList<uint> keys)
        {
            this._startCell.Remove(obj, keys);
        }

        public IList<T> Get(LogicLinks ll)
        {            
            List<TreeMemoryCell<T>> lastStepCells = new List<TreeMemoryCell<T>>() { this._startCell };
            foreach(LogicGroup lg in ll.GetRecords())
            {
                List<TreeMemoryCell<T>> thisStepCells = new List<TreeMemoryCell<T>>();
                switch (lg.Op)
                {
                    case LogicLinksEnum.Or:
                        foreach (uint key in lg.List)
                            foreach (TreeMemoryCell<T> cell in lastStepCells)
                                if (cell.Cells.TryGetValue(key, out TreeMemoryCell<T> nextCell)) thisStepCells.Add(nextCell);
                        break;
                    case LogicLinksEnum.Not:
                        foreach (TreeMemoryCell<T> cell in lastStepCells) thisStepCells.AddRange(cell.Cells.Values);
                        thisStepCells = thisStepCells.Distinct().ToList();
                        foreach (uint key in lg.List) thisStepCells.RemoveAll(s => s.Key == key);      
                        break;
                }
                lastStepCells = thisStepCells;
            }
            List<T> result = new List<T>();
            foreach (TreeMemoryCell<T> cell in lastStepCells)
                result.AddRange(cell.ObjectsFromCell);
            foreach (TreeMemoryCell<T> cell in lastStepCells)
                result.AddRange(cell.ObjectsFromDeep);
            result = result.Distinct().ToList();
            return result;
        }       
    }
}
