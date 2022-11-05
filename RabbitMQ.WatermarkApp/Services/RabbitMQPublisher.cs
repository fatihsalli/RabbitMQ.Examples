using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RabbitMQ.WatermarkApp.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService _rabbitmqClientService;

        public RabbitMQPublisher(RabbitMQClientService rabbitmqClientService)
        {
            _rabbitmqClientService = rabbitmqClientService;
        }

        public void Publish(ProductImageCreatedEvent productImageCreatedEvent)
        {
            var channel = _rabbitmqClientService.Connect();

            var bodyString=JsonSerializer.Serialize(productImageCreatedEvent);

            var bodyByte=Encoding.UTF8.GetBytes(bodyString);

            //Mesaj memoryde durmayıp fiziksel olarak kalması için;
            var properties=channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(RabbitMQClientService.ExchangeName,RabbitMQClientService.RoutingWaterMark,basicProperties:properties,body:bodyByte);
        }





    }
}
