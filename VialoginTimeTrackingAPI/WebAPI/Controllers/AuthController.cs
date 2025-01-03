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

            return Ok(new AuthResponseDto
            {
                User = response.Item1,
                Token = response.Item2
            });
        }

        /// <summary>
        /// Valida se um usuário está logado
        /// </summary>
        [HttpGet("verify")]
        [Authorize]
        public async Task<IActionResult> Verify()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { message = "Usuário não autenticado." });
            }

            var query = new GetUserByUsernameQuery { Username = username };
            var user = await _mediator.Send(query);

            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado." });
            }

            UserDto userDto = new UserDto
            {
                Id = user.Id,
                Username = username
            };

            var token = _jwtAuthenticationService.GenerateJwtToken(userDto);

            return Ok(new
            {
                User = user,
                Token = token
            });
        }
    }
}
