using System;

namespace WeatherStation.Panel.Settings
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
        /// Название очереди.
        /// </summary>
        public string QueueName { get; set; }

    }
}
