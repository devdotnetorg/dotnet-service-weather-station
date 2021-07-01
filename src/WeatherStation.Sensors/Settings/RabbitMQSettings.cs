using System;

namespace WeatherStation.Sensors.Settings
{
    /// <summary>
    /// Настройки для RabbitMQ сервера.
    /// </summary>
    public class RabbitMQSettings
    {
        /// <summary>
        /// Username to use when authenticating to the server.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Password to use when authenticating to the server.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Virtual host to access during this connection.
        /// </summary>
        public string VirtualHost { get; set; }
        /// <summary>
        /// The host to connect to.
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// Default client provided name to be used for connections.
        /// </summary>
        public string ClientProvidedName { get; set; }
        /// <summary>
        /// Время жизни сообщений.
        /// значение 300000 соответствует 5 минутам.
        /// </summary>
        public int x_message_ttl { get; set; }
        /// <summary>
        /// Максимальное количество сообщений в очереди.
        /// </summary>
        public int x_max_length { get; set; }
        /// <summary>
        /// Название обменника.
        /// </summary>
        public string exchangeName { get; set; }
        /// <summary>
        /// Название очереди.
        /// </summary>
        public string queueName { get; set; }
        /// <summary>
        /// Ключ для маршрутизации сообщений.
        /// </summary>
        public string routingKey { get; set; }
    }
}
