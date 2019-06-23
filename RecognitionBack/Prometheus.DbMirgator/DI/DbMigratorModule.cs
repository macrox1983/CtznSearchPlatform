using Autofac;
using Prometheus.Infrastructure.Component.DbMigration;

namespace Prometheus.DbMirgator.DI
{
    public class DbMigratorModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PrometheusDbMigrator>().As<IDbMigrator>();
        }
    }
}
