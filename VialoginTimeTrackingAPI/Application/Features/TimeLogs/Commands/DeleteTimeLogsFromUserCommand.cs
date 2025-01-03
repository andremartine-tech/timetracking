using Application.Interfaces;
using Core.Exceptions;
using Core.Interfaces;
using MediatR;

namespace Application.Features.TimeLogs.Commands
{
    /// <summary>
    /// Comando para excluir todos os registros de ponto do usuário de teste.
    /// </summary>
    public class DeleteTimeLogsFromUserCommand : IRequest
    {
        /// <summary>
        /// Identificador do registro do usuário de teste.
        /// </summary>
        public Guid UserId { get; }

        /// <summary>
        /// Construtor do command DeleteTimeLogsFromUserCommand
        /// <param name="userId">Identificador do registro do usuário de teste.</param>
        /// </summary>
        public DeleteTimeLogsFromUserCommand(Guid userId)
        {
            UserId = userId;
        }
    }

    /// <summary>
    /// Manipulador para o comando DeleteTimeLogsFromUserCommand.
    /// </summary>
    public class DeleteTimeLogsFromUserCommandHandler : IRequestHandler<DeleteTimeLogsFromUserCommand>
    {
        private readonly ITimeLogService _timeLogService;

        public DeleteTimeLogsFromUserCommandHandler(ITimeLogService timeLogService)
        {
            _timeLogService = timeLogService;
        }

        public async Task Handle(DeleteTimeLogsFromUserCommand request, CancellationToken cancellationToken)
        {
            await _timeLogService.DeleteTimeLogsFromUserAsync(request.UserId);
        }
    }
}

