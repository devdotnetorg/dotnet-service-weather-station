using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherStation.Panel.AvaloniaX11.Services
{
    /// <summary>
    /// Получение значений датчиков от сервера.
    /// </summary>
    interface IGetData:IDisposable
    {        
        /// <summary>
        /// Соединение открыто/закрыто.
        /// </summary>
        public bool IsOpen { get; }
        /// <summary>
        /// Подключение к серверу.
        /// </summary>
        public void Connect(IDictionary<string, object> parameters);
        public Task ConnectAsync(IDictionary<string, object> parameters, CancellationToken stoppingToken);
        /// <summary>
        /// Закрытие соединения с сервером.
        /// </summary>
        public void Close();        
        /// <summary>
        /// Полученые данных с сервера
        /// </summary>
        public event EventHandler DataReceived;
        /// <summary>
        /// Разорвано соединение с сервером
        /// </summary>
        public event EventHandler ConnectionShutdown;
        /// <summary>
        /// Установлено соединение с сервером
        /// </summary>
        public event EventHandler ConnectionOpen;
    }
}
