using MediatR;
using Application.Interfaces;
using Core.Exceptions;

namespace Application.Features.TimeLogs.Commands
{
    /// <summary>
    /// Comando para registrar um registro de ponto.
    /// </summary>
    public class RegisterTimeLogCommand : IRequest
    {
        /// <summary>
        /// Identificador do usuário.
        /// </summary>
        public Guid UserId { get; set; }

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
    public class RegisterTimeLogCommandHandler : IRequestHandler<RegisterTimeLogCommand>
    {
        private readonly ITimeLogService _timeLogService;

        public RegisterTimeLogCommandHandler(ITimeLogService timeLogService)
        {
            _timeLogService = timeLogService;
        }

        /// <inheritdoc />
        public async Task Handle(RegisterTimeLogCommand request, CancellationToken cancellationToken)
        {
            await _timeLogService.RegisterTimeLogAsync(request.UserId, request.TimestampIn, request.TimestampOut);
        }
    }
}
