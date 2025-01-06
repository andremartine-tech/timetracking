using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Infrastructure.Persistence.Configurations;
using VialoginTimeTrackingAPI.Core.Entities;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<TimeLog> TimeLogs { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<RevokedToken> RevokedTokens { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurações adicionais de entidades
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new TimeLogConfiguration());

            // Criação de índices
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique()
                .HasDatabaseName("idx_users_username");

            modelBuilder.Entity<TimeLog>()
                .HasIndex(t => t.UserId)
                .HasDatabaseName("idx_timelogs_userid");

            modelBuilder.Entity<TimeLog>()
                .HasIndex(t => t.TimestampIn)
                .HasDatabaseName("idx_timelogs_timestampin");

            modelBuilder.Entity<TimeLog>()
                .HasIndex(t => t.TimestampOut)
                .HasDatabaseName("idx_timelogs_timestampout");

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(r => r.Token)
                .HasDatabaseName("idx_refreshtokens_token");

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(r => r.Username)
                .HasDatabaseName("idx_refreshtokens_username");

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(r => r.Expiration)
                .HasDatabaseName("idx_refreshtokens_expiration");

            modelBuilder.Entity<RevokedToken>()
                .HasIndex(r => r.Token)
                .HasDatabaseName("idx_revokedtokens_token");

            modelBuilder.Entity<RevokedToken>()
                .HasIndex(r => r.RevokedAt)
                .HasDatabaseName("idx_revokedtokens_revokedat");

            base.OnModelCreating(modelBuilder);
        }
    }
}