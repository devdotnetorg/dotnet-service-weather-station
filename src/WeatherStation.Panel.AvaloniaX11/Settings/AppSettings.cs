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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeatherStation.Panel.AvaloniaX11.Settings
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
        public RabbitMQSettings RabbitMQ { get; set; }

        public AppSettings()
        {
            RabbitMQ = new RabbitMQSettings();
            _MustbeStopped = false;
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();             
            if (string.IsNullOrWhiteSpace(RabbitMQ.ClientProvidedName))
            {
                errors.Add(new ValidationResult("Не указан параметр RabbitMQ.ClientProvidedName. Задано " +
                    "значение по умолчанию app:sensors component:event-consumer"));
                RabbitMQ.ClientProvidedName = "app:sensors component:event-consumer";
            }
            
            if (string.IsNullOrWhiteSpace(RabbitMQ.HostName)
                && string.IsNullOrWhiteSpace(RabbitMQ.Password)
                && string.IsNullOrWhiteSpace(RabbitMQ.QueueName)
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
