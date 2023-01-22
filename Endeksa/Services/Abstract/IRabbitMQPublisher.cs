using Endeksa.Models;

namespace Endeksa.Services.Abstract
{
    public interface IRabbitMQPublisher
    {
        public void Publish(UserIPDetectedEvent userIPDetectedEvent);
    }
}
