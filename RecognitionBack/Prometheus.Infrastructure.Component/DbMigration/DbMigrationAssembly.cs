using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Prometheus.Infrastructure.Component.DbMigration
{

    public class DbMigrationAssembly : MigrationsAssembly
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly LazyRef<IReadOnlyDictionary<string, TypeInfo>> _migrations;

        public override IReadOnlyDictionary<string, TypeInfo> Migrations => _migrations.Value;

        public DbMigrationAssembly(ICurrentDbContext currentContext,
            IDbContextOptions options,
            IMigrationsIdGenerator idGenerator,
            IDiagnosticsLogger<DbLoggerCategory.Migrations> logger,
            IServiceProvider serviceProvider)
            : base(currentContext, options, idGenerator, logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            var contextType = currentContext.Context.GetType();

            _migrations = new LazyRef<IReadOnlyDictionary<string, TypeInfo>>(
            () =>
            {
                var result = new SortedList<string, TypeInfo>();
                var items =
                    from t in Assembly.GetExportedTypes().Select(t => t.GetTypeInfo())
                    let attr = t.GetCustomAttribute<ComponentDbMigrationAttribute>()
                    where t.IsSubclassOf(typeof(Migration))
                          && (attr?.DbContextType == contextType 
                          || (bool)attr?.DbContextType.GUID.Equals(contextType.BaseType.GUID))
                    let id = attr?.Id
                    orderby id
                    select (id, t);
                foreach ((string id, TypeInfo t) in items)
                {
                    if (id == null)
                    {
                        logger.MigrationAttributeMissingWarning(t);

                        continue;
                    }

                    result.Add(id, t);
                }                
                return result;
            });
        }

        public override Migration CreateMigration(TypeInfo migrationClass, string activeProvider)
        {
            var migration = (Migration)ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, migrationClass.AsType());
            return migration ?? throw new Exception($"Миграция {migrationClass.Name} не зарегистрирована в контейнере зависимостей");
        }
    }
}
