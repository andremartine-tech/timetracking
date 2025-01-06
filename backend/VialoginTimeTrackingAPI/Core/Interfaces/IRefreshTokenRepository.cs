using VialoginTimeTrackingAPI.Core.Entities;

namespace VialoginTimeTrackingAPI.Core.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken refreshToken);
        Task<RefreshToken> GetByTokenAsync(string token);
        Task RemoveAsync(string token);
        Task RemoveAllByUsernameAsync(string username);
    }
}
