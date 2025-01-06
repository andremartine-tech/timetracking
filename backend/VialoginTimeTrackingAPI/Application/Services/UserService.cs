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
    /// Serviço responsável por manipular operações relacionadas a usuários.
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
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            // Verifica se os usuários estão no cache
            if (!_cache.TryGetValue("UsersCache", out IEnumerable<User> users))
            {
                // Se não estiver no cache, busca no banco
                users = await _repository.GetAllAsNoTrackingAsync();

                // Armazena no cache por 10 minutos
                _cache.Set("UsersCache", users, TimeSpan.FromMinutes(10));
            }

            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        /// <inheritdoc />
        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            // Cria uma chave específica para cada usuário
            var cacheKey = $"User_{id}";

            if (!_cache.TryGetValue(cacheKey, out User user))
            {
                // Busca no banco se não estiver no cache
                user = await _repository.GetByIdAsync(id);

                if (user == null)
                {
                    throw new DomainException("Usuário não encontrado.");
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
            // Cria uma chave específica para cada usuário
            var cacheKey = $"User_{username}";

            if (!_cache.TryGetValue(cacheKey, out User user))
            {
                // Busca no banco se não estiver no cache
                user = await _repository.GetByUsernameAsync(username);

                if (user == null)
                {
                    throw new DomainException("Usuário não encontrado.");
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
