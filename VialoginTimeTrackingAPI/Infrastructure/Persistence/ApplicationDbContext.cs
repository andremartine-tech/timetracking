using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Infrastructure.Persistence.Configurations;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<TimeLog> TimeLogs { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurações adicionais de entidades
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new TimeLogConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}