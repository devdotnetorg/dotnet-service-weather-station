using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherStation.Sensors.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WeatherStation.Sensors.Settings;
using WeatherStation.Sensors.Services;
using Microsoft.Extensions.Logging;

namespace WeatherStation.Sensors
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
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
                    if(appSettings.MustbeStopped)
                    {
                        //��������� �������
                        Console.WriteLine("������ ����� ����������.");
                        Environment.Exit(-1);
                    }
                    SingletonAppSettings singletonAppSettings = SingletonAppSettings.Instance;
                    singletonAppSettings.appSettings = appSettings;
                    services.AddSingleton(singletonAppSettings);
                    services.AddScoped(sp => sp.GetService<SingletonAppSettings>().appSettings);
                    //next                    
                    //���������� ������� ������ ��������
                    services.AddScoped<IReadSensorsServices, ReadSensorsFakeServices>();
                    //���������� ������� �������� ������
                    services.AddScoped<ISendData, SendDataToRabbitMQ>();
                    //Main Worker
                    services.AddHostedService<Worker>();
                });
    }
}
