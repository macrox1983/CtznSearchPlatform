using FaceRecognitionDotNet;
using Microsoft.Extensions.Logging;
using Prometheus.CtznSearch.ApiService.DataContext;
using Prometheus.CtznSearch.ApiService.Options;
using Prometheus.Infrastructure.Component;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.CtznSearch.ApiService.HostedServices
{
    public class CtznSearchRecognitionHostedService : ComponentHostedService
    {
        private bool _pause;
        private bool _stop;
        private readonly CtznSearchApiServiceOptions _options;
        private readonly IComponentDbContextFactory<CtznSearchDbContext> _dbContextFactory;
        private readonly ILogger _logger;


        private FileSystemWatcher _fileWatcher;
        private FaceRecognition _faceRecognition;
        private ConcurrentDictionary<int, ImageForDetect> _searchTicketImages;
        private string _executingDirectory;
        private string _tempDirectory;

        private ConcurrentQueue<ImageForDetect> _imagesForDetect;

        private Task _analyzeImagesTask;
        private Task _addSearchImageFromDb;              


        public CtznSearchRecognitionHostedService(CtznSearchApiServiceOptions options, IComponentDbContextFactory<CtznSearchDbContext> dbContextFactory, ILoggerFactory loggerFactory)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _dbContextFactory = dbContextFactory;
            _logger = loggerFactory.CreateLogger(nameof(CtznSearchRecognitionHostedService));
        }

        public override string GetState()
        {
            return _pause ? "paused" : _stop ? "stoped" : "worked";
        }

        public override string ServiceVersion()
        {
            return "1.1";
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _stop = _pause = false;
            _executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _tempDirectory = $"{_executingDirectory}\\temp";
            //if (!Directory.Exists(_tempDirectory))
                Directory.CreateDirectory(_tempDirectory);

            string directory = $"{_executingDirectory}\\Dlib_models";

            string directoryForSearch = _options.SearchTicketImagesFolder;

            int cpus = -1;

            FaceRecognitionDotNet.Model model = FaceRecognitionDotNet.Model.Hog;

            if (Directory.Exists(directory))
            {
                _faceRecognition = FaceRecognition.Create(directory);
                _logger.LogInformation("Модуль распознавания загружен...");
                _searchTicketImages = new ConcurrentDictionary<int, ImageForDetect>();

                _imagesForDetect = new ConcurrentQueue<ImageForDetect>();

                var files = Directory.GetFiles(directoryForSearch);

                for (int i = 0; i < files.Count(); i++)
                {
                    var file = files[i];
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var id = Convert.ToInt32(fileName);
                    var img = FaceRecognition.LoadImageFile(file, Mode.Greyscale);
                    _searchTicketImages.TryAdd(id, new ImageForDetect { Image = img, SearchTicketId = id });
                }
                _logger.LogInformation($"Загружено пропавших для поиска [{_searchTicketImages.Count}]");

                _fileWatcher = new FileSystemWatcher(directoryForSearch);
                _fileWatcher.Created += _fileWatcher_Created;
                _fileWatcher.EnableRaisingEvents = true;

                //_analyzeImagesTask = Task.Run(() => AnalyzeImages());
                //_addSearchImageFromDb = Task.Run(()=> AddSearchImageFromDbToQueue());

            }
        }

        private void _fileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            try
            {
                var fileName = Path.GetFileNameWithoutExtension(e.FullPath);
                var id = Convert.ToInt32(fileName);
                var img = FaceRecognition.LoadImageFile(e.FullPath, Mode.Greyscale);
                if (!_searchTicketImages.ContainsKey(id))
                {
                    _searchTicketImages.TryAdd(id, new ImageForDetect { Image = img, SearchTicketId = id });
                    _logger.LogInformation($"Добавлен пропавший id[{id}]...");
                }
                _logger.LogInformation($"Загружено пропавших для поиска [{_searchTicketImages.Count}]");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка добавления фото пропавшего в список поиска");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _stop = true;
        }

        public override async Task PauseAsync()
        {
            _pause = true;
        }

        public async Task<bool> Detect(byte[] image, string fileType, int ticketId)
        {
            var tmpFile = $"{_tempDirectory}\\{ticketId}{fileType}";
            File.WriteAllBytes(tmpFile, image);

            ImageForDetect imageForSearch;
            if(_searchTicketImages.TryGetValue(ticketId, out imageForSearch))
            {
                var faceImage = FaceRecognition.LoadImageFile(tmpFile);
                File.Delete(tmpFile);
                var facesForCheck = _faceRecognition.FaceEncodings(faceImage);
                foreach(var face in facesForCheck)
                {
                    if (FaceRecognition.CompareFaces(_faceRecognition.FaceEncodings(imageForSearch.Image), face).Any(res=>res))
                    {
                        _logger.LogInformation($"Пропавший id[{ticketId}] найден.");
                        return true;
                    }
                }                
            }
            return false;
        }


        //private async void AnalyzeImages()
        //{            
        //    while (true)
        //    {
        //        if(!_stop&&!_pause)
        //        {
        //            ImageForDetect image;

        //            if (_imagesForDetect.TryDequeue(out image))
        //            {
        //                var detectedFaces = _faceRecognition.FaceEncodings(image.Image);

        //                if (detectedFaces != null)
        //                {
        //                    var list = _searchTicketImages.ToList();
        //                    for (int i = 0; i < list.Count; i++)
        //                    {
        //                        var kvp = list[i];
        //                        foreach (var detectedFace in detectedFaces)
        //                        {
        //                            var face = _faceRecognition.FaceEncodings(kvp.Value.Image).FirstOrDefault();
        //                            if (face != null && FaceRecognition.CompareFace(face, detectedFace))
        //                            {
        //                                using (var context = _dbContextFactory.Create())
        //                                {
        //                                    var detectHistory = new Model.FaceRecognitionHistory
        //                                    {
        //                                        FaceRecognitionHistoryId = Guid.NewGuid(),
        //                                        SearchTicketId = kvp.Key,
        //                                        Message = $"{kvp.Value.LostPersonName} обнаружен"
        //                                    };
        //                                    context.FaceRecognitionHistory.Add(detectHistory);
        //                                    await context.SaveChangesAsync();
        //                                }
        //                            }
        //                        }
        //                    }                        
        //                }
        //            }
        //        }                
        //    }
        //}

        //private void AddSearchImageFromDbToQueue()
        //{
        //    using (var context = _dbContextFactory.Create())
        //    {
        //        while (true)
        //        {
        //            if (!_stop && !_pause)
        //            {
        //                var newTickets = context.SearchTicket.Where(st=>st.SearchTicketStatus == (int)Model.SearchTicketStatuses.Active).ToList();
        //                foreach(var ticket in newTickets)
        //                {
        //                    System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(ticket.LostPersonPhoto));
        //                    System.Drawing.Bitmap main = new System.Drawing.Bitmap(image);
        //                    _searchTicketImages.TryAdd(ticket.SearchTicketId, new ImageForDetect
        //                    {
        //                        SearchTicketId = ticket.SearchTicketId,
        //                        LostPersonName = ticket.LostPersonName,
        //                        Image = FaceRecognition.LoadImage(ticket.LostPersonPhoto, main.Height, main.Width,ticket.LostPersonPhoto.Length)
        //                    });
        //                }
        //            }
        //        }
        //    }
        //}

        //public void AddImageForSearch(byte[] image, Guid ticketId, string name)
        //{
           
        //    _searchTicketImages.TryAdd(ticketId, new ImageForDetect
        //    {                
        //        SearchTicketId = ticketId,
        //        LostPersonName = name,
        //        Image = FaceRecognition.LoadImage(image, main.Height, main.Width, image.Length)
        //    });
        //}

        //public void AddImageForDetect(byte[] image, Guid ticketId, string name)
        //{            
        //    _imagesForDetect.Enqueue(new ImageForDetect
        //    {
        //        SearchTicketId = ticketId,
        //        LostPersonName = name,
        //        Image = FaceRecognition.LoadImage(image, main.Height, main.Width, image.Length)
        //    });
        //}
    }

    public class ImageForDetect
    {
        public int SearchTicketId { get; set; }

        public string LostPersonName { get; set; }

        public Image Image { get; set; }
    }
}
