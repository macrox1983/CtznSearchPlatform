using Autofac;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.MigrationAssembly.DI
{
    public class PrometheusMigrationAssemblyModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PrometheusMigrationAssembly>().As<MigrationsAssembly>().As<IMigrationsAssembly>();
        }
    }
}
