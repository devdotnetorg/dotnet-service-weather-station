using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherStation.Sensors.Services
{
    public interface IReadSensorsServices:IDisposable
    {
        public bool Init();
        public IDictionary<string, object> ReadAll();
        public Task<Dictionary<string, object>> ReadAllAsync(CancellationToken stoppingToken);
        public event EventHandler ButtonChanged;
    }
}