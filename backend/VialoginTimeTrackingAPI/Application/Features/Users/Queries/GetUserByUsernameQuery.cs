using Core.Exceptions;
using MediatR;
using Application.DTOs;
using Application.Interfaces;

namespace Application.Features.Users.Queries
{
    /// <summary>
    /// Consulta para obter um usu�rio pelo identificador �nico.
    /// </summary>
    public class GetUserByUsernameQuery : IRequest<UserDto>
    {
        /// <summary>
        /// Identificador do usu�rio a ser buscado.
        /// </summary>
        public string Username { get; set; }
    }

    /// <summary>
    /// Manipulador para a consulta GetUserByIdQuery.
    /// </summary>
    public class GetUserByUsernameQueryHandler : IRequestHandler<GetUserByUsernameQuery, UserDto>
    {
        private readonly IUserService _userService;

        public GetUserByUsernameQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        /// <inheritdoc />
        public async Task<UserDto> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByUsernameAsync(request.Username);

            return user;
        }
    }
}
