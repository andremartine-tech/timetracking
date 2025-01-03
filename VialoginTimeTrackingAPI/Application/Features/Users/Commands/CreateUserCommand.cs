using System;
using MediatR;
using Application.Interfaces;
using Core.Exceptions;

namespace Application.Features.Users.Commands
{
    /// <summary>
    /// Comando para criar um novo usuário.
    /// </summary>
    public class CreateUserCommand : IRequest<Guid>
    {
        /// <summary>
        /// Nome de usuário.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Senha do usuário.
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
                throw new ValidationException("Nome de usuário e senha são obrigatórios.");
            }

            return await _userService.CreateUserAsync(request.Username, request.Password);
        }
    }
}
