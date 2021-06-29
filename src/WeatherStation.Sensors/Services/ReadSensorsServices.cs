using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.Sensors.Settings;

namespace WeatherStation.Sensors.Services
{
    public class ReadSensorsServices : IReadSensorsServices
    {
        private readonly ILogger<ReadSensorsServices> _logger;
        private readonly AppSettings _appSettings;
        public event EventHandler ButtonChanged;
        public ReadSensorsServices(ILogger<ReadSensorsServices> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _appSettings = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AppSettings>();
            //

        }

        public IDictionary<string, object> ReadAll()
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, object>> ReadAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
