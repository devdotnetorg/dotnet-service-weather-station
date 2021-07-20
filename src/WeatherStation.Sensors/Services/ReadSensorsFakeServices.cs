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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WeatherStation.Sensors.Settings;

namespace WeatherStation.Sensors.Services
{
    public class ReadSensorsFakeServices: IReadSensorsServices
    {
        private readonly ILogger<ReadSensorsFakeServices> _logger;
        private readonly AppSettings _appSettings;
        public event EventHandler ButtonChanged;
        public ReadSensorsFakeServices(ILogger<ReadSensorsFakeServices> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _appSettings = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AppSettings>();            
        }
        public IDictionary<string, object> ReadAll()
        {
            var dictionary = new Dictionary<string, object>();
            dictionary.Add("HomeTemperature", (double) 10.1);
            dictionary.Add("HomePressure", (double) 20.1);
            dictionary.Add("HomeMeters", (double) 30.1);
            dictionary.Add("HomeHumidity", (double) 40.1);
            //event
            OnButtonChanged(new ButtonEventArgs("PressButton1"));
            //
            return dictionary;           
        }
        protected virtual void OnButtonChanged(ButtonEventArgs e)
        {
            ButtonChanged?.Invoke(this, e);
        }
        public Task<Dictionary<string, object>> ReadAllAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                var dictionary = new Dictionary<string, object>();
                dictionary.Add("HomeTemperature", (double)10.1);
                dictionary.Add("HomePressure", (double)20.1);
                dictionary.Add("HomeMeters", (double)30.1);
                dictionary.Add("HomeHumidity", (double)40.1);
                //event
                OnButtonChanged(new ButtonEventArgs("PressButton1"));
                //
                return dictionary;
            });
        }

        public void Dispose()
        {
            _logger.LogInformation("Sensors Dispose");
        }

        public bool Init()
        {
            _logger.LogInformation("Sensors Init");
            return true;
        }
    }
}
