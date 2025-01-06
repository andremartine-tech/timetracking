namespace VialoginTimeTrackingAPI.Infrastructure.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Core.Entities;
    using global::Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore;
    using VialoginTimeTrackingAPI.Core.Interfaces;

    namespace Infrastructure.Repositories
    {
        public class RefreshTokenRepository : IRefreshTokenRepository
        {
            private readonly global::Infrastructure.Persistence.ApplicationDbContext _context;

            public RefreshTokenRepository(global::Infrastructure.Persistence.ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task AddAsync(RefreshToken refreshToken)
            {
                await _context.RefreshTokens.AddAsync(refreshToken);
                await _context.SaveChangesAsync();
            }

            public async Task<RefreshToken> GetByTokenAsync(string token)
            {
                return await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.Token == token);
            }

            public async Task RemoveAsync(string token)
            {
                var refreshToken = GetByTokenAsync(token);
                if (refreshToken != null)
                {
                    _context.RefreshTokens.Remove(refreshToken.Result);
                    await _context.SaveChangesAsync();
                }
            }

            public async Task RemoveAllByUsernameAsync(string username)
            {
                var tokens = await _context.RefreshTokens
                                           .Where(rt => rt.Username == username)
                                           .ToListAsync();
                _context.RefreshTokens.RemoveRange(tokens);
                await _context.SaveChangesAsync();
            }
        }
    }

}
