using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BreakingStorm.Common.Backups
{
    [Serializable]
    public class BackupConfig
    {        
        public string DirectoryBackups { get; set; }
        public string DirectoryTrackers { get; set; }        
        public string DateTimeFullFormatString { get; set; }
    }
}
