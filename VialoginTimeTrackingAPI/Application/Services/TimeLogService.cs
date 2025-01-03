using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Application.Services
{
    /// <summary>
    /// Servi�o respons�vel por manipular opera��es relacionadas a registros de ponto.
    /// </summary>
    public class TimeLogService : ITimeLogService
    {
        private readonly ITimeLogRepository _repository;
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public TimeLogService(ITimeLogRepository repository, IRepository<User> userRepository, IMapper mapper)
        {
            _repository = repository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task RegisterTimeLogAsync(Guid userId, DateTime timestampIn, DateTime timestampOut)
        {
            if (await _userRepository.GetByIdAsync(userId) == null)
            {
                throw new DomainException("Usu�rio n�o encontrado.");
            }

            // Valida��es de neg�cio
            if (timestampIn >= timestampOut)
            {
                throw new ValidationException("A hora de entrada deve ser menor que a hora de sa�da.");
            }

            // Verifica sobreposi��o
            if (await _repository.HasOverlapAsync(Guid.Empty, timestampIn, timestampOut, userId))
            {
                throw new ValidationException("O intervalo de tempo se sobrep�e a outro registro existente.");
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
                throw new DomainException("Registro de ponto n�o encontrado.");
            }

            // Valida��es de neg�cio
            if (timestampIn >= timestampOut)
            {
                throw new ValidationException("A hora de entrada deve ser menor que a hora de sa�da.");
            }

            // Verifica sobreposi��o
            if (await _repository.HasOverlapAsync(id, timestampIn, timestampOut, timeLog.UserId))
            {
                throw new ValidationException("O intervalo de tempo se sobrep�e a outro registro existente.");
            }

            timeLog.TimestampIn = timestampIn;
            timeLog.TimestampOut = timestampOut;

            await _repository.UpdateAsync(timeLog);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TimeLogDto>> GetTimeLogsByUserAsync(Guid userId)
        {
            // Valida se o usu�rio existe
            if (await _userRepository.GetByIdAsync(userId) == null)
            {
                throw new DomainException("Usu�rio n�o encontrado.");
            }

            // Obt�m todos os registros associados ao usu�rio
            var logs = await _repository.QueryAsync(log => log.UserId == userId);

            if (!logs.Any())
            {
                throw new DomainException("Nenhum registro encontrado para o usu�rio especificado.");
            }

            return _mapper.Map<IEnumerable<TimeLogDto>>(logs);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TimeLogDto>> GetTimeLogsByUserMonthYearAsync(Guid userId, int month, int year)
        {
            // Valida se o usu�rio existe
            if (await _userRepository.GetByIdAsync(userId) == null)
            {
                throw new DomainException("Usu�rio n�o encontrado.");
            }

            // Obt�m todos os registros associados ao usu�rio
            var logs = await _repository.QueryAsync(log => log.UserId == userId && log.TimestampIn.Month == month && log.TimestampIn.Year == year, orderBy: log => log.TimestampIn, ascending: true);

            if (!logs.Any())
            {
                throw new DomainException("Nenhum registro encontrado para o usu�rio no per�odo especificado.");
            }

            return _mapper.Map<IEnumerable<TimeLogDto>>(logs);
        }

        /// <inheritdoc />
        public async Task DeleteTimeLogAsync(Guid id)
        {
            TimeLog timeLog = await _repository.GetByIdAsync(id);

            if (timeLog == null)
            {
                throw new DomainException("Registro de ponto n�o encontrado.");
            }

            await _repository.DeleteAsync(id);
        }

        public async Task DeleteTimeLogsFromUserAsync(Guid UserId)
        {
            await _repository.DeleteFromUserAsync(UserId);
        }
    }
}
