using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Repositório específico para operações relacionadas à entidade User.
    /// </summary>
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        ///// <summary>
        ///// Busca um usuário pelo identificador único de usuário (id).
        ///// </summary>
        ///// <param name="id">Identificador único de usuário.</param>
        ///// <returns>Entidade User correspondente ou null.</returns>
        //async Task<User> IRepository<User>.GetByIdAsync(Guid id)
        //{
        //    return await _context.Users
        //        .FirstOrDefaultAsync(u => u.Id == id);
        //}

        /// <inheritdoc/>
        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
