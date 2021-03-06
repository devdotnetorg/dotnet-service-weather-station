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

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WeatherStation.Panel.AvaloniaX11.Services
{
    class GetDataFromRabbitMQ : IGetData
    {
        public bool IsOpen => _IsOpen;
        private bool _IsOpen { get; set; }
        private IDictionary<string, object> _parameters;
        private ConnectionFactory _factory;
        private IConnection _conn;
        private IModel _channel;

        public event EventHandler DataReceived;
        public event EventHandler ConnectionShutdown;
        public event EventHandler ConnectionOpen;

        public void Close()
        {
            Console.WriteLine("Закрытие соединения с сервером RabbitMQ ...");
            if (IsOpen)
            {
                _channel.Close();
                _conn.Close();
            }
            _channel = null;
            _conn = null;
            Console.WriteLine("Соединение с сервером RabbitMQ закрыто");            
        }

        public void Connect(IDictionary<string, object> parameters)
        {
            _parameters = parameters;
            CreateConnectionFactory();
            //Connect
            ConnectToRabbitMQ();
        }

        public Task ConnectAsync(IDictionary<string, object> parameters, CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                _parameters = parameters;
                CreateConnectionFactory();
                //Connect
                while (!_IsOpen)
                {                   
                    ConnectToRabbitMQ();                 
                    if (!_IsOpen)
                    {
                        Console.WriteLine("Переподключение через 10 секунд.");
                        Task.Delay(10000, stoppingToken).Wait();
                    }
                    if (stoppingToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Операция прервана");
                        return;
                    }
                }   
            });
        }

        private void ConnectToRabbitMQ()
        {
            try
            {                
                _conn = _factory.CreateConnection();
                _conn.ConnectionShutdown += Connection_ConnectionShutdown;
                Console.WriteLine("Подключение к серверу RabbitMQ ...");
                _channel = _conn.CreateModel();                
                _IsOpen = true;
                //Срабатывание события
                OnConnectionOpen();
                Console.WriteLine("Соединение с сервером RabbitMQ установлено");
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (ch, ea) =>
                {                                        
                    string strBody = Encoding.UTF8.GetString(ea.Body.ToArray());
                    Console.WriteLine($" Get message: {strBody}\n");
                    var sensors=ParseBody(strBody);
                    OnDataReceived(new SensorsEventArgs(sensors));
                    _channel.BasicAck(ea.DeliveryTag, false);
                };
                //Queue
                string queueName = (string)_parameters["QueueName"];                
                _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            }
            catch (Exception ex)
            {
                _IsOpen = false;
                Console.WriteLine("Не удалось подключиться к серверу RabbitMQ.", ex);
            }
        }
        private void CreateConnectionFactory()
        {
            _factory = new ConnectionFactory
            {
                UserName = (string)_parameters["UserName"],
                Password = (string)_parameters["Password"],
                VirtualHost = (string)_parameters["VirtualHost"],
                HostName = (string)_parameters["HostName"],
                ClientProvidedName = (string)_parameters["ClientProvidedName"],
                AutomaticRecoveryEnabled = true,
                //
                RequestedHeartbeat = TimeSpan.FromSeconds(30),
                NetworkRecoveryInterval = TimeSpan.FromSeconds(15),
                RequestedConnectionTimeout = TimeSpan.FromSeconds(50)
            };
        }

        private async void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _IsOpen = false;
            Console.WriteLine("Соединение с сервером RabbitMQ потеряно");
            //Срабатывание события
            OnConnectionShutdown();
            Cleanup();
            while (!_IsOpen)
            {
                //Connect
                ConnectToRabbitMQ();
                if (!_IsOpen)
                {
                    Console.WriteLine("Переподключение через 10 секунд.");
                    await Task.Delay(10000);
                }
            }
        }
        private void Cleanup()
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
            catch (Exception ex)
            {
                // Close() may throw an IOException if connection
                // dies - but that's ok (handled by reconnect)
            }
        }

        public void Dispose()
        {
            Close();
        }
        protected virtual void OnConnectionShutdown()
        {
            ConnectionShutdown?.Invoke(this, new EventArgs());
        }
        protected virtual void OnConnectionOpen()
        {
            ConnectionOpen?.Invoke(this, new EventArgs());
        }
        protected virtual void OnDataReceived(SensorsEventArgs e)
        {
            DataReceived?.Invoke(this, e);
        }
        private IDictionary<string, object> ParseBody (string strBody)
        {            
            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(strBody);
            return values;
        }
    }
}
