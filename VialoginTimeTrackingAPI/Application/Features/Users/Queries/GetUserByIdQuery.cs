using Core.Exceptions;
using MediatR;
using Application.DTOs;
using Application.Interfaces;

namespace Application.Features.Users.Queries
{
    /// <summary>
    /// Consulta para obter um usuário pelo identificador único.
    /// </summary>
    public class GetUserByIdQuery : IRequest<UserDto>
    {
        /// <summary>
        /// Identificador do usuário a ser buscado.
        /// </summary>
        public Guid Id { get; set; }
    }

    /// <summary>
    /// Manipulador para a consulta GetUserByIdQuery.
    /// </summary>
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserService _userService;

        public GetUserByIdQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        /// <inheritdoc />
        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByIdAsync(request.Id);

            if (user == null)
            {
                throw new DomainException("Usuário não encontrado.");
            }

            return user;
        }
    }
}
