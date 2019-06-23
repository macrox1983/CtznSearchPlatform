namespace Prometheus.Infrastructure.Component
{
    public interface IComponentDbContextFactory<TDataContext>
    {
        TDataContext Create();
        TDataContext GetDbContext();
    }
}
