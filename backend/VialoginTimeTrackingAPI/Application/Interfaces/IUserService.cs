using Application.DTOs;
using Core.Entities;

namespace Application.Interfaces
{
    /// <summary>
    /// Interface para operações relacionadas a usuários.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Cria um novo usuário no sistema.
        /// </summary>
        /// <param name="username">Nome de usuário.</param>
        /// <param name="password">Senha do usuário.</param>
        /// <returns>Identificador do usuário criado.</returns>
        Task<Guid> CreateUserAsync(string username, string password);

        /// <summary>
        /// Obtém os dados de um usuário pelo identificador único.
        /// </summary>
        /// <param name="id">Identificador do usuário.</param>
        /// <returns>Objeto UserDto contendo as informações do usuário.</returns>
        Task<UserDto> GetUserByIdAsync(Guid id);

        /// <summary>
        /// Obtém os dados de um usuário pelo nome de usuário.
        /// </summary>
        /// <param name="username">Nome de usuário.</param>
        /// <returns>Objeto UserDto contendo as informações do usuário.</returns>
        Task<UserDto> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Obtém os dados de todos os usuários.
        /// </summary>
        /// <returns>Lista de UserDto contendo as informações dos usuários.</returns>
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
    }
}
