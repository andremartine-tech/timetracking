using Application.UseCases;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Infrastructure.Messaging.Consumers
{
    public class AddUserWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMqService _rabbitMqService;
        private readonly string _queueName;
        private readonly int _consumerCount;

        public AddUserWorker(IServiceProvider serviceProvider, RabbitMqService rabbitMqService, string queueName, int consumerCount = 5)
        {
            _serviceProvider = serviceProvider;
            _rabbitMqService = rabbitMqService;
            _queueName = queueName;
            _consumerCount = consumerCount;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            for (int i = 0; i < _consumerCount; i++)
            {
                IChannel channel = await _rabbitMqService.GetChannelAsync();
                
                await channel.BasicQosAsync(
                    prefetchSize: 0,
                    prefetchCount: 10,
                    global: false,
                    cancellationToken: stoppingToken
                );

                await Task.Run(() => StartConsumer(i, channel, stoppingToken), stoppingToken);
            }

            return;
        }

        private async void StartConsumer(int consumerId, IChannel channel, CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(channel);
            var scope = _serviceProvider.CreateScope();
            var processMessageAddUserUseCase = scope.ServiceProvider.GetRequiredService<IProcessMessageAddUserUseCase>();

            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    // Processa a mensagem                    
                    await processMessageAddUserUseCase.ExecuteAsync(message, stoppingToken);

                    // Confirma o processamento da mensagem
                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");
                    // TODO: Tratar mensagens não processadas - fila de erros
                }
            };

            // Inicia o consumo da fila
            await channel.BasicConsumeAsync(
                queue: _queueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken
            );

            Console.WriteLine($"Consumidor {consumerId} da fila {_queueName} iniciado.");

            //// Aguarda o encerramento do serviço
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }   
}