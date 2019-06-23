using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prometheus.Infrastructure.Component.DbMigration
{
    public class ComponentDbMigrator : Migrator
    {
        private IMigrationsAssembly _migrationsAssembly;
        private IHistoryRepository _historyRepository;
        private ISqlGenerationHelper _sqlGenerationHelper;
        private IDiagnosticsLogger<DbLoggerCategory.Migrations> _logger;
        private IRawSqlCommandBuilder _rawSqlCommandBuilder;
        private IMigrationsSqlGenerator _migrationsSqlGenerator;

        public ComponentDbMigrator(IMigrationsAssembly migrationsAssembly, IHistoryRepository historyRepository, 
            IDatabaseCreator databaseCreator, IMigrationsSqlGenerator migrationsSqlGenerator, 
            IRawSqlCommandBuilder rawSqlCommandBuilder, IMigrationCommandExecutor migrationCommandExecutor, 
            IRelationalConnection connection, ISqlGenerationHelper sqlGenerationHelper, IDiagnosticsLogger<DbLoggerCategory.Migrations> logger, 
            IDatabaseProvider databaseProvider) :
            base(migrationsAssembly, historyRepository, databaseCreator, migrationsSqlGenerator, rawSqlCommandBuilder, migrationCommandExecutor, connection, sqlGenerationHelper, logger, databaseProvider)
        {
            _migrationsAssembly = migrationsAssembly;
            _historyRepository = historyRepository;
            _sqlGenerationHelper = sqlGenerationHelper;
            _logger = logger;
            _rawSqlCommandBuilder = rawSqlCommandBuilder;
            _migrationsSqlGenerator = migrationsSqlGenerator;
        }

        public override string GenerateScript(string fromMigration = null, string toMigration = null, bool idempotent = false)
        {
            IEnumerable<string> appliedMigrations;
            if (string.IsNullOrEmpty(fromMigration)
                || fromMigration == Migration.InitialDatabase)
            {
                appliedMigrations = Enumerable.Empty<string>();
            }
            else
            {
                var fromMigrationId = _migrationsAssembly.GetMigrationId(fromMigration);
                appliedMigrations = _migrationsAssembly.Migrations
                    .Where(t => string.Compare(t.Key, fromMigrationId, StringComparison.OrdinalIgnoreCase) <= 0)
                    .Select(t => t.Key);
            }

            PopulateMigrations(
                appliedMigrations,
                toMigration,
                out var migrationsToApply,
                out var migrationsToRevert,
                out var actualTargetMigration);

            var builder = new IndentedStringBuilder();

            if (fromMigration == Migration.InitialDatabase
                || string.IsNullOrEmpty(fromMigration))
            {
                builder.AppendLine(_historyRepository.GetCreateIfNotExistsScript());
                builder.Append(_sqlGenerationHelper.BatchTerminator);
            }

            for (var i = 0; i < migrationsToRevert.Count; i++)
            {
                var migration = migrationsToRevert[i];
                var previousMigration = i != migrationsToRevert.Count - 1
                    ? migrationsToRevert[i + 1]
                    : actualTargetMigration;

                _logger.MigrationGeneratingDownScript(this, migration, fromMigration, toMigration, idempotent);

                foreach (var command in GenerateDownSql(migration, previousMigration))
                {
                    if (idempotent)
                    {
                        builder.AppendLine(_historyRepository.GetBeginIfExistsScript(migration.GetId()));
                        using (builder.Indent())
                        {
                            builder.AppendLines(command.CommandText);
                        }

                        builder.AppendLine(_historyRepository.GetEndIfScript());
                    }
                    else
                    {
                        builder.AppendLine(command.CommandText);
                    }

                    builder.Append(_sqlGenerationHelper.BatchTerminator);
                }
            }

            foreach (var migration in migrationsToApply)
            {
                _logger.MigrationGeneratingUpScript(this, migration, fromMigration, toMigration, idempotent);

                foreach (var command in GenerateUpSql(migration))
                {
                    if (idempotent)
                    {
                        builder.AppendLine(_historyRepository.GetBeginIfNotExistsScript(migration.GetId()));
                        using (builder.Indent())
                        {
                            builder.AppendLines(command.CommandText);
                        }

                        builder.AppendLine(_historyRepository.GetEndIfScript());
                    }
                    else
                    {
                        builder.AppendLine(command.CommandText);
                    }

                    builder.Append(_sqlGenerationHelper.BatchTerminator);
                }
            }

            return builder.ToString();
        }

        protected override IReadOnlyList<MigrationCommand> GenerateUpSql(Migration migration)
        {
            if(migration == null)
                throw new ArgumentNullException(nameof(migration));

            var insertCommand = _rawSqlCommandBuilder.Build(
                _historyRepository.GetInsertScript(new HistoryRow(migration.GetId(), ProductInfo.GetVersion())));

            return _migrationsSqlGenerator
                .Generate(migration.UpOperations, migration.TargetModel)
                .Concat(new[] { new MigrationCommand(insertCommand) })
                .ToList();

        }

        protected override IReadOnlyList<MigrationCommand> GenerateDownSql( Migration migration, Migration previousMigration)
        {
            if (migration == null)
                throw new ArgumentNullException(nameof(migration));

            var deleteCommand = _rawSqlCommandBuilder.Build(
                _historyRepository.GetDeleteScript(migration.GetId()));

            return _migrationsSqlGenerator
                .Generate(migration.DownOperations, previousMigration?.TargetModel)
                .Concat(new[] { new MigrationCommand(deleteCommand) })
                .ToList();
        }
    }
}
