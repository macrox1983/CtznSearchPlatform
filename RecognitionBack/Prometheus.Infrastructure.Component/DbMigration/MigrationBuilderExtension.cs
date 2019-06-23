using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Prometheus.Infrastructure.Component.DbMigration
{

    public static class MigrationBuilderExtension
    {
        public static MigrationBuilder AddColumnIfNotExists<T>(this MigrationBuilder migrationBuilder, string name, string table, string type = null, bool? unicode = null, int? maxLength = null, bool rowVersion = false, string schema = null, bool nullable = false, object defaultValue = null, string defaultValueSql = null, string computedColumnSql = null, bool? fixedLength = null)
        {
            migrationBuilder.Operations.Add(new AddColumnIfNotExistsOperation()
            {
                Name = name,
                Table = table,
                ColumnType = type,
                ClrType = typeof(T),
                IsUnicode = unicode,
                MaxLength = maxLength,
                IsRowVersion = rowVersion,
                Schema = schema,
                IsNullable = nullable,
                DefaultValue = defaultValue,
                DefaultValueSql = defaultValueSql,
                ComputedColumnSql = computedColumnSql,
                IsFixedLength = fixedLength
            });
            return migrationBuilder;
        }
    }
}
