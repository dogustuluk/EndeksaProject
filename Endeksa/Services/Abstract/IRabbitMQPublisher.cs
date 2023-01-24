using Endeksa.Models;

namespace Endeksa.Services.Abstract
{
    public interface IRabbitMQPublisher
    {
        void Publish(UserIPDetectedEvent userIPDetectedEvent);
    }
}
