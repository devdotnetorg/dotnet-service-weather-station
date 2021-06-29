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
            _sendData.Connect();
            //
            int TaskDelay = _appSettings.Sensors.ReadEvery * 1000;
            //Event
            _readSensorsServices.ButtonChanged += button_OnChange;
            //
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //Read sensors
                var values=_readSensorsServices.ReadAll();
                _logger.LogDebug($"Значения датчиков: {AppHelper.DictionaryToString(values)}");                                
                _sendData.Send(values);                         
                //
                await Task.Delay(TaskDelay, stoppingToken);
            }
            //
            _sendData.Close();
        }
        void button_OnChange(object sender, EventArgs e)
        {
            var obj = (ButtonEventArgs)e;                                 
            _sendData.Send("onButton", obj._Name);
        }
    }
}
