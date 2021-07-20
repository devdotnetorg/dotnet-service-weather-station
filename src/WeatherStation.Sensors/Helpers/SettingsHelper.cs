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

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WeatherStation.Sensors.Settings;

namespace WeatherStation.Sensors.Helpers
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
            var value= configuration["RabbitMQUserName"];
            if (value is not null) appSettings.RabbitMQ.UserName = value;
            value = configuration["RabbitMQPassword"];
            if (value is not null) appSettings.RabbitMQ.Password = value;
        }
    } 
}
