using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Haka.CtznSearch.Front.Models;

namespace Haka.CtznSearch.Front
{
    public class DBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<SearchTrack> SearchTracks { get; set; }
        public DbSet<SearchMessage> SearchMessages { get; set; }
        public DbSet<SearchCameraMatch> SearchCameraMatchs { get; set; }


        public DBContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u =>  u.Phone );
            modelBuilder.Entity<Ticket>().HasKey(t => t.Id );            
            modelBuilder.Entity<SearchTrack>().HasKey(p => p.Id);
            modelBuilder.Entity<SearchMessage>().HasKey(p => p.Id);

            modelBuilder.Entity<SearchCameraMatch>().HasKey(p => p.Id);  
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Data.db");
        }
    }
}
