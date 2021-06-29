using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherStation.Sensors.Services
{
    public class ButtonEventArgs : EventArgs
    {
        public string _Name { get;}
        public ButtonEventArgs(string Name)
        {
            _Name = Name;
        }
    }
}
