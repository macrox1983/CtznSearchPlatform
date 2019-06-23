using System;
using System.Threading;

namespace BreakingStorm.Common.Timings
{
    public class TimeFixedTask:BaseTimedTask
    {
        public delegate void voidDelegate();
        private readonly voidDelegate _handler;
        private readonly object locker = new object();

        public TimeFixedTask(DateTime executeAtTime, voidDelegate handler):base(executeAtTime)
        {
            this._handler = handler;
        }                
        
        public override void Execute() { this._handler?.Invoke(); }        
    }
}