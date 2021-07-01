using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherStation.Sensors.Services
{
    /// <summary>
    /// Чтение значений датчиков.
    /// </summary>
    public interface IReadSensorsServices:IDisposable
    {
        /// <summary>
        /// Инициализация датчиков.
        /// </summary>
        /// <returns></returns>
        public bool Init();
        /// <summary>
        /// Чтение значений всех датчиков.
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, object> ReadAll();
        /// <summary>
        /// Чтение значений всех датчиков.
        /// </summary>
        /// <param name="stoppingToken">Токен отмены выполнения задачи.</param>
        /// <returns></returns>
        public Task<Dictionary<string, object>> ReadAllAsync(CancellationToken stoppingToken);
        public event EventHandler ButtonChanged;
    }
}