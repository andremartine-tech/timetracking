using System.Threading.Tasks;
using Application.DTOs;

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
        Task<(UserDto, string)> AuthenticateAsync(string username, string password);

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
    }
}
