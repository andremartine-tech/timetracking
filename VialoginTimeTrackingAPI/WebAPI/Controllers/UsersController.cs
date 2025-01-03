using Microsoft.AspNetCore.Mvc;
using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using MediatR;
using Infrastructure.Messaging;
using RabbitMQ.Client;
using VialoginTestTimeTrackingAPI.Infrastructure.BackgroundJobs;
using VialoginTestTimeTrackingAPI.Application.Interfaces;
using Application.DTOs;
using System.Text;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controlador responsável por operações com usuários.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly RabbitMqService _rabbitMqService;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private BasicProperties basicProperties;

        public UsersController(IMediator mediator, RabbitMqService rabbitMqService, IBackgroundTaskQueue backgroundTaskQueue)
        {
            _mediator = mediator;
            _rabbitMqService = rabbitMqService;
            _backgroundTaskQueue = backgroundTaskQueue;
            basicProperties = new BasicProperties
            {
                Persistent = false,
                ContentType = "application/json"
            };
        }

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        /// <param name="userDto">Dto com os dados do usuário.</param>
        /// <returns>Id do usuário criado.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            if (!ValidAddRequest(userDto))
            {
                return BadRequest(new { error = "Dados inválidos para registro de usuário." });
            }

            IChannel channel = await _rabbitMqService.GetChannelAsync();

            StartTaskRun(userDto, channel, "add_user_queue");

            return Ok(new { status = "Usuário enviado para processamento" });
        }

        /// <summary>
        /// Obtém os detalhes de um usuário pelo ID.
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <returns>Informações do usuário.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var query = new GetUserByIdQuery { Id = id };
            var user = await _mediator.Send(query);
            return Ok(user);
        }

        /// <summary>
        /// Obtém todos os usuários.
        /// </summary>
        /// <returns>Lista de usuários.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _mediator.Send(new GetAllUsersQuery());
            return Ok(users);
        }

        private void StartTaskRun(UserDto userDto, IChannel channel, string queueName)
        {
            _backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
            {
                var mensagem = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(userDto));

                await channel.BasicPublishAsync(exchange: "", routingKey: queueName, mandatory: true, basicProperties: basicProperties, body: mensagem, cancellationToken: token);
            });
        }

        private bool ValidAddRequest(UserDto userDto)
        {
            return (userDto != null &&
                !string.IsNullOrEmpty(userDto.Username) &&
                !string.IsNullOrEmpty(userDto.Password)
            );
        }
    }
}
