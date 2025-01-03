using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;

namespace Application.Services
{
    /// <summary>
    /// Servi�o respons�vel por manipular opera��es relacionadas a usu�rios.
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
                throw new ValidationException("Nome de usu�rio e senha s�o obrigat�rios.");
            }

            var hashedPassword = _passwordService.HashPassword(password);

            // Verifica se o usu�rio j� existe
            var existingUser = await _repository.GetAllAsync();
            if (existingUser.Any(user => user.Username == username))
            {
                throw new DomainException("O nome de usu�rio j� est� em uso.");
            }

            // Cria e salva o novo usu�rio
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
                throw new DomainException("Usu�rio n�o encontrado.");
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            var user = await _repository.GetByUsernameAsync(username);

            if (user == null)
            {
                throw new DomainException("Usu�rio n�o encontrado.");
            }

            return _mapper.Map<UserDto>(user);
        }
    }
}
