using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.Panel.AvaloniaX11.Settings;

namespace WWeatherStation.Panel.AvaloniaX11.Helpers
{
    public static class SettingsHelper
    {
        /// <summary>
        /// Проверка конфигурации
        /// </summary>
        /// <param name="appSettings"></param>
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
        /// <summary>
        /// Проверка заданных значений Environment логина, пароля для клиента RabbitMQ
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="appSettings"></param>
        public static void ReadSettingsforRabbitMQ(IConfigurationRoot configuration, AppSettings appSettings)
        {
            //Get EnvironmentVariables. Read settings for RabbitMQ
            var value = configuration["RabbitMQUserName"];
            if (value is not null) appSettings.RabbitMQ.UserName = value;
            value = configuration["RabbitMQPassword"];
            if (value is not null) appSettings.RabbitMQ.Password = value;
        }
    }
}
