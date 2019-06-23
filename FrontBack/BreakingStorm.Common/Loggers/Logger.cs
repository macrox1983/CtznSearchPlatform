using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using BreakingStorm.Common.Helpers;

namespace BreakingStorm.Common.Loggers
{
    public class Logger : IDisposable
    {
        public delegate void CriticalCall();        
        /// <summary>
        /// Вызывается при возникновении критической ситуации
        /// </summary>
        public event CriticalCall OnCriticalEvent;

        public delegate void LogToExternal(string log, LogType logType);
        public event LogToExternal OnLogToExternal;

        private StreamWriter _writerLog;
        private StreamWriter _writerError;
        private Thread _timer;
        private readonly object _locker = new object();
        private bool _disposed = false;

        private uint _configSecondsToFlush = 120;
        private uint _configSecondsToRotate = 0;
        private readonly string _configLogDirectory;
        private string _configLogFile;
        private string _configErrorFile;

        private readonly IList<LogType> _typeForLogging = Enum.GetValues(typeof(LogType)).Cast<LogType>().ToList();
        private readonly IList<LogType> _typeForConsole = Enum.GetValues(typeof(LogType)).Cast<LogType>().ToList();
        private readonly IList<LogType> _typeForImmediatelyFlush = new List<LogType>();

        public Logger(string configFile = "")
        {            
            this._typeForImmediatelyFlush.Add(LogType.Error);
            this._typeForImmediatelyFlush.Add(LogType.Critical);                                    
            this._configLogDirectory = Directory.GetCurrentDirectory() + "/";
            this._configLogFile = _configLogDirectory + "logs_" + DateTime.Now.Ticks + ".txt";
            this._configErrorFile = _configLogDirectory + "errors_" + DateTime.Now.Ticks + ".txt";
            if ((configFile != "") && (File.Exists(configFile)))
            {
                if (!HelperSerialization.TryJSONFileDeserialization<LoggerConfig>(configFile, out LoggerConfig config)) 
                    throw new Exception("Logger config parsing error!");
                this._configLogDirectory = this._configLogDirectory + config.Directory+"/";
                if (!Directory.Exists(this._configLogDirectory)) Directory.CreateDirectory(this._configLogDirectory);
                if (config.LogRotationViaSeconds == 0)
                {
                    this._configLogFile = this._configLogDirectory + "logs.txt";
                    this._configErrorFile = this._configLogDirectory + "errors.txt";
                }
                else
                {
                    DateTime now = DateTime.Now;
                    this._configLogFile = this._configLogDirectory + "logs_" + now.Ticks + ".txt";
                    this._configErrorFile = this._configLogDirectory + "errors_" + now.Ticks + ".txt";
                }
                this._configSecondsToFlush = config.FlushDataViaSeconds;
                this._configSecondsToRotate = config.LogRotationViaSeconds;
                this._typeForLogging = this.parserLogTypes(config.LogTypeForLogging);
                this._typeForConsole = this.parserLogTypes(config.LogTypeForShowInConsole);
                this._typeForImmediatelyFlush = this.parserLogTypes(config.LogTypeForImmediatelyFlush);
            }
            this._writerLog = new StreamWriter(this._configLogFile, true, Encoding.UTF8);
            this._writerError = new StreamWriter(this._configErrorFile, true, Encoding.UTF8);
            this._timer = new Thread(this.timerTick);
            this._timer.Start();
        }

        public void Log(string log, LogType logType)
        {
            lock (this._locker)
            {
                if (this._disposed) return;
                DateTime now = DateTime.Now;
                string message = String.Format("[{1}]<{0}> {2}", logType, now, log);
                if (this._typeForLogging.Contains(logType)) { this._writerLog.WriteLine(message); }
                if (this._typeForConsole.Contains(logType)) Console.WriteLine(message);
                if (this._typeForImmediatelyFlush.Contains(logType)) this.FlushLogs();
                if ((logType == LogType.Error) || (logType == LogType.Critical)) { this._writerError.WriteLine(message); this.flushErrors(); }
                if (logType == LogType.Critical) this.OnCriticalEvent?.Invoke();
            }
            this.OnLogToExternal?.Invoke(log, logType);
        }

