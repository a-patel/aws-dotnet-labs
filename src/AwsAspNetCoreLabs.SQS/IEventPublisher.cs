using AwsAspNetCoreLabs.Models;

namespace AwsAspNetCoreLabs.SQS
{
    public interface IEventPublisher
    {
        void PublishEvent(Event eventData);
    }
}