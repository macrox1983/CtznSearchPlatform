using Autofac;
using System;

namespace Prometheus.Infrastructure.Component
{
    internal class DbContextFactory<TDataContext> : IComponentDbContextFactory<TDataContext>
    {
        private readonly IComponentContext _componentContext;

        public DbContextFactory(IComponentContext componentContext)
        {
            _componentContext = componentContext ?? throw new ArgumentNullException(nameof(componentContext));
        }

        public TDataContext Create()
        {
            return _componentContext.Resolve<TDataContext>();
        }

        public TDataContext GetDbContext()
        {
            return Create();
        }
    }
}
