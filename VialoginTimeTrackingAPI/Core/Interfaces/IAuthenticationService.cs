using System.Threading.Tasks;
using Application.DTOs;

namespace Core.Interfaces
{
    /// <summary>
    /// Interface para servi�os de autentica��o e gerenciamento de tokens.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Autentica um usu�rio com base nas credenciais fornecidas e retorna um token JWT.
        /// </summary>
        /// <param name="username">Nome de usu�rio.</param>
        /// <param name="password">Senha do usu�rio.</param>
        /// <returns>O token JWT.</returns>
        Task<(UserDto, string)> AuthenticateAsync(string username, string password);

        /// <summary>
        /// Renova o token JWT com base no refresh token.
        /// </summary>
        /// <param name="refreshToken">Refresh token para renova��o.</param>
        /// <returns>O novo token JWT.</returns>
        Task<string> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Gera um token.
        /// </summary>
        /// <param name="userDto">Usu�rio para a gera��o do Token.</param>
        /// <returns>O novo token JWT.</returns>
        string GenerateJwtToken(UserDto userDto);
    }
}
