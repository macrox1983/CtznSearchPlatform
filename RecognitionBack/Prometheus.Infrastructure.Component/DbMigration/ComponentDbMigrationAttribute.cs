using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Prometheus.Infrastructure.Component.DbMigration
{
    public class ComponentDbMigrationAttribute : Attribute
    {
        public ComponentDbMigrationAttribute(Type dbContextType, string id, string version)
        {
            if (!new Regex("^2[0-9][1-9][0-9][0,1][0-9][0-3][0-9]_.+").IsMatch(id))
            {
                throw new ArgumentException("Идентификатор миграции в атрибуте" +
                    " 'ComponentDbMigrationAttribute' должен соответствовать  паттерну yyyyMMdd_Наименование", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentException("Версия ПО, с которой производить данную миграцию обязательно должна быть указана в атрибуте 'ComponentDbMigrationAttribute'", nameof(version));
            }

            Id = id;
            Version = version;
            DbContextType = dbContextType ?? throw new ArgumentNullException(nameof(dbContextType));
        }

        public string Id { get; }

        public string Version { get; }

        public Type DbContextType { get; }
    }
}
