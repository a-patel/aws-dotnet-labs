using RabbitMQ.Client;
using AwsAspNetCoreLabs.Models;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Options;
using AwsAspNetCoreLabs.Models.Settings;
using Microsoft.Extensions.Logging;
using System;

namespace AwsAspNetCoreLabs.SQS
{
    public class RabbitMQEventPublisher : IEventPublisher
    {
        private IModel _channel;
        private string _exchangeName;
        private string _routingKey;
        private readonly ILogger<RabbitMQEventPublisher> _logger;

        public RabbitMQEventPublisher(IOptions<RabbitMQSettings> settings, ILogger<RabbitMQEventPublisher> logger)
        {
            try
            {
                _logger = logger;

                ConnectionFactory factory = new ConnectionFactory()
                {
                    UserName = settings.Value.Username,
                    Password = settings.Value.Password,
                    VirtualHost = settings.Value.VirtualHost,
                    HostName = settings.Value.Hostname,
                    Port = settings.Value.Port
                };

                IConnection connection = factory.CreateConnection();

                _channel = connection.CreateModel();

                _exchangeName = settings.Value.ExchangeName;
                _routingKey = settings.Value.RoutingKey;

                _channel.ExchangeDeclare(settings.Value.ExchangeName, ExchangeType.Direct);
                _channel.QueueDeclare(settings.Value.QueueName, false, false, false, null);
                _channel.QueueBind(settings.Value.QueueName, settings.Value.ExchangeName, settings.Value.RoutingKey, null);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Couldn't create an instance of RabbitMQEventPublisher");
                throw;
            }
        }

        public void PublishEvent(Event eventData)
        {
            try
            {
                byte[] messageBodyBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventData));
                _channel.BasicPublish(_exchangeName, _routingKey, null, messageBodyBytes);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Couldn't publish an event to RabbitMQ");
                throw;
            }
        }
    }
}
