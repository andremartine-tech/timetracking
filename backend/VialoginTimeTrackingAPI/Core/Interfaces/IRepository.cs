using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    /// <summary>
    /// Interface gen�rica para opera��es b�sicas de reposit�rio.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Obt�m um item pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador �nico do item.</param>
        /// <returns>O item correspondente ao identificador.</returns>
        Task<T> GetByIdAsync(Guid id);

        /// <summary>
        /// Obt�m todos os itens.
        /// </summary>
        /// <returns>Uma lista de todos os itens.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Adiciona um novo item ao reposit�rio.
        /// </summary>
        /// <param name="entity">O item a ser adicionado.</param>
        /// <returns>Tarefa ass�ncrona.</returns>
        Task AddAsync(T entity);

        /// <summary>
        /// Atualiza um item existente no reposit�rio.
        /// </summary>
        /// <param name="entity">O item a ser atualizado.</param>
        /// <returns>Tarefa ass�ncrona.</returns>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Exclui um item do reposit�rio.
        /// </summary>
        /// <param name="id">Identificador �nico do item a ser exclu�do.</param>
        /// <returns>Tarefa ass�ncrona.</returns>
        Task DeleteAsync(Guid id);
    }
}
