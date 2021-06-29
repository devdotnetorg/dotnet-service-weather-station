using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherStation.Sensors.Settings
{
    public class SensorsSettings
    {
        public int ReadEvery { get; set; }
        public int? I2CBusId { get; set; }
        public byte? BME280Address { get; set; }
        public int? GPIOCHIP { get; set; }
        public int? pinDS18B20 { get; set; }        
        public int? pinLED { get; set; }
        public bool? pinLED_active_low { get; set; }
        public int? pinBUTTON { get; set; }
    }
}
