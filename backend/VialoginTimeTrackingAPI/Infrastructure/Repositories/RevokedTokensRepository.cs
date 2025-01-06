using System;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VialoginTimeTrackingAPI.Core.Entities;
using VialoginTimeTrackingAPI.Core.Interfaces;

namespace VialoginTimeTrackingAPI.Infrastructure.Repositories
{
    public class RevokedTokensRepository : IRevokedTokensRepository
    {
        private readonly global::Infrastructure.Persistence.ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public RevokedTokensRepository(global::Infrastructure.Persistence.ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task AddAsync(RevokedToken revokedToken)
        {
            await _context.RevokedTokens.AddAsync(revokedToken);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsTokenRevokedAsync(string token)
        {
            if (_cache.TryGetValue(token, out bool isRevoked))
            {
                return isRevoked;
            }

            isRevoked = await _context.RevokedTokens.AsNoTracking().AnyAsync(rt => rt.Token == token);

            _cache.Set(token, isRevoked, TimeSpan.FromMinutes(60));

            return isRevoked;
        }
    }
}
