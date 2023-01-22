using RabbitMQ.Client;

namespace Endeksa.Services.Abstract
{
    public interface IRabbitMQClientService
    {
        public IModel Connect();
        public void Dispose();
    }
}
