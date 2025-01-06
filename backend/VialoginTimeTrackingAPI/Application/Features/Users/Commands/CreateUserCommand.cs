using System;
using MediatR;
using Application.Interfaces;
using Core.Exceptions;

namespace Application.Features.Users.Commands
{
    /// <summary>
    /// Comando para criar um novo usu�rio.
    /// </summary>
    public class CreateUserCommand : IRequest<Guid>
    {
        /// <summary>
        /// Nome de usu�rio.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Senha do usu�rio.
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// Manipulador para o comando CreateUserCommand.
    /// </summary>
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IUserService _userService;

        public CreateUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        /// <inheritdoc />
        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ValidationException("Nome de usu�rio e senha s�o obrigat�rios.");
            }

            return await _userService.CreateUserAsync(request.Username, request.Password);
        }
    }
}
