using System;
using System.Threading;

namespace BreakingStorm.Common.Timings
{
    public class TimeIntervalTask:BaseTimedTask
    {
        public delegate void voidDelegate();
        private readonly voidDelegate _handler;
        private readonly object locker = new object();

        public TimeIntervalTask(int timeInterval_ms, voidDelegate handler):base(timeInterval_ms)
        {
            this._handler = handler;
        }                
        
        public override void Execute() { this._handler?.Invoke(); }        
    }
}