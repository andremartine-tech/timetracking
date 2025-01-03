using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    /// Interface específica para operações relacionadas à entidade TimeLog.
    /// </summary>
    public interface ITimeLogRepository : IRepository<TimeLog>
    {
        /// <summary>
        /// Busca todos os registros de ponto associados a um usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <returns>Lista de registros de ponto do usuário.</returns>
        Task<IEnumerable<TimeLog>> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Busca todos os registros de ponto associados a um usuário.
        /// </summary>
        /// <param name="start">Data e hora do registro de ponto de entrada.</param>
        /// <param name="end">Data e hora do registro de ponto de saída.</param>
        /// <param name="userId">Identificador do usuário.</param>
        /// <returns>True se encontrou sobreposição; caso contrário, False.</returns>
        Task<bool> HasOverlapAsync(Guid id, DateTime start, DateTime end, Guid userId);

        /// <summary>
        /// Busca todos os de ponto de um usuário em um mês/ano.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="monthYear">Mês e ano de referência.</param>
        /// <returns>True se encontrou sobreposição; caso contrário, False.</returns>
        Task<IEnumerable<TimeLog>> GetByUserAndMonthYearAsync(Guid userId, DateTime monthYear);

        /// <summary>
        /// Busca todos os registros de ponto de acordo com uma query.
        /// </summary>
        /// <param name="predicate">Expressão lambda para filtrar os dados retornados.</param>
        /// <returns>Lista de registros de ponto do usuário.</returns>
        Task<IEnumerable<TimeLog>> QueryAsync(Expression<Func<TimeLog, bool>> predicate, Expression<Func<TimeLog, object>> orderBy = null, bool ascending = true);

        /// <summary>
        /// Exclui todos os registros de ponto do usuário de testes.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <returns>Tarefa assíncrona.</returns>
        Task DeleteFromUserAsync(Guid userId);
            
        /// <summary>
        /// Atualiza o registro de ponto fazendo detach do registro para evitar conflitos.
        /// </summary>
        /// <param name="timeLog">Objeto TimeLog.</param>
        /// <returns>Tarefa assíncrona.</returns>
        Task UpdateDetachedAsync(TimeLog timeLog);
    }
}
