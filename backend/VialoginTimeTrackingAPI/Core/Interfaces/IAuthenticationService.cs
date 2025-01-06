using System.Threading.Tasks;
using Application.DTOs;
using VialoginTimeTrackingAPI.Application.DTOs;

namespace Core.Interfaces
{
    /// <summary>
    /// Interface para serviços de autenticação e gerenciamento de tokens.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Autentica um usuário com base nas credenciais fornecidas e retorna um token JWT.
        /// </summary>
        /// <param name="username">Nome de usuário.</param>
        /// <param name="password">Senha do usuário.</param>
        /// <returns>O token JWT.</returns>
        Task<AuthResponseDto> AuthenticateAsync(string username, string password);

        /// <summary>
        /// Renova o token JWT com base no refresh token.
        /// </summary>
        /// <param name="refreshToken">Refresh token para renovação.</param>
        /// <returns>O novo token JWT.</returns>
        Task<string> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Gera um token.
        /// </summary>
        /// <param name="userDto">Usuário para a geração do Token.</param>
        /// <returns>O novo token JWT.</returns>
        string GenerateJwtToken(UserDto userDto);

        /// <summary>
        /// Invalida um token.
        /// </summary>
        /// <param name="token">Token a ser invalidado.</param>
        /// <returns>void.</returns>
        public Task InvalidateTokenAsync(string token);

        /// <summary>
        /// Verifica se um refresh token é válido.
        /// </summary>
        /// <param name="refreshToken">RefreshToken a ser verificado.</param>
        /// <returns>Verdadeiro se é valido, falso se não.</returns>
        public Task<bool> IsRefreshTokenValidAsync(string refreshToken);

        /// <summary>
        /// Verifica se um token é válido.
        /// </summary>
        /// <param name="token">Token a ser verificado.</param>
        /// <returns>Verdadeiro se é valido, falso se não.</returns>
        public Task<bool> ValidateJwtToken(string token);

        /// <summary>
        /// Busca o username em um Token.
        /// </summary>
        /// <param name="token">Token do qual deve ser retornado o username.</param>
        /// <returns>Username.</returns>
        public string GetUsernameFromToken(string token);
    }
}
