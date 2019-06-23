using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Migrations;
using Prometheus.Infrastructure.Component.DbConfigurator;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Prometheus.Infrastructure.Component.DbMigration
{

    public static class MigrationExtensions
    {
        public static string GetId(this Migration migration)
        {
            return migration.GetType().GetCustomAttribute<ComponentDbMigrationAttribute>()?.Id;
        }
    }

    public abstract class ComponentDbMigration : Migration
    {
        private readonly IEfDbConfigurator _databaseConfiguration;

        public ComponentDbMigration(IEfDbConfigurator databaseConfiguration)
        {
            _databaseConfiguration = databaseConfiguration ?? throw new ArgumentNullException(nameof(databaseConfiguration));
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            _databaseConfiguration.ConfigureDatabaseMigration(migrationBuilder);

            UpImpl(migrationBuilder);
        }

        protected abstract void UpImpl(MigrationBuilder migrationBuilder);
    }
}
