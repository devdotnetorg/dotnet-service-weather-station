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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherStation.Sensors.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;
using WeatherStation.Sensors.Settings;
using WeatherStation.Sensors.Services;

namespace WeatherStation.Sensors
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {                    
                    // build config          
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("config/appsettings.json", optional: false, reloadOnChange: false)
                        .AddEnvironmentVariables()
                        .Build();
                    var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>() ?? new AppSettings();
                    //Get EnvironmentVariables. Read settings for RabbitMQ
                    SettingsHelper.ReadSettingsforRabbitMQ(configuration, appSettings);
                    //Validate            
                    SettingsHelper.ValidateAppSettings(appSettings);
                    if (appSettings.MustbeStopped)
                    {
                        //Остановка приложения
                        Console.WriteLine("Сервис будет остановлен.");
                        Environment.Exit(-1);
                    }
                    //Настройки приложения
                    SingletonAppSettings singletonAppSettings = SingletonAppSettings.Instance;
                    singletonAppSettings.appSettings = appSettings;
                    services.AddSingleton(singletonAppSettings);
                    services.AddScoped(sp => sp.GetService<SingletonAppSettings>().appSettings);               
                    //Добавление сервиса чтения датчиков
                    //Fake
                    //services.AddScoped<IReadSensorsServices, ReadSensorsFakeServices>();
                    //Real
                    services.AddScoped<IReadSensorsServices, ReadSensorsServices>();
                    //Добавление сервиса отправки данных
                    services.AddScoped<ISendData, SendDataToRabbitMQ>();
                    //Main Worker
                    services.AddHostedService<Worker>();
                });
    }
}
