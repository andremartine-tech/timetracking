using Application.DTOs;
using Application.Interfaces;
using Application.UseCases;
using Confluent.Kafka;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public class ProcessMessageDeleteTimeLogUseCase : IProcessMessageDeleteTimeLogUseCase
    {

        private readonly ITimeLogRepository _timeLogRepository;

        public ProcessMessageDeleteTimeLogUseCase(ITimeLogRepository timeLogRepository)
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

                // Verifica se o Id está nulo
                if (timeLog?.Id == null) {
                    throw new ArgumentException("O registro não pode ser nulo.");
                }

                await _timeLogRepository.DeleteAsync(timeLog.Id);

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
