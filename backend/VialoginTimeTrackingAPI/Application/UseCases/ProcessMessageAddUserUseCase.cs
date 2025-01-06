using Application.DTOs;
using Application.Interfaces;
using Application.Services;
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
    public class ProcessMessageAddUserUseCase : IProcessMessageAddUserUseCase
    {

        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;

        public ProcessMessageAddUserUseCase(IUserRepository userRepository, IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
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

                // Desserializa a mensagem para o User
                var user = JsonSerializer.Deserialize<UserDto>(message);

                // Verifica se o usuário está nulo
                if (string.IsNullOrEmpty(user?.Username.ToString()) || string.IsNullOrEmpty(user.Password.ToString())) {
                    throw new ArgumentException("O nome de usuário e a senha não podem ser nulos.");
                }

                // Verifica se o usuário existe
                if (await _userRepository.GetByUsernameAsync(user.Username) != null)
                {
                    throw new DomainException("Usuário já existe.");
                }

                // Registra o novo ponto
                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Username = user.Username,
                    PasswordHash = _passwordService.HashPassword(user.Password)
                };
                await _userRepository.AddAsync(newUser);

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
