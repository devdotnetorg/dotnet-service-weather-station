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
