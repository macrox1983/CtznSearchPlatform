using Autofac;
using Prometheus.Infrastructure;

namespace Prometheus.ComponentLoader.DI
{
    public class ComponentManagerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ComponentLoader>().AsImplementedInterfaces().SingleInstance();
        }
    }
}
