using System.Collections.Generic;
using MediatR;
using Application.DTOs;
using Core.Interfaces;
using Core.Entities;
using AutoMapper;
using Application.Interfaces;

namespace Application.Features.Users.Queries
{
    /// <summary>
    /// Consulta para obter todos os usu�rios.
    /// </summary>
    public class GetAllUsersQuery : IRequest<IEnumerable<UserDto>>
    {
    }

    /// <summary>
    /// Manipulador para a consulta GetAllUsersQuery.
    /// </summary>
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
    {
        private readonly IUserService _service;
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandler(IUserService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _service.GetAllUsersAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
    }
}
