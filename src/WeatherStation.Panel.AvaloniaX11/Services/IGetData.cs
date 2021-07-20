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
