using Microsoft.EntityFrameworkCore;
using Prometheus.Infrastructure.Component.DbContext;
using Prometheus.Model;

namespace Prometheus.CtznSearch.ApiService.DataContext
{
    public class CtznSearchDbContext : ComponentDbContext<CtznSearchDbContext>, IComponentDbContext
    {
        public CtznSearchDbContext(ComponentDbContextOptions<CtznSearchDbContext> dbContextOptions):base(dbContextOptions)
        {
        }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        public DbSet<City> City { get; set; }

        //public DbSet<Role> Role { get; set; }

        public DbSet<Constant> Constant { get; set; }

        public DbSet<SearchTicket> SearchTicket { get; set; }

        public DbSet<SearchParticipant> SearchParticipant { get; set; }

        public DbSet<FaceRecognitionHistory> FaceRecognitionHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
