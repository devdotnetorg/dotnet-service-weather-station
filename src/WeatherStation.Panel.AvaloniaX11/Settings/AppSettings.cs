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
