using Application.DTOs;
using Core.Entities;

namespace Application.Interfaces
{
    /// <summary>
    /// Interface para opera��es relacionadas a usu�rios.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Cria um novo usu�rio no sistema.
        /// </summary>
        /// <param name="username">Nome de usu�rio.</param>
        /// <param name="password">Senha do usu�rio.</param>
        /// <returns>Identificador do usu�rio criado.</returns>
        Task<Guid> CreateUserAsync(string username, string password);

        /// <summary>
        /// Obt�m os dados de um usu�rio pelo identificador �nico.
        /// </summary>
        /// <param name="id">Identificador do usu�rio.</param>
        /// <returns>Objeto UserDto contendo as informa��es do usu�rio.</returns>
        Task<UserDto> GetUserByIdAsync(Guid id);

        /// <summary>
        /// Obt�m os dados de um usu�rio pelo nome de usu�rio.
        /// </summary>
        /// <param name="username">Nome de usu�rio.</param>
        /// <returns>Objeto UserDto contendo as informa��es do usu�rio.</returns>
        Task<UserDto> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Obt�m os dados de todos os usu�rios.
        /// </summary>
        /// <returns>Lista de UserDto contendo as informa��es dos usu�rios.</returns>
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
    }
}
