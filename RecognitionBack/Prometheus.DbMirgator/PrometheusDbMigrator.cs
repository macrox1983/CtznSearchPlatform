using Microsoft.Extensions.Logging;
using Prometheus.Infrastructure.Component;
using Prometheus.Infrastructure.Component.DbMigration;
using Prometheus.Infrastructure.DbContextBase;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.DbMirgator
{
    internal class PrometheusDbMigrator : IDbMigrator
    {
        private readonly IComponentDbContextFactory<IPrometheusMigrationDbContext> _prometheusDBContextFactory;
        private readonly ILogger _logger;

        public PrometheusDbMigrator(IComponentDbContextFactory<IPrometheusMigrationDbContext> prometheusDBContextFactory, ILoggerFactory loggerFactory)
        {
            _prometheusDBContextFactory = prometheusDBContextFactory ?? throw new ArgumentNullException(nameof(prometheusDBContextFactory));
            _logger = loggerFactory.CreateLogger(nameof(PrometheusDbMigrator));
        }

        public async Task ApplyMigrations()
        {
            _logger.Log(LogLevel.Information, "Применение миграций базы данных...");
            try
            {
                var context = _prometheusDBContextFactory.Create();
                await context.ApplyMigration();
                _logger.Log(LogLevel.Information, "Миграции успешно применены.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в процессе миграции");
            }
            
        }
    }
}
