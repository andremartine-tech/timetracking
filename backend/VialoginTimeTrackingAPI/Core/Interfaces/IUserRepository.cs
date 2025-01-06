using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    /// Interface específica para operações relacionadas à entidade User.
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Busca um usuário pelo nome de usuário (username).
        /// </summary>
        /// <param name="username">Nome de usuário.</param>
        /// <returns>Entidade User correspondente ou null.</returns>
        Task<User> GetByUsernameAsync(string username);

        /// <summary>
        /// Busca todos os usuários.
        /// </summary>
        /// <returns>Todos os usuários.</returns>
        Task<IEnumerable<User>> GetAllAsNoTrackingAsync();

        /// <summary>
        /// Busca o usuário pelo Id.
        /// </summary>
        /// <param name="id">Id do usuário.</param>
        /// <returns>O usuário pelo Id.</returns>
        Task<User> GetByIdAsNoTrackingAsync(Guid id);
    }
}

