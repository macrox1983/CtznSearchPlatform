using System.Threading.Tasks;

namespace Prometheus.Infrastructure.Component.DbMigration
{
    public interface IDbMigrator
    {
        Task ApplyMigrations();
    }
}