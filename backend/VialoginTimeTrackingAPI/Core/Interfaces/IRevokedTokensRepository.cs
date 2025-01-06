using VialoginTimeTrackingAPI.Core.Entities;

namespace VialoginTimeTrackingAPI.Core.Interfaces
{
    public interface IRevokedTokensRepository
    {
        Task AddAsync(RevokedToken revokedToken);
        Task<bool> IsTokenRevokedAsync(string token);
    }
}
