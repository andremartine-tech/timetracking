using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Services
{
    /// <summary>
    /// Serviço responsável por manipular operações relacionadas a registros de ponto.
    /// </summary>
    public class TimeLogService : ITimeLogService
    {
        private readonly ITimeLogRepository _repository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public TimeLogService(ITimeLogRepository repository, IUserService userService, IMapper mapper, IMemoryCache cache)
        {
            _repository = repository;
            _userService = userService;
            _mapper = mapper;
            _cache = cache;
        }

        /// <inheritdoc />
        public async Task RegisterTimeLogAsync(Guid userId, DateTime timestampIn, DateTime timestampOut)
        {
            if (await _userService.GetUserByIdAsync(userId) == null)
            {
                throw new DomainException("Usuário não encontrado.");
            }

            // Validações de negócio
            if (timestampIn >= timestampOut)
            {
                throw new ValidationException("A hora de entrada deve ser menor que a hora de saída.");
            }

            // Verifica sobreposição
            if (await _repository.HasOverlapAsync(Guid.Empty, timestampIn, timestampOut, userId))
            {
                throw new ValidationException("O intervalo de tempo se sobrepõe a outro registro existente.");
            }

            // Registra o novo ponto
            var timeLog = new TimeLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TimestampIn = timestampIn,
                TimestampOut = timestampOut
            };

            await _repository.AddAsync(timeLog);
        }

        /// <inheritdoc />
        public async Task UpdateTimeLogAsync(Guid id, DateTime timestampIn, DateTime timestampOut)
        {
            TimeLog timeLog = await _repository.GetByIdAsync(id);

            if (timeLog == null)
            {
                throw new DomainException("Registro de ponto não encontrado.");
            }

            // Validações de negócio
            if (timestampIn >= timestampOut)
            {
                throw new ValidationException("A hora de entrada deve ser menor que a hora de saída.");
            }

            // Verifica sobreposição
            if (await _repository.HasOverlapAsync(id, timestampIn, timestampOut, timeLog.UserId))
            {
                throw new ValidationException("O intervalo de tempo se sobrepõe a outro registro existente.");
            }

            timeLog.TimestampIn = timestampIn;
            timeLog.TimestampOut = timestampOut;

            await _repository.UpdateAsync(timeLog);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TimeLogDto>> GetTimeLogsByUserAsync(Guid userId)
        {
            // Cria uma chave específica para cada usuário
            var cacheKey = $"TimeLog_{userId}";

            UserDto user = await _userService.GetUserByIdAsync(userId);

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<TimeLog> timeLogs))
            {
                timeLogs = await _repository.QueryAsync(log => log.UserId == userId);

                if (timeLogs.Any())
                {
                    _cache.Set(cacheKey, timeLogs, TimeSpan.FromMinutes(10));
                } 
                else
                {
                    throw new DomainException("Nenhum registro encontrado para o usuário especificado.");
                }
            }

            return _mapper.Map<IEnumerable<TimeLogDto>>(timeLogs);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TimeLogDto>> GetTimeLogsByUserMonthYearAsync(Guid userId, int month, int year)
        {
            //// Cria uma chave específica para cada usuário
            //var cacheKey = $"TimeLog_{userId}_{month}_{year}";

            //if (!_cache.TryGetValue(cacheKey, out IEnumerable<TimeLog> timeLogs))
            //{
                // Define o intervalo de datas
            var startDate = new DateTime(year, month, 1).ToUniversalTime();
            var endDate = startDate.AddMonths(1).AddTicks(-1).ToUniversalTime(); // Último dia do mês

            // Busca diretamente no banco com intervalo de datas
            IEnumerable<TimeLog> timeLogs = await _repository.QueryAsync(
                log => log.UserId == userId && log.TimestampIn >= startDate && log.TimestampIn <= endDate,
                orderBy: log => log.TimestampIn,
                ascending: true
            );

                //if (timeLogs.Any())
                //{
                //    _cache.Set(cacheKey, timeLogs, TimeSpan.FromMinutes(10));
                //}
                //else
                //{
                //    throw new DomainException("Nenhum registro encontrado para o usuário especificado.");
                //}
            //}

            return _mapper.Map<IEnumerable<TimeLogDto>>(timeLogs);
        }

        /// <inheritdoc />
        public async Task DeleteTimeLogAsync(Guid id)
        {
            TimeLog timeLog = await _repository.GetByIdAsync(id);

            if (timeLog == null)
            {
                throw new DomainException("Registro de ponto não encontrado.");
            }

            await _repository.DeleteAsync(id);
        }

        public async Task DeleteTimeLogsFromUserAsync(Guid UserId)
        {
            await _repository.DeleteFromUserAsync(UserId);
        }
    }
}
