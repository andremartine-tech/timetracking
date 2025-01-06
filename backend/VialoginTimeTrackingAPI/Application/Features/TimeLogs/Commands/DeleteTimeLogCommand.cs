using Application.Interfaces;
using Core.Exceptions;
using Core.Interfaces;
using MediatR;

namespace Application.Features.TimeLogs.Commands
{
    /// <summary>
    /// Comando para excluir um registro de ponto.
    /// </summary>
    public class DeleteTimeLogCommand : IRequest
    {
        /// <summary>
        /// Identificador do registro de ponto.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Construtor do command DeleteTimeLogCommand
        /// <param name="id">Identificador único do registro de ponto.</param>
        /// </summary>
        public DeleteTimeLogCommand(Guid id)
        {
            Id = id;
        }
    }

    /// <summary>
    /// Manipulador para o comando DeleteTimeLogCommand.
    /// </summary>
    public class DeleteTimeLogCommandHandler : IRequestHandler<DeleteTimeLogCommand>
    {
        private readonly ITimeLogService _timeLogService;

        public DeleteTimeLogCommandHandler(ITimeLogService timeLogService)
        {
            _timeLogService = timeLogService;
        }

        public async Task Handle(DeleteTimeLogCommand request, CancellationToken cancellationToken)
        {
            await _timeLogService.DeleteTimeLogAsync(request.Id);
        }
    }
}

