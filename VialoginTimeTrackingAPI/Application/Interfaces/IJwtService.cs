using Application.DTOs;

namespace Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(UserDto user);
        bool ValidateToken(string token);
        string GetUsernameFromToken(string token);
    }
}

