using Endeksa.Models;
using Endeksa.Services.Abstract;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Endeksa.Services.Concrete
{
    public class RabbitMQPublisher : IRabbitMQPublisher
    {
        private readonly IRabbitMQClientService _rabbitMQClientService;

        public RabbitMQPublisher(IRabbitMQClientService rabbitMQClientService)
        {
            _rabbitMQClientService = rabbitMQClientService;
        }

        public void Publish(UserIPDetectedEvent userIPDetectedEvent)
        {
            var channel = _rabbitMQClientService.Connect();
            var bodyString = JsonSerializer.Serialize(userIPDetectedEvent);
            var bodyByte = Encoding.UTF8.GetBytes(bodyString);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.BasicPublish(exchange: RabbitMQClientService.ExchangeName, routingKey: RabbitMQClientService.RoutingIPAddress, basicProperties: properties, body: bodyByte);
        }
    }
}
