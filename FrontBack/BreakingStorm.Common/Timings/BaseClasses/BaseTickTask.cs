using System;

namespace BreakingStorm.Common.Timings
{
    public abstract class BaseTickTask
    {

        public delegate void DelegateTickTaskCallback(BaseTickTask basetask);
        protected DelegateTickTaskCallback _callback;


        protected BaseTickTask(byte inTick, DelegateTickTaskCallback callback)
        {
            this.InTick = inTick;
            this._callback = callback;
        }

        public bool RemoveTask { get; private set; } //флаг удаления задачи из диспетчера
        public byte InTick { get; private set; } //через сколько тиков выполнить
        public long ExecutedTick { get; set; } //тик когда задача была выполнена

        /// <summary>
        /// Убрать задачу из списка исполнения
        /// </summary>
        public void Remove() { this.RemoveTask = true; }
        public abstract void Execute();
    }
}