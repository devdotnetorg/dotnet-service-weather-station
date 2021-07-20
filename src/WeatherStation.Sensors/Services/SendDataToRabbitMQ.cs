// The MIT License(MIT)
//
// Copyright(c) 2021 Serdyukov Anton <anton@devdotnet.org>
// DevDotNet.Org
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WeatherStation.Sensors.Settings;

namespace WeatherStation.Sensors.Services
{
    public class SendDataToRabbitMQ : ISendData
    {

        private readonly ILogger<SendDataToRabbitMQ> _logger;
        private readonly AppSettings _appSettings;
        public event EventHandler ButtonChanged;
        private static IConnection _conn;
        private static IModel _channel;

        public bool IsOpen => _IsOpen;
        private bool _IsOpen { get; set; }

        public SendDataToRabbitMQ(ILogger<SendDataToRabbitMQ> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _appSettings = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AppSettings>();
            _IsOpen = false;
        }
        
        public void Connect()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    UserName = _appSettings.RabbitMQ.UserName,
                    Password = _appSettings.RabbitMQ.Password,
                    VirtualHost = _appSettings.RabbitMQ.VirtualHost,
                    HostName = _appSettings.RabbitMQ.HostName,
                    ClientProvidedName = _appSettings.RabbitMQ.ClientProvidedName,
                    AutomaticRecoveryEnabled = true,
                    //
                    RequestedHeartbeat = TimeSpan.FromSeconds(30),                
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(15),
                    RequestedConnectionTimeout = TimeSpan.FromSeconds(50)
                };
                _logger.LogInformation("Подключение к серверу RabbitMQ ...");
                _conn = factory.CreateConnection();
                _conn.ConnectionShutdown += Connection_ConnectionShutdown;
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
                //
                this._IsOpen = true;
                _logger.LogInformation("Соединение с сервером RabbitMQ установлено");
            }
            catch (Exception ex)
            {
                this._IsOpen = false;
                _logger.LogError("Не удалось подключиться к серверу RabbitMQ.", ex);
            }
            
        }
        private async void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            this._IsOpen = false;
            _logger.LogError("Соединение с сервером RabbitMQ потеряно");
            Cleanup();
            while (!_IsOpen)
            {
                this.Connect();
                if (!_IsOpen)                
                {
                    _logger.LogInformation("Переподключение через 5 секунд.");
                    await Task.Delay(5000);
                }    
            }
        }
        static void Cleanup()
        {
            try
            {
                if (_channel != null && _channel.IsOpen)
                {
                    _channel.Close();
                    _channel = null;
                }

                if (_conn != null && _conn.IsOpen)
                {
                    _conn.Close();
                    _conn = null;
                }
            }
            catch (IOException ex)
            {
                // Close() may throw an IOException if connection
                // dies - but that's ok (handled by reconnect)
            }
        }

        public void Close()
        {
            if (IsOpen)
            {
                _channel.Close();
                _conn.Close();
            }            
            _channel = null;
            _conn = null;
            _logger.LogInformation("Соединение с сервером RabbitMQ закрыто");
        }

        public bool Send(IDictionary<string, object> message)
        {
            if (!this._IsOpen||message is null)
            {
                _logger.LogError("Показания датчиков не были отправленны на сервер RabbitMQ");
                return false;
            }     
            message.Add("DateTimeNow", DateTime.UtcNow.Ticks);
            string json = JsonConvert.SerializeObject(message, Formatting.Indented);
            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes(json);           
            _channel.BasicPublish(_appSettings.RabbitMQ.exchangeName, _appSettings.RabbitMQ.routingKey, null, messageBodyBytes);
            _logger.LogInformation("Показания датчиков отправлены на сервер RabbitMQ");
            return true;
        }

        public bool Send(string name, object value)
        {
            if (!this._IsOpen)
            {
                _logger.LogError("Показания датчиков не были отправленны на сервер RabbitMQ");
                return false;
            }
            Dictionary<string, object> message = new Dictionary<string, object>();
            message.Add("DateTimeNow", DateTime.UtcNow.Ticks);
            message.Add(name, value);            
            string json = JsonConvert.SerializeObject(message, Formatting.Indented);
            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes(json);
            _channel.BasicPublish(_appSettings.RabbitMQ.exchangeName, _appSettings.RabbitMQ.routingKey, null, messageBodyBytes);            
            _logger.LogInformation($"Показания датчика отправлены на сервер RabbitMQ. Датчик: {name}, значение:{value}");
            return true;
        }
    }
}
