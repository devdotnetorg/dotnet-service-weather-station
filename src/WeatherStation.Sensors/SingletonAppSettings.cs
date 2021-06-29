using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherStation.Sensors.Settings;

namespace WeatherStation.Sensors
{
    public class SingletonAppSettings
    {
        public AppSettings appSettings;
        //
        private static readonly Lazy<SingletonAppSettings> lazy = new Lazy<SingletonAppSettings>(() => new SingletonAppSettings());
        private SingletonAppSettings()
        {
        }
        public static SingletonAppSettings Instance => lazy.Value;
    }
}
