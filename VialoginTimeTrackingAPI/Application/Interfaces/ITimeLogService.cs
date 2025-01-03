using Application.DTOs;
using MediatR;

namespace Application.Interfaces
{
    /// <summary>
    /// Interface para opera��es relacionadas a registros de ponto.
    /// </summary>
    public interface ITimeLogService
    {
        /// <summary>
        /// Registra um novo ponto.
        /// </summary>
        /// <param name="userId">Identificador do usu�rio associado.</param>
        /// <param name="timestampIn">Data e hora do registro de entrada.</param>
        /// <param name="timestampOut">Data e hora do registro de sa�da.</param>
        /// <returns>Tarefa ass�ncrona.</returns>
        Task RegisterTimeLogAsync(Guid userId, DateTime timestampIn, DateTime timestampOut);

        /// <summary>
        /// Atualiza um registro de ponto.
        /// </summary>
        /// <param name="Id">Identificador do registro de ponto.</param>
        /// <param name="timestampIn">Data e hora do registro de entrada.</param>
        /// <param name="timestampOut">Data e hora do registro de sa�da.</param>
        /// <returns>Tarefa ass�ncrona.</returns>
        Task UpdateTimeLogAsync(Guid Id, DateTime TimestampIn, DateTime TimestampOut);

        /// <summary>
        /// Exclui um registro de ponto.
        /// </summary>
        /// <param name="Id">Identificador do registro de ponto.</param>
        /// <returns>Tarefa ass�ncrona.</returns>
        Task DeleteTimeLogAsync(Guid Id);

        /// <summary>
        /// Exclui todos os registros de ponto do usu�rio de testes.
        /// </summary>
        /// <param name="UserId">Identificador do usu�rio de testes dos registros de ponto.</param>
        /// <returns>Tarefa ass�ncrona.</returns>
        Task DeleteTimeLogsFromUserAsync(Guid UserId);

        /// <summary>
        /// Obt�m os registros de ponto associados a um usu�rio.
        /// </summary>
        /// <param name="userId">Identificador do usu�rio.</param>
        /// <returns>Lista de objetos TimeLogDto contendo os registros de ponto.</returns>
        Task<IEnumerable<TimeLogDto>> GetTimeLogsByUserAsync(Guid userId);

        /// <summary>
        /// Obt�m os registros de ponto associados a um usu�rio por m�s e ano.
        /// </summary>
        /// <param name="userId">Identificador do usu�rio.</param>
        /// <param name="month">M�s de refer�ncia.</param>
        /// <param name="year">Ano de refer�ncia.</param>
        /// <returns>Lista de objetos TimeLogDto contendo os registros de ponto.</returns>
        Task<IEnumerable<TimeLogDto>> GetTimeLogsByUserMonthYearAsync(Guid userId, int month, int year);
    }
}
