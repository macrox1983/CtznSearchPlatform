using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BreakingStorm.Common.Loggers;
using BreakingStorm.Common.Helpers;
using BreakingStorm.Common.Extensions;

namespace BreakingStorm.Common.Backups
{
    public interface IBackupSystem : IDisposable
    {
        string DateTimeFormat { get; }
        void MakeBackupFull<T>(string name, Dictionary<string, T> objs, string reasonId = "", string byWho = "");
        void MakeBackupPartical<T>(string name, Dictionary<string, T> objs, string reasonId = "", string byWho = "");
        void MakeBackup(string name, object obj, object id, string reasonId = "", string byWho = "");
        IList<T> LoadBackup<T>(string name, bool justTry);
        IList<BackupContainer<T>> LoadBackupWithShell<T>(string name, bool justTry);
        void ClearBackup(string name, object id, string reasonId = "", string byWho = "");
        void ClearBackupFull(string name);        
    }

    [Serializable]
    public class BackupContainer<T>
    {
        public BackupContainer(string id, T obj, string timeString, string reason, string byWho)
        {
            this.Id = id;
            this.Object = obj;
            this.TimeString = timeString;
            this.Reason = reason;
            this.ByWho = byWho;
        }
        public string Id;
        public T Object;
        public string Reason;
        public string ByWho;
        public string TimeString;
        public bool Delete = false;

        [NonSerialized]
        public DateTime DateTime;
    }

    public class BackupSystem : IBackupSystem
    {
        private readonly Logger _logger;

        private readonly object _locketSystem = new object();

        private readonly string _configDirectoryBackups;
        private readonly string _configDateTimeFormat;

        public string DateTimeFormat { get { return this._configDateTimeFormat; } }

        private readonly Dictionary<string, object> _lockers = new Dictionary<string, object>();
        private object getLocker(string name)
        {
            if (this._lockers.ContainsKey(name)) return this._lockers[name]; else
            {
                lock (this._locketSystem)
                {
                    //проверка в локере
                    if (this._lockers.ContainsKey(name)) return this._lockers[name];
                    this._lockers[name] = new object();
                    return this._lockers[name];
                }
            }
        }

        public BackupSystem(Logger logger = null, string configFile = "")
        {
            this._logger = logger;
            this._configDirectoryBackups = $"{Directory.GetCurrentDirectory()}/backup";
            this._configDateTimeFormat = "yyyy.MM.dd@HH-mm-ss-fffffff#zz";
            if ((configFile != "") && (File.Exists(configFile)))
            {
                if (!HelperSerialization.TryJSONFileDeserialization<BackupConfig>(configFile, out BackupConfig config))
                {
                    logger.Critical(String.Format("BackupSystem config parsing error! Check directory '{0}/{1}' ", Directory.GetCurrentDirectory(), configFile));
                    throw new Exception("BackupSystem config parsing error!");
                }
                this._configDirectoryBackups = Directory.GetCurrentDirectory() + "/" + config.DirectoryBackups;
                this._configDateTimeFormat = config.DateTimeFullFormatString;
            }
        }

