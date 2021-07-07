using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeatherStation.Sensors.Helpers;
using WeatherStation.Sensors.Services;
using WeatherStation.Sensors.Settings;

namespace WeatherStation.Sensors
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly AppSettings _appSettings;
        private readonly IReadSensorsServices _readSensorsServices;
        private readonly ISendData _sendData;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _appSettings = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AppSettings>();
            _readSensorsServices=serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IReadSensorsServices>();
            _sendData = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ISendData>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{DateTime.Now}: Запуск сервиса ...");
            if (!_readSensorsServices.Init())
            {
                //Остановка приложения
                _logger.LogError("Инициализация датчиков не выполнена");
                _logger.LogInformation("Сервис будет остановлен");
                Environment.Exit(-2);
            }            
            while (!_sendData.IsOpen)
            {
                _sendData.Connect();                
                if(!_sendData.IsOpen) _logger.LogInformation("Переподключение через 10 секунд.");
                await Task.Delay(10000, stoppingToken);
                if (stoppingToken.IsCancellationRequested) return;
            }
            int TaskDelay = _appSettings.Sensors.ReadEvery * 1000;
            //Add event
            _readSensorsServices.ButtonChanged += button_OnChange;
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //Read sensors
                var values = await _readSensorsServices.ReadAllAsync(stoppingToken);                
                _logger.LogDebug($"Значения датчиков: {AppHelper.DictionaryToString(values)}");
                try
                {
                    _sendData.Send(values);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Показания датчиков не были отправленны на сервер", ex);
                }
                await Task.Delay(TaskDelay, stoppingToken);
            }
            _sendData.Close();
            _readSensorsServices.Dispose();
        }
        void button_OnChange(object sender, EventArgs e)
        {
            var obj = (ButtonEventArgs)e;
            try
            {
                _sendData.Send("Command", obj._Name);
            }
            catch (Exception ex)
            {
                _logger.LogError("Событие от датчика не было отправленно на сервер", ex);
            }            
        }
    }
}
