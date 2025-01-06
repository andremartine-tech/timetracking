using MediatR;
using Application.Interfaces;
using Core.Exceptions;

namespace Application.Features.TimeLogs.Commands
{
    /// <summary>
    /// Comando para atualizar um registro de ponto.
    /// </summary>
    public class UpdateTimeLogCommand : IRequest
    {
        /// <summary>
        /// Identificador do registro de ponto.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Data e hora do registro de ponto de entrada.
        /// </summary>
        public DateTime TimestampIn { get; set; }

        /// <summary>
        /// Data e hora do registro de ponto de saída.
        /// </summary>
        public DateTime TimestampOut { get; set; }
    }

    /// <summary>
    /// Manipulador para o comando RegisterTimeLogCommand.
    /// </summary>
    public class UpdateTimeLogCommandHandler : IRequestHandler<UpdateTimeLogCommand>
    {
        private readonly ITimeLogService _timeLogService;

        public UpdateTimeLogCommandHandler(ITimeLogService timeLogService)
        {
            _timeLogService = timeLogService;
        }

        /// <inheritdoc />
        public async Task Handle(UpdateTimeLogCommand request, CancellationToken cancellationToken)
        {
            await _timeLogService.UpdateTimeLogAsync(request.Id, request.TimestampIn, request.TimestampOut);
        }
    }
}
