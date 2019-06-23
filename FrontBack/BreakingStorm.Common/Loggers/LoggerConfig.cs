using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BreakingStorm.Common.Loggers
{
    [Serializable]
    public class LoggerConfig
    {        
        public string Directory;
        //<!-- 0: never, records in one file -->
        public uint LogRotationViaSeconds;
        //<!-- Levels: Develop, Info, Action, Warning, Error, Critical-->
        public string LogTypeForLogging;
        public string LogTypeForShowInConsole;
        public string LogTypeForImmediatelyFlush;
        public uint FlushDataViaSeconds;
    }
}
