using Autofac;

namespace Prometheus.DbContext.DI
{
    public class DbContextModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PrometheusDbContext>().AsImplementedInterfaces();
        }
    }
}
