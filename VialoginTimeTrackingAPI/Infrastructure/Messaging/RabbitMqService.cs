using System.Threading.Channels;
using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;

namespace Infrastructure.Messaging
{
    public class RabbitMqService : IDisposable
    {
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IChannel _channel;

        public RabbitMqService(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IChannel> GetChannelAsync()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                _connection = await _connectionFactory.CreateConnectionAsync();
            }

            if (_channel == null || !_channel.IsOpen)
            {
                _channel = await _connection.CreateChannelAsync();
            }

            return _channel;
        }

        public void Dispose()
        {
            _channel?.CloseAsync();
            _channel?.Dispose();
            _connection?.CloseAsync();
            _connection?.Dispose();
        }
    }
}