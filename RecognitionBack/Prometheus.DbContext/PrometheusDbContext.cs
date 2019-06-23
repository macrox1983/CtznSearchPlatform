using Microsoft.EntityFrameworkCore;
using Prometheus.Infrastructure.Component.DbConfigurator;
using Prometheus.Infrastructure.Component.DbContext;
using Prometheus.Infrastructure.DbContextBase;
using Prometheus.Model;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.DbContext
{
    public sealed class PrometheusDbContext : ComponentDbContext<PrometheusDbContext>, IPrometheusDbContext, IPrometheusMigrationDbContext
    {
        private readonly IEfDbConfigurator _databaseConfiguration;

        public PrometheusDbContext(ComponentDbContextOptions<PrometheusDbContext> options) : base(options)
        {
            //Func<State, bool> where = new Func<State, bool>(s=> { return true; });
            //Set<TEntity>().Where(where);
            //Flight.Where(f => f.FlightNumber == "UT254").First();
        }

        public object GetKey(Type dependentEntityType)
        {
            var md = base.Entry(new object()).Metadata;
            var fk = md.GetForeignKeys();

            var key = fk.FirstOrDefault(k => k.DeclaringEntityType == dependentEntityType);
            if(key!=null)
            {
               // new DbSet<>()
            }
            return null;
        }

        #region Sets of entities      
        public DbSet<ApplicationUser> ApplicationUser { get; set; }        

        public DbSet<City> City { get; set; }

        //public DbSet<Role> Role { get; set; }     

        public DbSet<Constant> Constant { get; set; }

        public DbSet<SearchTicket> SearchTicket { get; set; }

        public DbSet<SearchParticipant> SearchParticipant { get; set; }

        public DbSet<FaceRecognitionHistory> FaceRecognitionHistory { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if(_databaseConfiguration!=null)
                _databaseConfiguration.ConfigureDatabaseModelCreating(modelBuilder);                                  


            // Пользователь_систекмы N ←→ 1 Роль_пользователя
            //modelBuilder.Entity<ApplicationUser>()
            //    .HasOne<Role>(u => u.Role)
            //    .WithMany(r => r.ApplicationUsers)
            //    .HasForeignKey(u => u.RoleId);

            //// Роль_пользователя 1 → N Пользователь_систекмы
            //modelBuilder.Entity<Role>()
            //    .HasMany<ApplicationUser>(r => r.ApplicationUsers)
            //    .WithOne(u => u.Role)
            //    .HasForeignKey(u => u.RoleId);
           
            base.OnModelCreating(modelBuilder);
        }

        public async Task ApplyMigration()
        {          
            if((await Database.GetPendingMigrationsAsync()).Count()>0)
                await Database.MigrateAsync();
        }

        public async Task SaveChanges(CancellationToken cancellationToken = default(CancellationToken))
        {            
            await SaveChangesAsync(cancellationToken);
        }

        public async Task<int> SaveChanges(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            
            return await SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
