using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Core.Interfaces;
using Application.DTOs;
using Infrastructure.Repositories;
using MediatR;
using Core.Exceptions;
using Application.Interfaces;
using Core.Entities;
using AutoMapper;

namespace Infrastructure.Services
{
    public class JwtAuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private static readonly Dictionary<string, string> _refreshTokens = new(); // Simula armazenamento em memória

        public JwtAuthenticationService(IConfiguration configuration, IUserRepository userRepository, IPasswordService passwordService, IJwtService jwtService, IMapper mapper)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _passwordService = passwordService;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        public async Task<(UserDto, string)> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                throw new NotFoundException("Usuário não encontrado.");
            }

            // Verifica a senha
            if (!_passwordService.VerifyPassword(password, user.PasswordHash))
            {
                throw new ValidationException("Senha inválida.");
            }

            UserDto userDto = new UserDto {
                Id = user.Id,
                Username = user.Username
            };

            // Gera o token JWT e o refresh token
            var jwtToken = GenerateJwtToken(userDto);
            
            var refreshToken = GenerateRefreshToken();

            // Armazena o refresh token (associado ao usuário)
            _refreshTokens[refreshToken] = username;

            return await Task.FromResult((userDto,jwtToken));
        }

        public async Task<string> RefreshTokenAsync(string refreshToken)
        {
            return await Task.Run(async () => {
                // Verifica se o refresh token existe e é válido
                //string username = _jwtService.GetUsernameFromToken(refreshToken);
                if (!_refreshTokens.TryGetValue(refreshToken, out var usernameFromToken))
                    throw new UnauthorizedAccessException("Token de atualização inválido.");

                User user = _userRepository.GetByUsernameAsync(usernameFromToken).Result;

                UserDto userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                };

                // Opcional: pode adicionar uma lógica de expiração para o refresh token aqui.

                // Remove o refresh token antigo e gera novos tokens
                _refreshTokens.Remove(refreshToken);
                var newJwtToken = GenerateJwtToken(userDto);
                var newRefreshToken = GenerateRefreshToken();

                // Armazena o novo refresh token
                _refreshTokens[newRefreshToken] = usernameFromToken;

                return await Task.FromResult(newJwtToken);
            });            
        }

        public string GenerateJwtToken(UserDto user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Double.Parse(_configuration["Jwt:ExpirationInMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        
    }
}
