using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.FilteringMode;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Device.I2c;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WeatherStation.Sensors.Settings;

namespace WeatherStation.Sensors.Services
{
    public class ReadSensorsServices : IReadSensorsServices
    {
        private readonly ILogger<ReadSensorsServices> _logger;
        private readonly AppSettings _appSettings;
        public event EventHandler ButtonChanged;
        //
        private static GpioController controller;
        private static I2cDevice i2cDevice;
        private static Bme280 bme280;
        private static PinValue ledPinValue = PinValue.Low;

        public ReadSensorsServices(ILogger<ReadSensorsServices> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _appSettings = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AppSettings>();
        }
        public void Init()
        {
            _logger.LogInformation("Инициализация датчиков");
            //GPIO
            var drvGpio = new LibGpiodDriver(_appSettings.Sensors.GPIOCHIP.Value);
            controller = new GpioController(PinNumberingScheme.Logical, drvGpio);
            //LED
            if (_appSettings.Sensors.pinLED_active_low.Value) ledPinValue = !ledPinValue;
            controller.OpenPin(_appSettings.Sensors.pinLED.Value, PinMode.Output);            
            controller.Write(_appSettings.Sensors.pinLED.Value, ledPinValue); //LED OFF
            //BUTTON
            controller.OpenPin(_appSettings.Sensors.pinBUTTON.Value, PinMode.Input);
            //При нажатие включается LED
            controller.RegisterCallbackForPinValueChangedEvent(_appSettings.Sensors.pinBUTTON.Value, PinEventTypes.Rising, (o, e) =>
            {
                ledPinValue = !ledPinValue;
                controller.Write(_appSettings.Sensors.pinLED.Value, ledPinValue);                
            });
            //При отпускание срабатывает событие и выключается LED
            controller.RegisterCallbackForPinValueChangedEvent(_appSettings.Sensors.pinBUTTON.Value, PinEventTypes.Falling, (o, e) =>
            {
                ledPinValue = !ledPinValue;
                controller.Write(_appSettings.Sensors.pinLED.Value, ledPinValue);
                //Срабатывание события
                OnButtonChanged(new ButtonEventArgs("PressButton1"));
            });
            //I2C
            //Bme280.SecondaryI2cAddress
            I2cConnectionSettings i2cSettings = new(_appSettings.Sensors.I2CBusId.Value, _appSettings.Sensors.BME280Address.Value);
            i2cDevice = I2cDevice.Create(i2cSettings);
            bme280 = new Bme280(i2cDevice)
            {
                // set higher sampling
                TemperatureSampling = Sampling.UltraHighResolution,
                PressureSampling = Sampling.UltraHighResolution,
                HumiditySampling = Sampling.UltraHighResolution,
                FilterMode = Bmx280FilteringMode.X2
            };
            //
            _logger.LogInformation("Инициализация датчиков успешно закончена");
        }
        protected virtual void OnButtonChanged(ButtonEventArgs e)
        {
            ButtonChanged?.Invoke(this, e);
        }

        public IDictionary<string, object> ReadAll()
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, object>> ReadAllAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                // Perform a synchronous measurement
                var readResult = bme280.ReadAsync().Result;
                var dictionary = new Dictionary<string, object>();
                //Read data
                dictionary.Add("HomeTemperature", readResult.Temperature?.DegreesCelsius);
                dictionary.Add("HomePressure", readResult.Pressure?.Hectopascals);
                dictionary.Add("HomeHumidity", readResult.Humidity?.Percent);
                return dictionary;
            });  
        }

        public void Dispose()
        {
            controller.Dispose();
            bme280.Dispose();
            i2cDevice.Dispose();                    
        }       
    }
}