        public void MakeBackupFull<T>(string name, Dictionary<string, T> objs, string reasonId = "", string byWho = "")
        {
            string timeString = DateTime.Now.ToString(this._configDateTimeFormat);
            string dir = $"{this._configDirectoryBackups}/{name}/";
            string fileName = $"{dir}/full/{timeString}.json";
            string backup = "";
            try
            {
                lock (this.getLocker(name))
                {
                    Directory.CreateDirectory($"{dir}/full");
                    Directory.CreateDirectory($"{dir}/stacks");
                    string stackFileName = $"{dir}/stack.json";
                    string newStackFileName = $"{dir}/stacks/{timeString}.json";
                    if (File.Exists(stackFileName)) File.Move(stackFileName, newStackFileName);

                    StreamWriter writer = new StreamWriter(fileName, true, Encoding.UTF8);
                    foreach (string id in objs.Keys)
                    {
                        backup = HelperSerialization.JSONSerialization(new BackupContainer<object>(id, objs[id], timeString, reasonId, byWho));
                        writer.WriteLine(backup);
                    }
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception e)
            {
                if (this._logger != null) this._logger.Error(e, String.Format("Error of full making backup '{0}' obj='{1}'.", name, objs.ToJSON()));
                return;
            }
            if (this._logger != null) this._logger.Info(String.Format("Made full backup '{0}'.", name));
        }
        public void MakeBackupPartical<T>(string name, Dictionary<string, T> objs, string reasonId = "", string byWho = "")
        {
            string timeString = DateTime.Now.ToString(this._configDateTimeFormat);
            string dir = $"{this._configDirectoryBackups}/{name}/";
            string fileName = $"{dir}/stack.json";
            string backup = "";
            try
            {
                lock (this.getLocker(name))
                {
                    Directory.CreateDirectory(dir);
                    StreamWriter writer = new StreamWriter(fileName, true, Encoding.UTF8);
                    foreach (string id in objs.Keys)
                    {
                        backup = HelperSerialization.JSONSerialization(new BackupContainer<object>(id, objs[id], timeString, reasonId, byWho));
                        writer.WriteLine(backup);
                    }
                    long size = writer.BaseStream.Length;
                    writer.Flush();
                    writer.Close();
                    if (size > 8000000) //больше 8 мб
                    {
                        Directory.CreateDirectory($"{dir}/stacks");
                        string newFileName = $"{dir}/stacks/{timeString}.json";
                        File.Move(fileName, newFileName);
                    }
                }
            }
            catch (Exception e)
            {
                if (this._logger != null) this._logger.Error(e, String.Format("Error of partical making backup '{0}' obj='{1}'.", name, objs.ToJSON()));
                return;
            }
            if (this._logger != null) this._logger.Info(String.Format("Made partical backup '{0}'.", name));
        }
        /// <summary>
        /// Создать одиночную запись частичного-бэкапа по ид объекта
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="obj">Экземляр объета</param>
        /// <param name="id">Ид объекта</param>
        public void MakeBackup(string name, object obj, object id, string reasonId = "", string byWho = "")
        {
            this.makeBackup(name, obj, id, reasonId, byWho);
        }
        private void makeBackup(string name, object obj, object id, string reasonId = "", string byWho = "", bool delete = false)
        {
            string idStr = id.ToString();
            string timeString = DateTime.Now.ToString(this._configDateTimeFormat);
            string dir = $"{this._configDirectoryBackups}/{name}/";
            string fileName = $"{dir}/stack.json";
            string backup = "";
            try
            {
                BackupContainer<object> bc = new BackupContainer<object>(idStr, obj, timeString, reasonId, byWho);
                bc.Delete = delete;
                backup = HelperSerialization.JSONSerialization(bc);
                lock (this.getLocker(name))
                {

                    Directory.CreateDirectory(dir);
                    StreamWriter writer = new StreamWriter(fileName, true, Encoding.UTF8);
                    long size = writer.BaseStream.Length;
                    writer.WriteLine(backup);
                    writer.Flush();
                    writer.Close();
                    if (size > 8000000) //больше 8 мб
                    {
                        Directory.CreateDirectory($"{dir}/stacks");
                        string newFileName = $"{dir}/stacks/{timeString}.json";
                        File.Move(fileName, newFileName);
                    }
                }
            }
            catch (Exception e)
            {
                if (this._logger != null) this._logger.Error(e, String.Format("Error of making backup '{0}' obj='{1}'.", name, obj.ToJSON()));
                return;
            }
            if (this._logger != null) this._logger.Info(String.Format("Made backup '{0}'.", name));
        }

        /// <summary>
        /// Загружает и создает экземляры всех записей частичного-бэкапа. Возвращая список объектов.
        /// </summary>
        /// <typeparam name="T">Тип объета</typeparam>
        /// <typeparam name="justTry">Если тру то при ошибке загрузки не будет записи в лог</typeparam>
        /// <returns>Список восстановленных объектов</returns>        
        public IList<T> LoadBackup<T>(string name, bool justTry=false)
        {
            return this.LoadBackupWithShell<T>(name, justTry).Select(s => s.Object).ToList();
        }

        /// <summary>
        /// Загрузить частичные бэкапы с подробными данными о последнем бэкапе
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IList<BackupContainer<T>> LoadBackupWithShell<T>(string name, bool justTry = false)
        {
            lock (this.getLocker(name))
            {
                /*
                1 Ищу последний Fullbackup
                2 Составляю список всех стэков, которые происходят после (ищу тот который = fullbackup или 1й который меньше)
                3 Загружаю и парсу фуллбэкап
                4 сверху вниз парсю все стеки.
                5.из результатов создаю фуллбэкап, если изменений по кол-ву больше чем половина размера начального бэкапа
                6 выдаю результаты
                */
                Dictionary<string, BackupContainer<T>> lib = new Dictionary<string, BackupContainer<T>>();
                string dir = $"{this._configDirectoryBackups}/{name}";                
                if (!Directory.Exists(dir)) { if (!justTry) if (this._logger != null) this._logger.Warning(String.Format("Directory of backup '{0}' not found.", name)); return lib.Values.ToList(); }
                if (this._logger != null) this._logger.Info("Loading backup '" + name + "'...");
                DateTime lockTime = new DateTime(0);
                if (Directory.Exists($"{dir}/full"))
                {
                    SortedDictionary<DateTime, string> fullBackupsByTime = new SortedDictionary<DateTime, string>();
                    foreach (string fileName in Directory.GetFiles($"{dir}/full"))
                    {
                        string timeString = Path.GetFileNameWithoutExtension(fileName);
                        if (DateTime.TryParseExact(timeString, this._configDateTimeFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
                            fullBackupsByTime.Add(dateTime, fileName);
                    }
                    if (fullBackupsByTime.Count > 0)
                    {
                        lockTime = fullBackupsByTime.Last().Key;
                        List<BackupContainer<T>> list = this.loadBackupContainers<T>(fullBackupsByTime.Last().Value, name);
                        foreach (BackupContainer<T> bc in list) lib[bc.Id] = bc;
                        list.Clear();
                    }
                }

                Dictionary<DateTime, string> stacks = new Dictionary<DateTime, string>();                
                DateTime preLockTime = lockTime;
                if (Directory.Exists($"{dir}/stacks"))
                    foreach (string fileName in Directory.GetFiles($"{dir}/stacks"))
                    {
                        string timeString = Path.GetFileNameWithoutExtension(fileName);
                        if (DateTime.TryParseExact(timeString, this._configDateTimeFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
                        {
                            if (dateTime > preLockTime)
                                if (dateTime < lockTime) preLockTime = dateTime;
                            stacks.Add(dateTime, fileName);
                        }
                    }
                Dictionary<DateTime, string> checkStacks = stacks.Keys.Where(k => k > lockTime).ToDictionary(k => k, k => stacks[k]);
                if (File.Exists($"{dir}/stack.json")) checkStacks.Add(DateTime.Now, $"{dir}/stack.json");
                if (stacks.ContainsKey(lockTime)) checkStacks.Add(lockTime, stacks[lockTime]);
                if (lockTime!=preLockTime)
                    if (stacks.ContainsKey(preLockTime)) checkStacks.Add(preLockTime, stacks[preLockTime]);
                bool fullBackupChanged = false;
                int changesCount = 0;
                int inFullCount = lib.Count;
                foreach (string fileName in checkStacks.Values)
                {
                    List<BackupContainer<T>> list = this.loadBackupContainers<T>(fileName, name);
                    foreach (BackupContainer<T> bc in list)
                    {
                        if (lib.TryGetValue(bc.Id, out BackupContainer<T> existBC))
                        {
                            if (existBC.DateTime > bc.DateTime) continue;
                            lib[bc.Id] = bc;                            
                            fullBackupChanged = true;
                            changesCount++;
                        }
                        else
                        {
                            lib[bc.Id] = bc;
                            fullBackupChanged = true;
                            changesCount++;
                        }
                    }
                }
                foreach (BackupContainer<T> bc in lib.Values.Where(v => v.Delete).ToList()) lib.Remove(bc.Id);
                if (fullBackupChanged)
                    if (inFullCount/2<changesCount)
                        this.MakeBackupFull<T>(name, lib.ToDictionary(k => k.Key, v => v.Value.Object), "LoadingBackup");
                return lib.Values.ToList();               
            }
        }

        private List<BackupContainer<T>> loadBackupContainers<T>(string fileName, string backupName)
        {
            List<BackupContainer<T>> result = new List<BackupContainer<T>>();
            StreamReader reader = new StreamReader(fileName, Encoding.UTF8);
            try
            {
                reader.BaseStream.Position = 0;
                while (!reader.EndOfStream)
                {
                    string json = reader.ReadLine();
                    BackupContainer<object> res = HelperSerialization.JSONDeserialization<BackupContainer<object>>(json);
                    BackupContainer<T> clearRes = new BackupContainer<T>(res.Id, default, res.TimeString, res.Reason, res.ByWho);
                    clearRes.Delete = res.Delete;
                    if (HelperSerialization.TryJSONDeserialization<T>(res.Object.ToJSON(), out T typedObj)) clearRes.Object = typedObj;
                    DateTime.TryParseExact(clearRes.TimeString, this._configDateTimeFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime bcDateTime);
                    clearRes.DateTime = bcDateTime;
                    result.Add(clearRes);
                    res = null;
                }
                reader.Close();
                reader.Dispose();
            }
            catch (Exception e)
            {
                if (this._logger != null) this._logger.Error(e, String.Format("Load backup error.'{0}'", backupName));
                reader.Close();
                reader.Dispose();
            }            
            return result;
        }

        /// <summary>
        /// Помечает объект по ид как удаленный, он более не будет загружаться из бэкапа
        /// </summary>        
        /// <param name="id">Ид объекта</param>
        public void ClearBackup(string name, object id, string reasonId = "", string byWho = "")
        {
            this.makeBackup(name, new object(), id, reasonId, byWho, true);            
        }

        /// <summary>
        /// Удаляет все записи бэкапа по типу объекта
        /// </summary>
        public void ClearBackupFull(string name)
        {            
            string dir = $"{this._configDirectoryBackups}/{name}";
            if (this._logger != null) this._logger.Info(String.Format("Full clear of backup '{0}'.", name));
            lock (this.getLocker(name))
            {
                if (!Directory.Exists(dir)) return;
                Directory.Delete(dir, true);
            }
        }

        public void Dispose()
        {
           
        }
    }
}

