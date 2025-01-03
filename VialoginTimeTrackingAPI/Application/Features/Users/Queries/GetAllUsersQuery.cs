using System.Collections.Generic;
using MediatR;
using Application.DTOs;
using Core.Interfaces;
using Core.Entities;
using AutoMapper;

namespace Application.Features.Users.Queries
{
    /// <summary>
    /// Consulta para obter todos os usuários.
    /// </summary>
    public class GetAllUsersQuery : IRequest<IEnumerable<UserDto>>
    {
    }

    /// <summary>
    /// Manipulador para a consulta GetAllUsersQuery.
    /// </summary>
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
    {
        private readonly IRepository<User> _repository;
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandler(IRepository<User> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
    }
}
