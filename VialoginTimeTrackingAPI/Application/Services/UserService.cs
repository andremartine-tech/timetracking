using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;

namespace Application.Services
{
    /// <summary>
    /// Serviço responsável por manipular operações relacionadas a usuários.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;

        public UserService(IUserRepository repository, IMapper mapper, IPasswordService passwordService)
        {
            _repository = repository;
            _mapper = mapper;
            _passwordService = passwordService;
        }

        /// <inheritdoc />
        public async Task<Guid> CreateUserAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new ValidationException("Nome de usuário e senha são obrigatórios.");
            }

            var hashedPassword = _passwordService.HashPassword(password);

            // Verifica se o usuário já existe
            var existingUser = await _repository.GetAllAsync();
            if (existingUser.Any(user => user.Username == username))
            {
                throw new DomainException("O nome de usuário já está em uso.");
            }

            // Cria e salva o novo usuário
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            await _repository.AddAsync(user);
            return user.Id;
        }

        /// <inheritdoc />
        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            var user = await _repository.GetByIdAsync(id);

            if (user == null)
            {
                throw new DomainException("Usuário não encontrado.");
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            var user = await _repository.GetByUsernameAsync(username);

            if (user == null)
            {
                throw new DomainException("Usuário não encontrado.");
            }

            return _mapper.Map<UserDto>(user);
        }
    }
}
