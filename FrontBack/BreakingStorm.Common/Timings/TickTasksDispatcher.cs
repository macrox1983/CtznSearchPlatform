using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using BreakingStorm.Common.Loggers;

namespace BreakingStorm.Common.Timings
{
    public class TickTasksDispatcher : IDisposable
    {
        public delegate void TickTaskDispatcherDelegate(TickTasksDispatcher taskDispatcher);

        private readonly object _locker = new object();

        private readonly int _optimalTickDelay;
        private readonly bool _executeInOtherThread;
        private readonly Logger _logger;

        private readonly Thread _workerThread;        
        private IList<IList<BaseTickTask>> _ticksLists;
        private IList<int> _tickExecuteMs;
        private bool _needStop = false;

        public event TickTaskDispatcherDelegate ChangeTickSize;



        public TickTasksDispatcher(Logger logger, int tickDelayMs, bool executeInOtherThread=false)
        {
            this._logger = logger;
            this._optimalTickDelay = tickDelayMs;
            this._executeInOtherThread = executeInOtherThread;
            this._ticksLists = new List<IList<BaseTickTask>>();
            this._tickExecuteMs = new List<int>();                        
            this._workerThread = new Thread(worker);
            this._workerThread.Start();
        }
                
        public int CurrentTickExecuteDelay { get; private set; }



        private void worker(object state)
        {
            this.CurrentTickExecuteDelay = this._optimalTickDelay;
            long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            while (!_needStop)
            {
                int curDelay = (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - now);
                int delta = this._optimalTickDelay - curDelay;
                if (delta < 0) this._tickExecuteMs.Add(curDelay); else this._tickExecuteMs.Add(_optimalTickDelay);
                if (this._tickExecuteMs.Count > 3) this._tickExecuteMs.RemoveAt(0);
                int sa = 0; foreach (int td in this._tickExecuteMs) sa = sa + td;
                int newCurrentDelay = sa / this._tickExecuteMs.Count;
                if (this.CurrentTickExecuteDelay!= newCurrentDelay)
                {
                    this.CurrentTickExecuteDelay = newCurrentDelay;
                    this.ChangeTickSize?.Invoke(this);
                }
                //осталось время до следующего тика
                if (delta > 0) { Thread.Sleep(delta); }                

                now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                IList<BaseTickTask> currentList;
                lock (this._locker)
                {
                    if (this._ticksLists.Count == 0) continue;
                    currentList = this._ticksLists[0];
                    this._ticksLists.RemoveAt(0);                
                    if (currentList == null) continue;
                    if (delta < 1) delta = 1;
                    Thread.Sleep(delta);
                    foreach (BaseTickTask task in currentList)
                    {
                        if (task.RemoveTask) continue;
                        task.ExecutedTick = now;                        
                        if (this._executeInOtherThread)
                        {
                            Thread executor = new Thread(task.Execute);
                            executor.Start();
                        }
                        else
                        {
                            try { task.Execute(); }
                            catch (Exception e) { this._logger.Error(e, "TickTaskDispatcher cant execute task"); }
                        }
                    }
                }
            }
        }
        
        public void AddTask(BaseTickTask task)
        {
            lock (this._locker)
            {
                while (this._ticksLists.Count<task.InTick) { this._ticksLists.Add(null); }                
                if (this._ticksLists[task.InTick-1] == null) this._ticksLists[task.InTick-1] = new List<BaseTickTask>();
                this._ticksLists[task.InTick-1].Add(task);
            }
        }
        public void AddTasks(IList<BaseTickTask> tasks)
        {
            lock (this._locker)
            {
                foreach (BaseTickTask task in tasks)
                { 
                    while (this._ticksLists.Count < task.InTick) { this._ticksLists.Add(null); }
                    if (this._ticksLists[task.InTick-1] == null) this._ticksLists[task.InTick-1] = new List<BaseTickTask>();
                    this._ticksLists[task.InTick-1].Add(task);
                }
            }
        }        

        public void Dispose()
        {
            this._needStop = true;
        }

    
    }
}