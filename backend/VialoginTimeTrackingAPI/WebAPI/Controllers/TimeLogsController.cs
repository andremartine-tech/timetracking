using Microsoft.AspNetCore.Mvc;
using Application.Features.TimeLogs.Commands;
using Application.Features.TimeLogs.Queries;
using MediatR;
using RabbitMQ.Client;
using Application.DTOs;
using System.Text;
using Infrastructure.Messaging;
using VialoginTestTimeTrackingAPI.Application.Interfaces;
using VialoginTestTimeTrackingAPI.Infrastructure.BackgroundJobs;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controlador responsável por operações com registros de ponto.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TimeLogsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly RabbitMqService _rabbitMqService;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private BasicProperties basicProperties;

        public TimeLogsController(IMediator mediator, RabbitMqService rabbitMqService, IBackgroundTaskQueue backgroundTaskQueue)
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
        /// Registra um ponto.
        /// </summary>
        /// <param name="timeLogDto">Dto com as informações do registro de ponto.</param>
        /// <returns>Confirmação do envio do registro para processamento.</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddTimeLog([FromBody] TimeLogDto timeLogDto)
        {
            if (!ValidAddRequest(timeLogDto))
            {
                return BadRequest(new { error = "Dados inválidos para registro de ponto." });
            }
            
            IChannel channel = await _rabbitMqService.GetChannelAsync();

            StartTaskRun(timeLogDto, channel, "add_timelog_queue");

            return Ok(new { status = "Ponto enviado para processamento" });
        }

        /// <summary>
        /// Atualiza um ponto.
        /// </summary>
        /// <param name="timeLogDto">Dto com as informações do registro de ponto.</param>
        /// <returns>Confirmação do envio do registro para processamento.</returns>
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateTimeLog([FromBody] TimeLogDto timeLogDto)
        {
            if (!ValidUpdateRequest(timeLogDto))
            {
                return BadRequest(new { error = "Dados inválidos para registro de ponto." });
            }

            IChannel channel = await _rabbitMqService.GetChannelAsync();
            
            StartTaskRun(timeLogDto, channel, "update_timelog_queue");

            return Ok(new { status = "Ponto enviado para processamento" });
        }

        /// <summary>
        /// Obtém os registros de ponto de um usuário.
        /// </summary>
        /// <param name="userId">ID do usuário.</param>
        /// <returns>Lista de registros de ponto.</returns>
        [Authorize]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetTimeLogsByUser(Guid userId)
        {
            var query = new GetTimeLogsByUserQuery { UserId = userId };
            var timeLogs = await _mediator.Send(query);
            return Ok(timeLogs);
        }

        /// <summary>
        /// Exclui um registro de ponto.
        /// </summary>
        /// <param name="id">ID do registro de ponto.</param>
        /// <returns>Confirmação da ação.</returns>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeLog(Guid id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest(new { error = "Dados inválidos para exclusão de ponto." });
            }

            IChannel channel = await _rabbitMqService.GetChannelAsync();

            var timeLog = new TimeLogDto
            {
                Id = id,
            };

            StartTaskRun(timeLog, channel, "delete_timelog_queue");

            return Ok(new { status = "Ponto enviado para processamento" });
        }

        /// <summary>
        /// Exclui um registro de ponto.
        /// </summary>
        /// <param name="id">ID do registro de ponto.</param>
        /// <returns>Confirmação da ação.</returns>
        [Authorize]
        [HttpDelete("cleartests/{id}")]
        public async Task<IActionResult> DeleteTimeLogsFromUser(Guid userId)
        {
            var command = new DeleteTimeLogsFromUserCommand(userId);
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Obtém os registros de ponto de um usuário para o mês e ano de referência.
        /// </summary>
        /// <param name="userId">ID do usuário.</param>
        /// <param name="dateTime">DateTime contento o mês e ano de referência.</param>
        /// <returns>Lista de registros de ponto.</returns>
        [Authorize]
        [HttpGet("{userId}/{month}/{year}")]
        public async Task<IActionResult> GetTimeLogsByUserMonthYear(Guid userId, int month, int year)
        {
            var query = new GetTimeLogsByUserMonthYearQuery { UserId = userId, Month = month, Year = year };
            var timeLogs = await _mediator.Send(query);
            return Ok(timeLogs);
        }

        private bool ValidAddRequest(TimeLogDto timeLogDto)
        {
            return (timeLogDto != null && 
                !string.IsNullOrEmpty(timeLogDto.UserId.ToString()) && 
                !string.IsNullOrEmpty(timeLogDto.TimestampIn.ToString()) && 
                !string.IsNullOrEmpty(timeLogDto.TimestampOut.ToString())
            );
        }

        private bool ValidUpdateRequest(TimeLogDto timeLogDto)
        {
            return (timeLogDto != null &&
                !string.IsNullOrEmpty(timeLogDto.Id.ToString()) &&
                !string.IsNullOrEmpty(timeLogDto.UserId.ToString()) &&
                !string.IsNullOrEmpty(timeLogDto.TimestampIn.ToString()) &&
                !string.IsNullOrEmpty(timeLogDto.TimestampOut.ToString())
            );
        }

        private void StartTaskRun(TimeLogDto timeLogDto, IChannel channel, string queueName)
        {
            _backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
            {
                var mensagem = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(timeLogDto));

                await channel.BasicPublishAsync(exchange: "", routingKey: queueName, mandatory: true, basicProperties: basicProperties, body: mensagem, cancellationToken: token);
            });
        }
    }
}
