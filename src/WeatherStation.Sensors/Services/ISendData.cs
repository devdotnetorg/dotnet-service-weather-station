using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherStation.Sensors.Services
{
    public interface ISendData
    {
        public bool IsOpen { get; }
        public void Connect();
        public void Close();
        public bool Send(IDictionary<string, object> message);
        public bool Send(string name, object value);
    }
}
