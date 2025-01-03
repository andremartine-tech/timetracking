using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities;

namespace Infrastructure.Persistence.Configurations
{
    public class TimeLogConfiguration : IEntityTypeConfiguration<TimeLog>
    {
        public void Configure(EntityTypeBuilder<TimeLog> builder)
        {
            builder.HasKey(t => t.Id); // Define a chave primária
            builder.Property(t => t.TimestampIn)
                   .IsRequired(); // Campo obrigatório
            builder.Property(t => t.TimestampOut)
                   .IsRequired(); // Campo obrigatório

            // Relacionamento N:1 entre TimeLogs e User
            builder.HasOne(t => t.User)
                   .WithMany(u => u.TimeLogs)
                   .HasForeignKey(t => t.UserId);
        }
    }
}