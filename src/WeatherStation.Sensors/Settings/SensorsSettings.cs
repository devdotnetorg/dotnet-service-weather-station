using System;

namespace WeatherStation.Sensors.Settings
{
    /// <summary>
    /// Настройки для опроса датчиков
    /// </summary>
    public class SensorsSettings
    {
        /// <summary>
        /// Периодичность чтения значений датчиков в секундах.
        /// </summary>
        public int ReadEvery { get; set; }
        /// <summary>
        /// Номер шины I2C.
        /// </summary>
        public int? I2CBusId { get; set; }
        /// <summary>
        /// Адрес датчика BME280.
        /// </summary>
        public byte? BME280Address { get; set; }
        /// <summary>
        /// Номер GPIOCHIP на котором располагнаются контакты
        /// кнопки и светодиода.
        /// </summary>
        public int? GPIOCHIP { get; set; }     
        /// <summary>
        /// Номер контакта светодиода.
        /// </summary>
        public int? pinLED { get; set; }
        /// <summary>
        /// Инверсия значений для светодиода. По умолчанию false.
        /// Если true, то на логическое значение "1" будет задано напряжение 0V.
        /// </summary>
        public bool pinLED_active_low { get; set; }
        /// <summary>
        /// Номер контакта кнопки.
        /// </summary>
        public int? pinBUTTON { get; set; }
    }
}
