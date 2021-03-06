// The MIT License(MIT)
//
// Copyright(c) 2021 Serdyukov Anton <anton@devdotnet.org>
// DevDotNet.Org
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
