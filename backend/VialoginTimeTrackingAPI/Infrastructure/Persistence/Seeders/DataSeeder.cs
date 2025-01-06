using Application.Interfaces;
using Core.Entities;
using Infrastructure.Persistence;

namespace VialoginTimeTrackingAPI.Infrastructure.Persistence.Seeders
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordService _passwordService;

        public DataSeeder(ApplicationDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task SeedAsync()
        {
            if (!_context.Users.Any())
            {
                _context.Users.AddRange(
                    new User
                    {
                        Id = Guid.NewGuid(),
                        Username = "test",
                        PasswordHash = _passwordService.HashPassword("test")
                    },
                    new User
                    {
                        Id = Guid.NewGuid(),
                        Username = "andre",
                        PasswordHash = _passwordService.HashPassword("k4hvd891")
                    }
                );

                await _context.SaveChangesAsync();
            }
        }
    }

}
