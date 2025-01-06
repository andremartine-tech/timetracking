using Application.DTOs;
using MediatR;

namespace Application.Interfaces
{
    /// <summary>
    /// Interface para operações relacionadas a registros de ponto.
    /// </summary>
    public interface ITimeLogService
    {
        /// <summary>
        /// Registra um novo ponto.
        /// </summary>
        /// <param name="userId">Identificador do usuário associado.</param>
        /// <param name="timestampIn">Data e hora do registro de entrada.</param>
        /// <param name="timestampOut">Data e hora do registro de saída.</param>
        /// <returns>Tarefa assíncrona.</returns>
        Task RegisterTimeLogAsync(Guid userId, DateTime timestampIn, DateTime timestampOut);

        /// <summary>
        /// Atualiza um registro de ponto.
        /// </summary>
        /// <param name="Id">Identificador do registro de ponto.</param>
        /// <param name="timestampIn">Data e hora do registro de entrada.</param>
        /// <param name="timestampOut">Data e hora do registro de saída.</param>
        /// <returns>Tarefa assíncrona.</returns>
        Task UpdateTimeLogAsync(Guid Id, DateTime TimestampIn, DateTime TimestampOut);

        /// <summary>
        /// Exclui um registro de ponto.
        /// </summary>
        /// <param name="Id">Identificador do registro de ponto.</param>
        /// <returns>Tarefa assíncrona.</returns>
        Task DeleteTimeLogAsync(Guid Id);

        /// <summary>
        /// Exclui todos os registros de ponto do usuário de testes.
        /// </summary>
        /// <param name="UserId">Identificador do usuário de testes dos registros de ponto.</param>
        /// <returns>Tarefa assíncrona.</returns>
        Task DeleteTimeLogsFromUserAsync(Guid UserId);

        /// <summary>
        /// Obtém os registros de ponto associados a um usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <returns>Lista de objetos TimeLogDto contendo os registros de ponto.</returns>
        Task<IEnumerable<TimeLogDto>> GetTimeLogsByUserAsync(Guid userId);

        /// <summary>
        /// Obtém os registros de ponto associados a um usuário por mês e ano.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="month">Mês de referência.</param>
        /// <param name="year">Ano de referência.</param>
        /// <returns>Lista de objetos TimeLogDto contendo os registros de ponto.</returns>
        Task<IEnumerable<TimeLogDto>> GetTimeLogsByUserMonthYearAsync(Guid userId, int month, int year);
    }
}
