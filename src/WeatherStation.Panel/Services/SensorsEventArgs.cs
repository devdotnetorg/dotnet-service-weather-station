using System;
using System.Collections.Generic;

namespace WeatherStation.Panel.Services
{
    public class SensorsEventArgs : EventArgs
    {
        public IDictionary<string, object> _sensors { get; }
        public SensorsEventArgs(IDictionary<string, object> sensors)
        {
            _sensors = sensors;
        }
    }    
}
