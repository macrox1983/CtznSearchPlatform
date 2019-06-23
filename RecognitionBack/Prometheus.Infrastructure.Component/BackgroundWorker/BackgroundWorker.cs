using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.Infrastructure.Component.BackgroundWorker
{
    public class BackgroundWorker
    {
        /// <summary>
        /// Функция запускает бесконечное выполнение действия
        /// </summary>
        /// <param name="stopAction"> критерий полной остановки</param>
        /// <param name="pauseAction"> критерий паузы </param>
        /// <param name="work"> выполняемое действие</param>
        /// <param name="timeBetweenIteration">таймаут между вызовами выполняемого действия </param>
        /// <returns></returns>
        public static Task StartEndlessWork(Func<bool> stopAction, Func<bool> pauseAction, Action work, int timeBetweenIteration = 50)
        {
            return Task.Run(() =>
            {
                while (!stopAction())
                {
                    if (!pauseAction())
                    {
                        work();
                        Thread.Sleep(timeBetweenIteration);
                    }
                }
            });
        }
    }
}
