using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakingStorm.Common.Memories
{
    /* Ремарка как это работает
     черный и белый хлеб без кунжута
       1 and 2 or 3 not 4 => or 3 or (1 and 2) not 4
     Зеленый лук без красной тетевы
        1 or 2 not 3 and 4 => or 1 or 2 not (3 and 4)
    */
    public enum LogicLinksEnum { Or=0, Not=1 }
    public class LogicGroup
    {
        private readonly string _andListString;
        public LogicGroup(LogicLinksEnum op, IList<uint> andList)
        {
            this.Op = op;
            this.List = andList;
            this._andListString = String.Join("-", andList.Select(s => s.ToString("x")));
        }
        public LogicGroup(LogicLinksEnum op, uint andId)
        {
            this.Op = op;
            this.List = new List<uint>(); (this.List as List<uint>).Add(andId);
            this._andListString = andId.ToString("x");
        }        
        public LogicGroup(string stringRecord)
        {
            List<uint> list = new List<uint>();
            this.List = list;
            if (stringRecord != "")
            {
                string[] parser = stringRecord.Split('?');
                if (!Enum.TryParse<LogicLinksEnum>(parser[0], out LogicLinksEnum op)) throw new Exception("Cant parse LogicRecord");
                this.Op = op;
                foreach (string andStr in parser[1].Split('-'))
                {   uint andId = Convert.ToUInt32(andStr,16);
                    if (andStr!=andId.ToString("x")) { throw new Exception("Cant parse LogicRecord"); }
                    list.Add(andId);
                }
                this._andListString = String.Join("-", list.Select(s => s.ToString("x")));
            }                
        }
        public LogicLinksEnum Op { get; private set; }
        public IList<uint> List { get; private set; }

        public override string ToString() { return this.Op+"?"+this._andListString; }
    }
        
    public class LogicLinks
    {
        
        private readonly object _locker = new object();        
        private readonly List<LogicGroup> _records = new List<LogicGroup>();

        public LogicLinks()
        {            
      
        }
        
        public IEnumerable<LogicGroup> GetRecords() { lock (this._locker) { return this._records.ToList(); } }
        public LogicGroup GetRecord(int position) { return this._records[position]; }
        public bool IsEmpty() { return this._records.Count == 0; }
        public int Count { get { return this._records.Count; } }
        public void Add(IList<LogicGroup> records) { this._records.AddRange(records); this.optimize(); }
        public void Add(LogicGroup record) { this._records.Add(record); this.optimize(); }
        public void Remove(LogicGroup record) { this._records.RemoveAll(s => (s.ToString() == record.ToString())); this.optimize(); }
        private void optimize()
        {
            lock (this._locker)
            {
                List<LogicGroup> _newList = this._records.Where(s => s.Op == LogicLinksEnum.Or).OrderBy(s => s.List.Count()).ToList();
                _newList.AddRange(this._records.Where(s => s.Op == LogicLinksEnum.Not).ToList());
                this._records.Clear();
                this._records.AddRange(_newList);
            }            
        }
        public override string ToString()
        {
            string result = "";
            foreach (LogicGroup lr in this._records)
                result += lr.ToString()+"&";
            return result;
        }
    }
}
