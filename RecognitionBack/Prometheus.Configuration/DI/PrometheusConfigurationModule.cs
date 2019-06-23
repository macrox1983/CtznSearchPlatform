using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Prometheus.Infrastructure.Configuration;

namespace Prometheus.Configuration.DI
{
    public class PrometheusConfigurationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PrometheusConfiguratorWithMigration>().SingleInstance().AsImplementedInterfaces();

            builder.Register(c => c.Resolve<IOptionsMonitor<DbConfiguration>>().CurrentValue);

            builder.Register(c => c.Resolve<IPrometheusConfigurator>().GetDbConfigurator()).SingleInstance().As<IPrometheusDbConfigurator>();

            builder.Register(c => c.Resolve<IPrometheusConfiguratorWithDbMigration>().GetDbMigrationConfigurator()).SingleInstance().As<IPrometheusDbConfiguratorWithMigration>();

            builder.Register(context =>
            {
                var dbContextOptionBuilder = new DbContextOptionsBuilder();
                context.Resolve<IPrometheusDbConfigurator>().ConfigureDatabase(dbContextOptionBuilder);
                return new PrometheusDbContextOptions(dbContextOptionBuilder.Options);
            }).SingleInstance();
        }
    }
}
