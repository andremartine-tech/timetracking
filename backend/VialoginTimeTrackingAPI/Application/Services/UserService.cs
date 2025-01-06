using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

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
        private readonly IMemoryCache _cache;

        public UserService(IUserRepository repository, IMapper mapper, IPasswordService passwordService, IMemoryCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _passwordService = passwordService;
            _cache = cache;
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
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            // Verifica se os usu�rios est�o no cache
            if (!_cache.TryGetValue("UsersCache", out IEnumerable<User> users))
            {
                // Se n�o estiver no cache, busca no banco
                users = await _repository.GetAllAsNoTrackingAsync();

                // Armazena no cache por 10 minutos
                _cache.Set("UsersCache", users, TimeSpan.FromMinutes(10));
            }

            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        /// <inheritdoc />
        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            // Cria uma chave espec�fica para cada usu�rio
            var cacheKey = $"User_{id}";

            if (!_cache.TryGetValue(cacheKey, out User user))
            {
                // Busca no banco se n�o estiver no cache
                user = await _repository.GetByIdAsync(id);

                if (user == null)
                {
                    throw new DomainException("Usu�rio n�o encontrado.");
                }

                if (user != null)
                {
                    _cache.Set(cacheKey, user, TimeSpan.FromMinutes(10));
                }
            }

            return _mapper.Map<UserDto>(user);
        }

        /// <inheritdoc />
        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            // Cria uma chave espec�fica para cada usu�rio
            var cacheKey = $"User_{username}";

            if (!_cache.TryGetValue(cacheKey, out User user))
            {
                // Busca no banco se n�o estiver no cache
                user = await _repository.GetByUsernameAsync(username);

                if (user == null)
                {
                    throw new DomainException("Usu�rio n�o encontrado.");
                }

                if (user != null)
                {
                    _cache.Set(cacheKey, user, TimeSpan.FromMinutes(10));
                }
            }

            return _mapper.Map<UserDto>(user);
            
        }
    }
}
