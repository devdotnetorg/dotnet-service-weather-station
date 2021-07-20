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

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeatherStation.Sensors.Settings
{
    /// <summary>
    /// Настройки приложения.
    /// </summary>
    public class AppSettings : IValidatableObject
    {
        /// <summary>
        /// Приложение должно быть остановленно.
        /// </summary>
        public bool MustbeStopped => _MustbeStopped;
        private bool _MustbeStopped { get; set; }
        public SensorsSettings Sensors { get; set; }
        public RabbitMQSettings RabbitMQ { get; set; }

        public AppSettings()
        {
            Sensors = new SensorsSettings();
            RabbitMQ = new RabbitMQSettings();
            _MustbeStopped = false;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            if (Sensors.ReadEvery ==0)
            {
                errors.Add(new ValidationResult("Не указан параметр Sensors.ReadEvery. Задано " +
                    "значение по умолчанию = 20"));
                Sensors.ReadEvery = 20;
            }            
            if (string.IsNullOrWhiteSpace(RabbitMQ.ClientProvidedName))
            {
                errors.Add(new ValidationResult("Не указан параметр RabbitMQ.ClientProvidedName. Задано " +
                    "значение по умолчанию app:sensors component:event-consumer"));
                RabbitMQ.ClientProvidedName = "app:sensors component:event-consumer";
            }
            if (RabbitMQ.x_max_length==0)
            {
                errors.Add(new ValidationResult("Не указан параметр RabbitMQ.x_max_length. Задано " +
                    "значение по умолчанию = 5"));
                RabbitMQ.x_max_length = 5;
            }
            if (RabbitMQ.x_message_ttl == 0)
            {
                errors.Add(new ValidationResult("Не указан параметр RabbitMQ.x_message_ttl. Задано " +
                    "значение по умолчанию = 300000"));
                RabbitMQ.x_message_ttl = 300000;
            }
            if (string.IsNullOrWhiteSpace(RabbitMQ.exchangeName)
                && string.IsNullOrWhiteSpace(RabbitMQ.HostName)
                && string.IsNullOrWhiteSpace(RabbitMQ.Password)
                && string.IsNullOrWhiteSpace(RabbitMQ.queueName)
                && string.IsNullOrWhiteSpace(RabbitMQ.routingKey)
                && string.IsNullOrWhiteSpace(RabbitMQ.UserName)
                && string.IsNullOrWhiteSpace(RabbitMQ.VirtualHost))
            {
                errors.Add(new ValidationResult("Не указаны все необходимые параметры для подключения к серверу RabbitMQ."));
                _MustbeStopped = true;
            }
            return errors;
        }
    }
}
