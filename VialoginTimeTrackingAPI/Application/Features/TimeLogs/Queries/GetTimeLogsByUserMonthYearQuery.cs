using Core.Exceptions;
using MediatR;
using Application.DTOs;
using Application.Interfaces;

namespace Application.Features.TimeLogs.Queries
{
    /// <summary>
    /// Consulta para obter os registros de ponto de um usu�rio espec�fico pelo m�s e ano.
    /// </summary>
    public class GetTimeLogsByUserMonthYearQuery : IRequest<IEnumerable<TimeLogDto>>
    {
        /// <summary>
        /// Identificador do usu�rio.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// M�s.
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Ano.
        /// </summary>
        public int Year { get; set; }

    }

    /// <summary>
    /// Manipulador para a consulta GetTimeLogsByUserMonthYearQuery.
    /// </summary>
    public class GetTimeLogsByUserMonthYearQueryHandler : IRequestHandler<GetTimeLogsByUserMonthYearQuery, IEnumerable<TimeLogDto>>
    {
        private readonly ITimeLogService _timeLogService;

        public GetTimeLogsByUserMonthYearQueryHandler(ITimeLogService timeLogService)
        {
            _timeLogService = timeLogService;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TimeLogDto>> Handle(GetTimeLogsByUserMonthYearQuery request, CancellationToken cancellationToken)
        {
            var logs = await _timeLogService.GetTimeLogsByUserMonthYearAsync(request.UserId, request.Month, request.Year);

            if (logs == null)
            {
                throw new DomainException("Nenhum registro de ponto encontrado para o usu�rio.");
            }

            return logs;
        }
    }
}
