using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Prometheus.Infrastructure.Component.DbConfigurator;

namespace Prometheus.Infrastructure.Component.DbMigration
{
    public class ComponentSqlGenerator : MigrationsSqlGenerator
    {
        private readonly IEfDbConfigurator _dbConfigurator;
        private readonly IMigrationsSqlGenerator _currentMigrationsSqlGenerator;

        public ComponentSqlGenerator(IEfDbConfigurator dbConfigurator, MigrationsSqlGeneratorDependencies dependencies, IMigrationsAnnotationProvider migrationsAnnotations) : base(dependencies)//, migrationsAnnotations)
        {
            _dbConfigurator = dbConfigurator;
            if(_dbConfigurator.GetSpecifiedMigrationSqlGeneratorType()!=null)
                _currentMigrationsSqlGenerator = (IMigrationsSqlGenerator)Activator.CreateInstance(_dbConfigurator.GetSpecifiedMigrationSqlGeneratorType(), dependencies, migrationsAnnotations);
        }

        public override IReadOnlyList<MigrationCommand> Generate(IReadOnlyList<MigrationOperation> operations, IModel model = null)
        {
            var commands = base.Generate(operations, model);
            var sql = commands.Aggregate("", (s, s1) => { return s + s1.CommandText; });
            return commands;
        }

        protected override void Generate(MigrationOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            if (operation is AddColumnIfNotExistsOperation columnIfNotExistsOperation)
            {
                Generate(columnIfNotExistsOperation, model, builder);
            }
            else if(_currentMigrationsSqlGenerator !=null && operation is AlterColumnOperation alterColumnOperation)
            {
                var method = _currentMigrationsSqlGenerator
                    .GetType()
                    .GetMethod("Generate", BindingFlags.Instance|BindingFlags.NonPublic, Type.DefaultBinder,
                                new[] { typeof(AlterColumnOperation), typeof(IModel), typeof(MigrationCommandListBuilder) }, null);
                method?.Invoke(_currentMigrationsSqlGenerator, new object[] { alterColumnOperation, model, builder });
            }
            else
                base.Generate(operation, model, builder);
        }

        private void Generate(AddColumnIfNotExistsOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            var sqlHelper = Dependencies.SqlGenerationHelper;            
            var stringMapping = Dependencies.TypeMappingSource.FindMapping(typeof(string));

            builder.Append("If not exists(select 1 from sys.columns where name = ");
            builder.Append(stringMapping.GenerateSqlLiteral(operation.Name));
            builder.Append(" and  object_id = OBJECT_ID(");
            builder.Append(stringMapping.GenerateSqlLiteral(operation.Table));
            builder.AppendLine("))");
            builder.AppendLine("begin");
            builder.Append("Alter table ");
            builder.Append(sqlHelper.DelimitIdentifier(operation.Table));
            builder.Append(" add ");
            base.ColumnDefinition(operation, model, builder);
            builder.AppendLine("");
            builder.Append("end");
            builder.Append(sqlHelper.StatementTerminator);
            builder.EndCommand();
        }
    }
}
