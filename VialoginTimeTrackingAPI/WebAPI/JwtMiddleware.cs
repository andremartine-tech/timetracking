﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPI.Middlewares
{
    /// <summary>
    /// Middleware para validação de tokens JWT em requisições.
    /// </summary>
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        /// <summary>
        /// Invocado para processar a requisição e validar o token JWT.
        /// </summary>
        /// <param name="context">Contexto da requisição HTTP.</param>
        /// <returns>Tarefa assíncrona representando o middleware.</returns>
        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                await AttachUserToContext(context, token);
            }

            await _next(context);
        }

        /// <summary>
        /// Valida o token JWT e anexa as informações do usuário ao contexto.
        /// </summary>
        /// <param name="context">Contexto da requisição HTTP.</param>
        /// <param name="token">Token JWT.</param>
        /// <returns>Tarefa assíncrona.</returns>
        private async Task AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

                // Anexa o usuário ao contexto
                context.Items["UserId"] = userId;
            }
            catch (Exception)
            {
                // Token inválido, não faz nada
            }
        }
    }
}
