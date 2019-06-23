using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BreakingStorm.Common.Loggers;

namespace BreakingStorm.Common.Timings
{
    /// <summary>
    /// Диспетчер задач, основанный на списке задач по процесорным тикам
    /// </summary>
    public class TasksDispatcher : IDisposable
    {
        private TimeSpan _stepSize;
        private readonly object _locker = new object();

        private readonly Logger _logger;

        private readonly Thread _workerThread;
        private EventWaitHandle _workerdWait;

        private SortedDictionary<long, BaseTimedTask> _tasks = new SortedDictionary<long, BaseTimedTask>();
        private bool _needStop = false;
        

        public TasksDispatcher(Logger logger, int stepSize_ms=100)
        {
            this._stepSize = new TimeSpan(0,0,0,0,stepSize_ms);
            this._logger = logger;            
            this._workerThread = new Thread(worker);
            this._workerThread.Start();
            while (this._workerdWait == null) Thread.Sleep(1);//убеждаемся что обработчик запустился
        }
        
        private void worker(object state)
        {
            this._workerdWait = new AutoResetEvent(true);
            while (!_needStop)
            {
                if (this._tasks.Count == 0) { this._workerdWait.WaitOne(_stepSize); continue; }
                DateTime now = DateTime.Now;
                TimeSpan wait = new TimeSpan(100);
                lock (this._locker)
                {
                    if (this._tasks.Count == 0) continue;
                    BaseTimedTask task = this._tasks.Values.First();
                    if (task.RemoveTask) //запрос на удаление задачи
                    {
                        this._tasks.Remove(task.ExecuteTick);
                        continue;
                    }
                    if (task.ExecuteTick < now.Ticks) //задача на выполнение
                    {
                        this._tasks.Remove(task.ExecuteTick);
                        task.ExecutedTick = now.Ticks;
                        Task.Run(() => { this.saveExecutor(task); });                                                    
                        continue;
                    }
                    //время еще не пошло, ожидаем                    
                    wait = new TimeSpan(task.ExecuteTick-now.Ticks);
                    if (wait > _stepSize) wait = _stepSize;
                }
                this._workerdWait.WaitOne(wait);
            }
        }

        private void saveExecutor(BaseTimedTask task)
        {
            try { task.Execute(); }
            catch (Exception e) { this._logger.Error(e, "TaskDispatcher cant execute task"); }
        }

        public void AddTask(BaseTimedTask task)
        {            
            lock (this._locker)
            {
                if (task.TimeInterval < 0)
                    task.ExecuteTick = task.NeedExecuteAtTime.Ticks; else
                    task.ExecuteTick = DateTime.Now.Ticks + task.TimeInterval*TimeSpan.TicksPerMillisecond;                                
                while (true)                
                    if (this._tasks.ContainsKey(task.ExecuteTick)) task.ExecuteTick++; else
                    {
                        this._tasks.Add(task.ExecuteTick, task);
                        break;
                    }                
                if (this._tasks.Count==1) this._workerdWait.Set();            
            }
        }

        public void Dispose()
        {
            this._needStop = true;
            this._workerdWait.Set();
        }


    }
}