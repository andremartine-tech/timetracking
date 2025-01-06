using Microsoft.AspNetCore.Mvc;
using Application.Features.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Application.Features.Users.Queries;
using Infrastructure.Services;
using Core.Interfaces;
using Application.DTOs;
using VialoginTimeTrackingAPI.Application.DTOs;
using Core.Entities;
using Newtonsoft.Json.Linq;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controlador responsável por autenticação.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAuthenticationService _jwtAuthenticationService;

        public AuthController(IMediator mediator, IAuthenticationService jwtAuthenticationService)
        {
            _mediator = mediator;
            _jwtAuthenticationService = jwtAuthenticationService;
        }

        /// <summary>
        /// Autentica um usuário e retorna um token JWT.
        /// </summary>
        /// <param name="command">Comando contendo o username e password.</param>
        /// <returns>Token JWT.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        /// <summary>
        /// Valida se um usuário está logado
        /// </summary>
        [HttpGet("verify")]
        public async Task<IActionResult> Verify()
        {
            // Obtém o token JWT do cabeçalho de autorização
            var authorizationHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { message = "Token não fornecido." });
            }

            var token = authorizationHeader.Replace("Bearer ", "");

            // Verifica se o token é válido
            if (!await _jwtAuthenticationService.ValidateJwtToken(token))
            {
                return Unauthorized(new { message = "Token inválido ou expirado." });
            }

            // Obtém o nome de usuário do token
            var username = _jwtAuthenticationService.GetUsernameFromToken(token);
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { message = "Usuário não autenticado." });
            }

            // Busca o usuário no banco de dados
            var query = new GetUserByUsernameQuery { Username = username };
            var user = await _mediator.Send(query);

            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado." });
            }

            // Gera um novo token JWT
            UserDto userDto = new UserDto
            {
                Id = user.Id,
                Username = username
            };
            var newJwtToken = _jwtAuthenticationService.GenerateJwtToken(userDto);

            return Ok(new
            {
                User = user,
                Token = newJwtToken
            });
        }

        /// <summary>
        /// Verifica se o refresh token é válido e retorna o token.
        /// </summary>
        /// <param name="refreshToken">Refresh Token a ser verificado.</param>
        /// <returns>Novo Token JWT.</returns>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                var newJwtToken = await _jwtAuthenticationService.RefreshTokenAsync(refreshToken);
                return Ok(new { Token = newJwtToken });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }


        /// <summary>
        /// Logout do usuário atual, invalidando o token JWT.
        /// </summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string token)
        {
            await _jwtAuthenticationService.InvalidateTokenAsync(token);
            return Ok(new { message = "Logout realizado com sucesso." });
        }
    }
}
