using Autofac;
using Prometheus.CtznSearch.ApiService.DataContext;
using Prometheus.CtznSearch.ApiService.HostedServices;
using Prometheus.CtznSearch.ApiService.Options;
using Prometheus.Infrastructure.Component;

namespace Prometheus.CtznSearch.ApiService.Component
{
    public class CtznApiServiceСomponent : Infrastructure.Component.Component
    {
        public override string ComponentName => "CtznSearchApiService";

        public override string ComponentDescription => "Сервис, предоставляющий API Ctzn Search платформы";

        protected override void ConfigureComponent(IComponentBuilder builder)
        {
            builder
                .RegisterOptions<CtznSearchApiServiceOptions, CtznSearchApiServiceConfigureOptions>()
                //.RegisterDependencies(containerBuilder =>
                //{
                //    containerBuilder.RegisterType<BRSController>().InstancePerRequest();
                //})
                .RegisterDbContext<CtznSearchApiServiceOptions, CtznSearchDbContext>()
                .RegisterHostedService<CtznSearchRecognitionHostedService, CtznApiServiceСomponent>();
        }
    }
}