        public void RawInfo(string log)
        {
            LogType logType = LogType.InfoRaw;
            string message = log + Environment.NewLine;
            lock (this._locker)
            {                
                if (this._disposed) return;                                
                if (this._typeForLogging.Contains(logType)) { this._writerLog.WriteLine(message); }
                if (this._typeForConsole.Contains(logType)) Console.WriteLine(message);
                if (this._typeForImmediatelyFlush.Contains(logType)) this.FlushLogs();                
            }
            this.OnLogToExternal?.Invoke(message, logType);
        }

        public void Develop(string str) { this.Log(str, LogType.Develop); }
        public void Info(string str) { this.Log(str, LogType.Info); }
        public void Action(string str) { this.Log(str, LogType.Action); }
        public void Warning(string str) { this.Log(str, LogType.Warning); }
        public void Error(string str) { this.Log(str, LogType.Error); }
        public void Error(Exception exception, string str="") { this.Log(str + Environment.NewLine + exception.ToString(), LogType.Error); }
        public void ErrorWithStack(string str) { System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace(); this.Error(str+"["+t.ToString()+"]"); }
        public void Critical(string str) { this.Log(str, LogType.Critical); }

        public void FlushLogs()
        {
            lock (this._locker)
            {
                this._writerLog.Flush();
                this._writerLog.Close();
                this._writerLog = new StreamWriter(this._configLogFile, true, Encoding.UTF8);
            }
        }

        public IList<string> GetLogs(int fromString, int count)
        {
            string[] lines;
            lock (this._locker)
            {                
                this._writerLog.Close();
                lines = File.ReadAllLines(this._configLogFile);
                this._writerLog = new StreamWriter(this._configLogFile, true, Encoding.UTF8);
            }
            int size = lines.Length;
            int pos = size - fromString - count;
            IList<string> result = new List<string>();
            int c = 0;
            for (int i = size - 1; i > pos; i--)
            {
                if (i < 0) continue;
                if (c == count) break;
                c++;
                result.Add(lines[i]);
            }
            return result;
        }

        public void Dispose()
        {
            lock (this._locker) { this._disposed = true; }
        }

        public void flushErrors()
        {
            lock (this._locker)
            {
                this._writerError.Flush();
                this._writerError.Close();
                this._writerError = new StreamWriter(this._configErrorFile, true, Encoding.UTF8);
            }
        }

        private void timerTick()
        {
            int counterFlush = 0;
            int rotationCounter = 0;
            while (!this._disposed)
            {
                Thread.Sleep(1000);
                counterFlush++;
                rotationCounter++;
                if (counterFlush == this._configSecondsToFlush) { counterFlush = 0; this.FlushLogs(); }
                if (this._configSecondsToRotate>0)
                    if (rotationCounter == this._configSecondsToRotate)
                    {
                        rotationCounter = 0;
                        lock (this._locker)
                        {
                            this.FlushLogs();
                            this._writerLog.Close();
                            this._configLogFile = this._configLogDirectory + "logs_" + DateTime.Now.Ticks + ".txt";
                            this._writerLog = new StreamWriter(this._configLogFile, true, Encoding.UTF8);                            
                        }
                    }
            }
            this.FlushLogs();
        }

        private IList<LogType> parserLogTypes(string str)
        {
            IList<LogType> result = new List<LogType>();
            string[] items = str.Split(',');
            foreach (string item in items)
            {
                if (Enum.TryParse(item.Trim(' '), out LogType lt)) result.Add(lt);
            }
            return result;
        }
    }

    public enum LogType    
    {
        Develop=0, //данные используемые при разработке
        Info=1, //информационные данные разного характера, формируются библиотеками
        InfoRaw = 2, //информационные данные без указания даты, удобнов в сихронных потоках
        Action =3, //события, которые происходят уже в основном коде и требуют внимания
        Warning=4, //предупреждения
        Error=5, //ошибки
        Critical=6, //критические ошибки, которые требуют вызова критического делегата.

        
    }
}
