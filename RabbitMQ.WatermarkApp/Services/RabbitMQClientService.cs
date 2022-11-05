using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;

namespace RabbitMQ.WatermarkApp.Services
{
    public class RabbitMQClientService:IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        public static string ExchangeName = "ImageDirectExchange";
        public static string RoutingWaterMark = "watermark-route-image";
        public static string QueueName = "queue-watermark-image";
        private readonly ILogger<RabbitMQClientService> _logger;

        public RabbitMQClientService(ConnectionFactory connectionFactory,ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            Connect();


        }

        //Geriye bir model yani kanal dönüyoruz.
        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();
            //Kanal var ise direkt kanalı dönüyoruz yeni kanal oluşturmamak adına
            //if (_channel.IsOpen==true) => aşağıdaki ile aynı anlama gelir.
            if (_channel is { IsOpen:true})
            {
                return _channel;
            }
            //Kanal oluşturduk
            _channel = _connection.CreateModel();
            //Exchange oluşturduk
            _channel.ExchangeDeclare(ExchangeName, type: "direct", true, false);
            //Kuyruk oluşturduk
            _channel.QueueDeclare(QueueName,true,false, false,null);
            //Consumer ayrıldığında kuyruğun silinmesi için "QueueBind" metotunu kullanıyoruz.
            _channel.QueueBind(exchange:ExchangeName,queue:QueueName,routingKey:RoutingWaterMark);
            //Log atmak için
            _logger.LogInformation("RabbitMQ ile bağlantı kuruldu...");

            return _channel;
        }

        //_channel kapatma dispose etme gibi ayarlar için "IDisposable" miras aldık.
        public void Dispose()
        {
            //_channel?.Close(); ? kullanımı varsa yani null değilse değer.
            _channel?.Close();
            _channel?.Dispose();
            //Default soldaki değerin default değerini verir. Int olsa 0, string boş,bool false
            //_channel = default;
            _connection?.Close();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMQ ile bağlantı koptu...");

        }
    }
}
