using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;

namespace Endeksa.Services
{
    //background servisimiz ilgili kuyruğu dinleyip dataları alacak.consumer
    //
    //
    public class RabbitMQClientService : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        public static string ExchangeName = "IPDirectExchange";
        public static string RoutingIPAddress = "ip-route-detector";
        public static string QueueName = "queue-ip-detector";
        private readonly ILogger _logger;
        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();
            if (_channel is { IsOpen:true })
            {
                return _channel;
            }
            
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(ExchangeName, type: "direct", true, false);

            _channel.QueueDeclare(QueueName, true, false, false, null);

            _channel.QueueBind(queue: QueueName, exchange: ExchangeName, routingKey: RoutingIPAddress);

            _logger.LogInformation("RabbitMQ ile bağlantı kuruldu.");

            return _channel;
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection.Close();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMQ ile bağlantı kesildi.");
        }
    }
}
