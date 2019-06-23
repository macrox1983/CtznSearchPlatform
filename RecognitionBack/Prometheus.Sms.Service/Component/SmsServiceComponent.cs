using Prometheus.Infrastructure.Component;
using Autofac;
using Prometheus.Sms.Service.Abstractions;
using Prometheus.Sms.Service.Options;
using Prometheus.Sms.Service.HostedServices;
using Prometheus.Sms.Service.SmsApiClient;

namespace Prometheus.Sms.Service.Component
{
    public class SmsServiceComponent : Infrastructure.Component.Component
    {
        public override string ComponentName => "SmsService";

        public override string ComponentDescription => "Модуль информирования через sms";

        protected override void ConfigureComponent(IComponentBuilder builder)
        {
            builder.RegisterOptions<SmsServiceOptions>();
            builder.RegisterDependencies(containerBuilder =>
            {
                containerBuilder.RegisterType<SmsClient>().As<ISmsClient>();
            })
            .RegisterHostedService<SmsHostedService, SmsServiceComponent>();
        }
    }
}
