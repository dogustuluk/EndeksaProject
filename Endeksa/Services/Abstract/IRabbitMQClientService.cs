using RabbitMQ.Client;

namespace Endeksa.Services.Abstract
{
    public interface IRabbitMQClientService
    {
        IModel Connect();
        void Dispose();
    }
}
