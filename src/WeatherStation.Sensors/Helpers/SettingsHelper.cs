using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.Sensors.Settings;

namespace WeatherStation.Sensors.Helpers
{
    public static class SettingsHelper
    {
        public static void ValidateAppSettings(AppSettings appSettings)
        {
            var resultsValidation = new List<ValidationResult>();
            var context = new ValidationContext(appSettings);
            if (!Validator.TryValidateObject(appSettings, context, resultsValidation, true))
            {
                resultsValidation.ForEach(
                    error => Console.WriteLine($"Проверка конфигурации: {error.ErrorMessage}"));
            }
        }
        public static void ReadSettingsforRabbitMQ(IConfigurationRoot configuration, AppSettings appSettings)
        {            
            //Get EnvironmentVariables
            //Read settings for RabbitMQ
            var value= configuration["RabbitMQUserName"];
            if (value is not null) appSettings.RabbitMQ.UserName = value;
            value = configuration["RabbitMQPassword"];
            if (value is not null) appSettings.RabbitMQ.Password = value;
        }
    }
 
}
