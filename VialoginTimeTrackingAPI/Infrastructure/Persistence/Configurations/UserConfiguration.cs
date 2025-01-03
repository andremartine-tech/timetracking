using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities;

namespace Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id); // Define a chave prim�ria
            builder.Property(u => u.Username)
                   .IsRequired() // Campo obrigat�rio
                   .HasMaxLength(100); // Define o tamanho m�ximo

            builder.Property(u => u.PasswordHash)
                   .IsRequired();

            // Relacionamento 1:N entre User e TimeLogs
            builder.HasMany(u => u.TimeLogs)
                   .WithOne(t => t.User)
                   .HasForeignKey(t => t.UserId);
        }
    }
}