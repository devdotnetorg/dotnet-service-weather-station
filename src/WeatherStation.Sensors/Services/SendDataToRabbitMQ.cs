using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.Sensors.Settings;

namespace WeatherStation.Sensors.Services
{
    public class SendDataToRabbitMQ : ISendData
    {

        private readonly ILogger<SendDataToRabbitMQ> _logger;
        private readonly AppSettings _appSettings;
        public event EventHandler ButtonChanged;
        private IConnection _conn;
        private IModel _channel;
        public SendDataToRabbitMQ(ILogger<SendDataToRabbitMQ> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _appSettings = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AppSettings>();
        }
        // _logger.LogDebug($"Значения датчиков: {AppHelper.DictionaryToString(values)}");                
        //_logger.LogInformation("Отправка значений датчиков");
        
        public void Connect()
        {
            _logger.LogInformation("Подключение к серверу отправки данных");
            ConnectionFactory factory = new ConnectionFactory();            
            factory.UserName = _appSettings.RabbitMQ.UserName;
            factory.Password = _appSettings.RabbitMQ.Password;
            factory.VirtualHost = _appSettings.RabbitMQ.VirtualHost;
            factory.HostName = _appSettings.RabbitMQ.HostName;
            factory.ClientProvidedName = _appSettings.RabbitMQ.ClientProvidedName;
            _conn = factory.CreateConnection();
            _channel = _conn.CreateModel();
            //Exchange
            string exchangeName = _appSettings.RabbitMQ.exchangeName;
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            //Queue
            string queueName = _appSettings.RabbitMQ.queueName, routingKey = _appSettings.RabbitMQ.routingKey;
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            //x-message-ttl = 5 min = 300000
            arguments.Add("x-message-ttl", _appSettings.RabbitMQ.x_message_ttl);
            //x-max-length = 5
            arguments.Add("x-max-length", _appSettings.RabbitMQ.x_max_length);
            _channel.QueueDeclare(queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: arguments
            );

            _channel.QueueBind(queue: queueName,
                exchange: exchangeName,
                routingKey: routingKey,
                arguments: null
            );
        }

        public Task ConnectAsync()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            _logger.LogInformation("Закрытие соединения с сервером отправки данных");
            _channel.Close();
            _conn.Close();            
        }

        public Task CloseAsync()
        {
            throw new NotImplementedException();
        }

        public Task SendAsync()
        {
            throw new NotImplementedException();





        }

        public void Send(IDictionary<string, object> message)
        {
            _logger.LogInformation("Отправка значений датчиков");
            message.Add("DateTimeNow", DateTime.UtcNow.ToString());
            string json = JsonConvert.SerializeObject(message, Formatting.Indented);
            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes(json);
            _channel.BasicPublish(_appSettings.RabbitMQ.exchangeName, _appSettings.RabbitMQ.routingKey, null, messageBodyBytes);
        }

        public void Send(string name, object value)
        {
            _logger.LogInformation($"Отправка значения датчика: {name}, значение:{value}");
            Dictionary<string, object> message = new Dictionary<string, object>();
            message.Add("DateTimeNow", DateTime.UtcNow.ToString());
            message.Add(name, value);            
            string json = JsonConvert.SerializeObject(message, Formatting.Indented);
            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes(json);
            _channel.BasicPublish(_appSettings.RabbitMQ.exchangeName, _appSettings.RabbitMQ.routingKey, null, messageBodyBytes);
        }
    }
}
