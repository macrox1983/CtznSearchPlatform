using Autofac;
using AutoMapper;
using AutoMapper.Configuration;
using AutoMapper.QueryableExtensions;
using Prometheus.DAL.DI;
using Prometheus.Model;
using Prometheus.Presentation;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Prometheus.Services.DI
{
    public class PrometheusServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<PrometheusDALModule>();
            builder.RegisterType<AuthService>().As<IAuthService>();
            builder.RegisterType<DictionaryAggregateCrudService>().As<IDictionaryAggregateCrudService>();           
            builder.RegisterType<CityCrudService>().As<IEntityCrudService<CityVm, Guid>>();
            builder.RegisterType<UserCrudService>().As<IEntityCrudService<ApplicationUserVm, Guid>>();

            builder.Register(context =>
            {
                var container = context.Resolve<ILifetimeScope>();
                // Конфигурация Automapper для таблиц справочников
                Mapper.Initialize(config =>
                {
                    config.ConstructServicesUsing(container.Resolve);                                                          

                    config.CreateMap<ApplicationUser, ApplicationUserVm>();
                    config.CreateMap<ApplicationUserVm, ApplicationUser>();                   

                    config.CreateMap<City, CityVm>();
                    config.CreateMap<CityVm, City>();                   
                });
                return Mapper.Configuration.CreateMapper();
            }).SingleInstance();
        }
    }
}
