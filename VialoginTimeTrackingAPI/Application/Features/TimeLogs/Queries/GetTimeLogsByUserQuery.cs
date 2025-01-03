using Core.Exceptions;
using MediatR;
using Application.DTOs;
using Application.Interfaces;

namespace Application.Features.TimeLogs.Queries
{
    /// <summary>
    /// Consulta para obter os registros de ponto de um usuário específico.
    /// </summary>
    public class GetTimeLogsByUserQuery : IRequest<IEnumerable<TimeLogDto>>
    {
        /// <summary>
        /// Identificador do usuário.
        /// </summary>
        public Guid UserId { get; set; }
    }

    /// <summary>
    /// Manipulador para a consulta GetTimeLogsByUserQuery.
    /// </summary>
    public class GetTimeLogsByUserQueryHandler : IRequestHandler<GetTimeLogsByUserQuery, IEnumerable<TimeLogDto>>
    {
        private readonly ITimeLogService _timeLogService;

        public GetTimeLogsByUserQueryHandler(ITimeLogService timeLogService)
        {
            _timeLogService = timeLogService;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TimeLogDto>> Handle(GetTimeLogsByUserQuery request, CancellationToken cancellationToken)
        {
            var logs = await _timeLogService.GetTimeLogsByUserAsync(request.UserId);

            if (logs == null)
            {
                throw new DomainException("Nenhum registro de ponto encontrado para o usuário.");
            }

            return logs;
        }
    }
}
