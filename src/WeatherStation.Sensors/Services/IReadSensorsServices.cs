using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherStation.Sensors.Services
{
    public interface IReadSensorsServices
    {
        public IDictionary<string, object> ReadAll();
        public Task<Dictionary<string, object>> ReadAllAsync();
        public event EventHandler ButtonChanged;
    }
}