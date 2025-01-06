using Application.DTOs;
using Application.Interfaces;
using Application.UseCases;
using Confluent.Kafka;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public class ProcessMessageUpdateTimeLogUseCase : IProcessMessageUpdateTimeLogUseCase
    {

        private readonly ITimeLogRepository _timeLogRepository;

        public ProcessMessageUpdateTimeLogUseCase(ITimeLogRepository timeLogRepository)
        {
            _timeLogRepository = timeLogRepository;
        }

        public async Task ExecuteAsync(string message, CancellationToken cancellationToken)
        {
            try
            {
                // Verifica se a mensagem não está nula ou vazia
                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new ArgumentException("A mensagem não pode ser nula ou vazia.");
                }

                // Desserializa a mensagem para o TimeLogDto
                var timeLog = JsonSerializer.Deserialize<TimeLogDto>(message);

                // Verifica se o log está nulo
                if (timeLog?.UserId == null || timeLog?.TimestampIn == null || timeLog?.TimestampOut == null) {
                    throw new ArgumentException("O registro não pode ser nulo.");
                }

                // Verifica se o usuário existe
                if (await _timeLogRepository.GetByUserIdAsync(timeLog.UserId) == null)
                {
                    throw new DomainException("Usuário não encontrado.");
                }

                // Verifica se o horário inicial é menor que o final
                if (timeLog.TimestampIn >= timeLog.TimestampOut)
                {
                    throw new ValidationException("A hora de entrada deve ser menor que a hora de saída.");
                }

                // Verifica sobreposição com outros horários
                //if (await _timeLogRepository.HasOverlapAsync(Guid.Empty, timeLog.TimestampIn, timeLog.TimestampOut, timeLog.UserId))
                //{
                //    throw new ValidationException("O intervalo de tempo se sobrepõe a outro registro existente.");
                //}

                // Registra o novo ponto
                var updatedTimeLog = new TimeLog
                {
                    Id = timeLog.Id,
                    UserId = timeLog.UserId,
                    TimestampIn = timeLog.TimestampIn,
                    TimestampOut = timeLog.TimestampOut
                };

                await _timeLogRepository.UpdateDetachedAsync(updatedTimeLog);
                
                //Console.WriteLine($"Mensagem processada com sucesso: {message}");
            }
            catch (Exception ex)
            {
                // Lida com erros de processamento
                Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");
                throw;
            }
        }
    }
}
