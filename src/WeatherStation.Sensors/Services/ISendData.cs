using System.Collections.Generic;

namespace WeatherStation.Sensors.Services
{
    /// <summary>
    /// Отправка значений датчиков серверу.
    /// </summary>
    public interface ISendData
    {
        /// <summary>
        /// Соединение открыто/закрыто.
        /// </summary>
        public bool IsOpen { get; }
        /// <summary>
        /// Подключение к серверу.
        /// </summary>
        public void Connect();
        /// <summary>
        /// Закрытие соединения с сервером.
        /// </summary>
        public void Close();
        /// <summary>
        /// Отправка данных на сервер
        /// </summary>
        /// <param name="message">значения в формате ключ/значение.</param>
        /// <returns></returns>
        public bool Send(IDictionary<string, object> message);
        /// <summary>
        /// Отправка данных на сервер.
        /// </summary>
        /// <param name="name">Название показателя.</param>
        /// <param name="value">Значение показателя.</param>
        /// <returns></returns>
        public bool Send(string name, object value);
    }
}
