using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    /// <summary>
    /// Interface genérica para operações básicas de repositório.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Obtém um item pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador único do item.</param>
        /// <returns>O item correspondente ao identificador.</returns>
        Task<T> GetByIdAsync(Guid id);

        /// <summary>
        /// Obtém todos os itens.
        /// </summary>
        /// <returns>Uma lista de todos os itens.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Adiciona um novo item ao repositório.
        /// </summary>
        /// <param name="entity">O item a ser adicionado.</param>
        /// <returns>Tarefa assíncrona.</returns>
        Task AddAsync(T entity);

        /// <summary>
        /// Atualiza um item existente no repositório.
        /// </summary>
        /// <param name="entity">O item a ser atualizado.</param>
        /// <returns>Tarefa assíncrona.</returns>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Exclui um item do repositório.
        /// </summary>
        /// <param name="id">Identificador único do item a ser excluído.</param>
        /// <returns>Tarefa assíncrona.</returns>
        Task DeleteAsync(Guid id);
    }
}
