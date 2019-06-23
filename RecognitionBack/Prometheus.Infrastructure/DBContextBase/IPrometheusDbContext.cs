using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Prometheus.Infrastructure.Component.DbContext;
using Prometheus.Model;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.Infrastructure.DbContextBase
{

    public class ForeignKeyReferenceValueExtractor<TDependencyDbEntity, TDependencyKey> where TDependencyDbEntity : class
    {

        public ForeignKeyReferenceValueExtractor(string foreignKeyName, Func<TDependencyDbEntity,bool> where)
        {
            ForeignKeyName = foreignKeyName;
            Where = where;
        }

        public string ForeignKeyName { get; }
        public Func<TDependencyDbEntity, bool> Where { get; }

        public TDependencyKey ExtractForeignKeyReferenceValue(IPrometheusDbContext prometheusDbContext)
        {
            var entity = prometheusDbContext.Set<TDependencyDbEntity>().Where(Where);
            return (TDependencyKey)entity.GetType().GetProperty(ForeignKeyName).GetValue(entity);
        }
    }


    public interface IPrometheusMigrationDbContext
    {
        Task ApplyMigration();
    }

    public interface IPrometheusDbContext : IComponentDbContext
    {
        DbSet<ApplicationUser> ApplicationUser { get; set; }

        DbSet<City> City { get; set; }

        //DbSet<Role> Role { get; set; }

        DbSet<Constant> Constant { get; set; }

        DbSet<SearchTicket> SearchTicket { get; set; }

        DbSet<TDependencyDbEntity> Set<TDependencyDbEntity>() where TDependencyDbEntity : class;
    }
}