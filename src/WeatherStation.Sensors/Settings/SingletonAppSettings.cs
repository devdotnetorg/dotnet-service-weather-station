using System;

namespace WeatherStation.Sensors.Settings
{
    /// <summary>
    /// Singleton класса настройек прилложения
    /// </summary>
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
