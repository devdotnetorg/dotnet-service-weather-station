using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherStation.Sensors.Settings
{
    public class RabbitMQSettings
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public string HostName { get; set; }
        public string ClientProvidedName { get; set; }
        public int x_message_ttl { get; set; }
        public int x_max_length { get; set; }
        public string exchangeName { get; set; }
        public string queueName { get; set; }
        public string routingKey { get; set; }

    }
}
