using System;
using Microsoft.EntityFrameworkCore;
using TimeTracker.DataBase.DBModels;

namespace TimeTracker.DataBase
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
            Database.Migrate();
        }

        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("TimeTracker");

            modelBuilder.Entity<User>().HasKey(x => x.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}

