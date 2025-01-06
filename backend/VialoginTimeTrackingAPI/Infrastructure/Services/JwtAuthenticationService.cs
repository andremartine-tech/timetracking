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
using VialoginTimeTrackingAPI.Core.Entities;
using VialoginTimeTrackingAPI.Core.Interfaces;
using VialoginTimeTrackingAPI.Infrastructure.Repositories.Infrastructure.Repositories;
using VialoginTimeTrackingAPI.Application.DTOs;
using VialoginTimeTrackingAPI.Core.Exceptions;
using VialoginTimeTrackingAPI.Infrastructure.Repositories;

namespace Infrastructure.Services
{
    public class JwtAuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private static readonly Dictionary<string, string> _refreshTokens = []; // Simula armazenamento em memória
        private readonly HashSet<string> _invalidTokens;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRevokedTokensRepository _revokedTokensRepository;

        public JwtAuthenticationService(IConfiguration configuration, IUserRepository userRepository, IPasswordService passwordService, IJwtService jwtService, IMapper mapper, IRefreshTokenRepository refreshTokenRepository, IRevokedTokensRepository revokedTokensRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _passwordService = passwordService;
            _jwtService = jwtService;
            _mapper = mapper;
            _invalidTokens = [];
            _refreshTokenRepository = refreshTokenRepository;
            _revokedTokensRepository = revokedTokensRepository;
        }

        public async Task<AuthResponseDto> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username) ?? throw new UnauthorizedAccessExceptionCustom("Usuário não encontrado.");

            if (!_passwordService.VerifyPassword(password, user.PasswordHash))
                throw new UnauthorizedAccessExceptionCustom("Senha inválida.");

            UserDto userDto = new UserDto { Id = user.Id, Username = user.Username };
            var jwtToken = GenerateJwtToken(userDto);
            var refreshToken = GenerateRefreshToken();

            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                Token = refreshToken,
                Username = username,
                Expiration = DateTime.UtcNow.AddDays(7)
            });

            AuthResponseDto authResponseDto = new AuthResponseDto
            {
                User = userDto,
                Token = jwtToken,
                RefreshToken = refreshToken
            };

            return authResponseDto;
        }

        public async Task<string> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken)
                ?? throw new UnauthorizedAccessExceptionCustom("Token inválido.");

            if (storedToken.Expiration < DateTime.UtcNow)
            {
                await _refreshTokenRepository.RemoveAsync(refreshToken);
                throw new UnauthorizedAccessExceptionCustom("Token expirado.");
            }

            var user = await _userRepository.GetByUsernameAsync(storedToken.Username)
                ?? throw new UnauthorizedAccessExceptionCustom("Usuário não encontrado.");

            UserDto userDto = _mapper.Map<UserDto>(user);

            // Remove o token antigo
            await _refreshTokenRepository.RemoveAsync(refreshToken);

            // Gera novos tokens
            var newJwtToken = GenerateJwtToken(userDto);
            var newRefreshToken = GenerateRefreshToken();

            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                Token = newRefreshToken,
                Username = storedToken.Username,
                Expiration = DateTime.UtcNow.AddDays(7)
            });

            return newJwtToken;
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

        public string GetUsernameFromToken(string token)
        {
            return _jwtService.GetUsernameFromToken(token);
        }

        public async Task InvalidateTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token não pode ser nulo ou vazio.", nameof(token));

            var tokenHandler = new JwtSecurityTokenHandler();

            // Verifica se é um JWT
            if (tokenHandler.CanReadToken(token))
            {
                // Invalida o JWT
                await InvalidateJwtTokenAsync(token);
            }
            else
            {
                // Trata como refresh token e remove do banco
                await _refreshTokenRepository.RemoveAsync(token);
            }
        }

        public async Task<bool> IsRefreshTokenValidAsync(string refreshToken)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            return storedToken != null && storedToken.Expiration >= DateTime.UtcNow;
        }

        public async Task<bool> ValidateJwtToken(string token)
        {
            try
            {
                if (!await _revokedTokensRepository.IsTokenRevokedAsync(token))
                {
                    return true;
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return true; // Token válido
            }
            catch
            {
                return false; // Token inválido
            }
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        private async Task InvalidateJwtTokenAsync(string token)
        {
            // Armazena o JWT na tabela de tokens revogados
            await _revokedTokensRepository.AddAsync(new RevokedToken
            {
                Token = token,
                RevokedAt = DateTime.UtcNow
            });
        }
    }
}
