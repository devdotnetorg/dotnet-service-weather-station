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
using Iot.Device.OneWire;

namespace WeatherStation.Sensors.Services
{
    public class ReadSensorsServices : IReadSensorsServices
    {
        private readonly ILogger<ReadSensorsServices> _logger;
        private readonly AppSettings _appSettings;
        public event EventHandler ButtonChanged;
        private static bool isLEDAvailable;
        //
        private static GpioController controller;
        private static I2cDevice i2cDevice;
        private static Bme280 bme280;
        private static PinValue ledPinValue;
        private static OneWireThermometerDevice devOneWire;

        public ReadSensorsServices(ILogger<ReadSensorsServices> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _appSettings = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AppSettings>();
            ledPinValue = PinValue.Low;
            isLEDAvailable = false;
        }
        public bool Init()
        {
            _logger.LogInformation("Инициализация датчиков");
            //I2C Bme280.SecondaryI2cAddress
            try
            {
                if (_appSettings.Sensors.I2CBusId != null && _appSettings.Sensors.BME280Address != null)
                {
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
                    //Test Read
                    bme280.ReadAsync().Wait();
                    _logger.LogInformation("Температурный датчик Bme280 успешно инициализирован");
                } else _logger.LogInformation("Не заданы настройки для датчика Bme280");
            }
            catch (Exception ex)
            {                
                _logger.LogError($"Не удалось подключиться к датчику Bme280. Ошибка: {ex.Message}", ex);                
            }
            //DS18B20
            if(bme280 is null){
                try
                {
                    devOneWire = OneWireThermometerDevice.EnumerateDevices().FirstOrDefault();
                    devOneWire.ReadTemperatureAsync().Wait();
                    _logger.LogInformation($"Температурный датчик OneWire:{devOneWire.Family} успешно инициализирован");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ни один температурный датчик не найден!. Ошибка: {ex.Message}", ex);
                    return false;
                }
            }
            if(_appSettings.Sensors.GPIOCHIP!=null && _appSettings.Sensors.pinBUTTON!=null)
            {
                try
                {
                    //GPIO
                    var drvGpio = new LibGpiodDriver(_appSettings.Sensors.GPIOCHIP.Value);
                    controller = new GpioController(PinNumberingScheme.Logical, drvGpio);
                    //LED
                    if(_appSettings.Sensors.pinLED!=null&&
                            !controller.IsPinOpen(_appSettings.Sensors.pinLED.Value)&&
                            controller.IsPinModeSupported(_appSettings.Sensors.pinLED.Value, PinMode.Output))
                            {
                                controller.OpenPin(_appSettings.Sensors.pinLED.Value, PinMode.Output);
                                controller.Write(_appSettings.Sensors.pinLED.Value, (ledPinValue ==
                                (_appSettings.Sensors.pinLED_active_low == false))); //LED OFF                                
                                isLEDAvailable = true;
                                _logger.LogInformation("LED успешно инициализирован");
                    }
                    //BUTTON
                    if (!controller.IsPinOpen(_appSettings.Sensors.pinBUTTON.Value) &&
                            controller.IsPinModeSupported(_appSettings.Sensors.pinBUTTON.Value, PinMode.Input))
                    {
                        controller.OpenPin(_appSettings.Sensors.pinBUTTON.Value, PinMode.Input);
                        if(isLEDAvailable)
                        {
                            //При нажатие включается LED
                            controller.RegisterCallbackForPinValueChangedEvent(_appSettings.Sensors.pinBUTTON.Value, PinEventTypes.Rising, (o, e) =>
                            {
                                ledPinValue = !ledPinValue;
                                controller.Write(_appSettings.Sensors.pinLED.Value, (ledPinValue ==
                            (_appSettings.Sensors.pinLED_active_low == false)));
                            });
                        }
                        //При отпускание срабатывает событие и выключается LED
                        controller.RegisterCallbackForPinValueChangedEvent(_appSettings.Sensors.pinBUTTON.Value, PinEventTypes.Falling, (o, e) =>
                        {
                            if (isLEDAvailable)
                            {
                                ledPinValue = !ledPinValue;
                                controller.Write(_appSettings.Sensors.pinLED.Value, (ledPinValue ==
                                (_appSettings.Sensors.pinLED_active_low == false)));
                            }
                            //Срабатывание события
                            OnButtonChanged(new ButtonEventArgs("PressButton1"));
                        });
                        _logger.LogInformation("Кнопка успешно инициализирована");
                    }   
                }
                catch (Exception ex)
                {
                    _logger.LogError($"При инициализации кнопки и/или светодиода возникла ошибка: {ex.Message}", ex);                    
                }
            }
            //
            _logger.LogInformation("Инициализация датчиков закончена");
            return true;
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
                var dictionary = new Dictionary<string, object>();
                if (bme280!=null)
                {
                    //Read data
                    var readResult = bme280.ReadAsync().Result;
                    dictionary.Add("HomeTemperature", Math.Round((double)readResult.Temperature?.DegreesCelsius, 2, MidpointRounding.AwayFromZero));
                    dictionary.Add("HomePressure", Math.Round((double)(readResult.Pressure?.Hectopascals * 100), 0, MidpointRounding.AwayFromZero));
                    dictionary.Add("HomeHumidity", Math.Round((double)readResult.Humidity?.Percent, 0, MidpointRounding.AwayFromZero));
                }else
                {
                    //var temp = Math.Round((Decimal)devOneWire.ReadTemperatureAsync().Result.DegreesCelsius, 2, MidpointRounding.AwayFromZero);
                    dictionary.Add("HomeTemperature", Math.Round((Decimal)devOneWire
                        .ReadTemperatureAsync().Result.DegreesCelsius,2, MidpointRounding.AwayFromZero));
                }
                return dictionary;
            });  
        }

        public void Dispose()
        {
            if (bme280 != null) bme280.Dispose();
            if (i2cDevice != null) i2cDevice.Dispose();
            if (controller!=null) controller.Dispose();
            if (devOneWire != null) devOneWire = null;            
        }        
    }
}
