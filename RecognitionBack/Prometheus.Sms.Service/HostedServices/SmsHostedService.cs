using Prometheus.Infrastructure.Component;
using Prometheus.Infrastructure.Component.BackgroundWorker;
using Prometheus.Sms.Service.Abstractions;
using Prometheus.Sms.Service.DataModel;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.Sms.Service.HostedServices
{
    public class SmsHostedService : ComponentHostedService
    {
        private bool _pause;
        private bool _stop;
        private bool _initialized;
        private readonly ISmsClient _smsClient;
        private MockDataRepository _repository;
        private Task _AddingToQueueTask;
        private Task _SendSmsTask;

        private ConcurrentQueue<SmsInfo> _queue;

        public SmsHostedService(ISmsClient smsClient)
        {
            _queue = new ConcurrentQueue<SmsInfo>();
            _repository = new MockDataRepository();
            _smsClient = smsClient;
        }

        public override string GetState()
        {
            return $"{(_pause?"paused":_stop?"stoped":"worked")}";
        }

        public override string ServiceVersion()
        {
            return "1.0";
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            //AddingToQueue();            
            if(_stop || !_initialized)
            {
                _AddingToQueueTask = BackgroundWorker.StartEndlessWork(() => _stop, () => _pause, AddingToQueue);
                _SendSmsTask = BackgroundWorker.StartEndlessWork(() => _stop, () => _pause, SendSms);
                _stop = false;
                _initialized = true;
            }
            _pause = false;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {            
            _stop = true;
        }

        private void AddingToQueue()
        {
            var smsInfos = _repository.GetSmsInfos();
            foreach(var info in smsInfos)
            {
                _queue.Enqueue(info);
            }
        }

        private void SendSms()
        {
            SmsInfo info;
            if (_queue.TryDequeue(out info))
            {
                _smsClient.SendSms(info);
            }
        }

        public override async Task PauseAsync()
        {
            _pause = true;
        }
    }
}
