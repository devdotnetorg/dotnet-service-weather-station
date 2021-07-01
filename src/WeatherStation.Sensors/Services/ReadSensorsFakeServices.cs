using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WeatherStation.Sensors.Settings;

namespace WeatherStation.Sensors.Services
{
    public class ReadSensorsFakeServices: IReadSensorsServices
    {
        private readonly ILogger<ReadSensorsFakeServices> _logger;
        private readonly AppSettings _appSettings;
        public event EventHandler ButtonChanged;
        public ReadSensorsFakeServices(ILogger<ReadSensorsFakeServices> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _appSettings = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AppSettings>();            
        }
        public IDictionary<string, object> ReadAll()
        {
            var dictionary = new Dictionary<string, object>();
            dictionary.Add("HomeTemperature", (double) 10.1);
            dictionary.Add("HomePressure", (double) 20.1);
            dictionary.Add("HomeMeters", (double) 30.1);
            dictionary.Add("HomeHumidity", (double) 40.1);
            //event
            OnButtonChanged(new ButtonEventArgs("PressButton1"));
            //
            return dictionary;           
        }
        protected virtual void OnButtonChanged(ButtonEventArgs e)
        {
            ButtonChanged?.Invoke(this, e);
        }
        public Task<Dictionary<string, object>> ReadAllAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                var dictionary = new Dictionary<string, object>();
                dictionary.Add("HomeTemperature", (double)10.1);
                dictionary.Add("HomePressure", (double)20.1);
                dictionary.Add("HomeMeters", (double)30.1);
                dictionary.Add("HomeHumidity", (double)40.1);
                //event
                OnButtonChanged(new ButtonEventArgs("PressButton1"));
                //
                return dictionary;
            });
        }

        public void Dispose()
        {
            _logger.LogInformation("Sensors Dispose");
        }

        public bool Init()
        {
            _logger.LogInformation("Sensors Init");
            return true;
        }
    }
}
