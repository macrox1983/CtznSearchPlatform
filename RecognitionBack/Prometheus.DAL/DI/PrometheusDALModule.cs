using Autofac;
using Prometheus.Infrastructure.Component;
using Prometheus.Infrastructure.DbContextBase;
using Prometheus.Infrastructure.RepositoryBase;

namespace Prometheus.DAL.DI
{
    public class PrometheusDALModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {           
            builder.RegisterType<UserRepository>().As<IUserRepository>();

            builder.Register(context =>
            {
                var cache = new CityCache();
                cache.InitialCache(context.Resolve<IComponentDbContextFactory<IPrometheusDbContext>>());
                return cache;
            }).SingleInstance().AsImplementedInterfaces();
            builder.RegisterType<CityRepository>().As<ICityRepository>();            

            builder.RegisterType<ConstantRepository>().As<IConstantRepository>();
        }
    }
}