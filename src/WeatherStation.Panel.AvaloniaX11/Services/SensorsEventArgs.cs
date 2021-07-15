using System;
using System.Collections.Generic;

namespace WeatherStation.Panel.AvaloniaX11.Services
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
