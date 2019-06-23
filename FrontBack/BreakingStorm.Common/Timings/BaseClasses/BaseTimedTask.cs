using System;
using System.Threading;

namespace BreakingStorm.Common.Timings
{
    public abstract class BaseTimedTask
    {
        private readonly object locker = new object();
      
        protected BaseTimedTask(int timeInterval_ms)
        {
            this.TimeInterval = timeInterval_ms;
            this.ExecuteTick = -1;
        }

        protected BaseTimedTask(DateTime time)
        {
            this.NeedExecuteAtTime = time;
            this.TimeInterval = -1;            
            this.ExecuteTick = -1;
        }
                
        public int TimeInterval { get; private set; } //интервал временной до выполнения задачи в мс
        public DateTime NeedExecuteAtTime { get; private set; }
        public bool RemoveTask { get; private set; } //флаг удаления задачи из диспетчера        
        public long ExecuteTick { get; set; } //тик когда необходимо выполнить задачу
        public long ExecutedTick { get; set; } //тик когда задача была выполнена

        /// <summary>
        /// Убрать задачу из списка исполнения
        /// </summary>
        public void Remove() { this.RemoveTask = true; }
        public virtual void Execute() { }        
    }
}