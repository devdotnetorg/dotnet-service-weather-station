using System;

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
