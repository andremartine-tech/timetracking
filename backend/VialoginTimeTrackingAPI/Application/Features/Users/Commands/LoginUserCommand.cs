using System;
using MediatR;
using Core.Exceptions;
using Core.Interfaces;
using Application.DTOs;
using VialoginTimeTrackingAPI.Application.DTOs;

namespace Application.Features.Users.Commands
{
    /// <summary>
    /// Comando para autenticar um usu�rio.
    /// </summary>
    public class LoginUserCommand : IRequest<AuthResponseDto>
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
    /// Manipulador para o comando LoginUserCommand.
    /// </summary>
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResponseDto>
    {
        private readonly IAuthenticationService _authenticationService;

        /// <inheritdoc />
        public LoginUserCommandHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <inheritdoc />
        public async Task<AuthResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ValidationException("Nome de usu�rio e senha s�o obrigat�rios.");
            }

            return await _authenticationService.AuthenticateAsync(request.Username, request.Password);
        }
    }
}
