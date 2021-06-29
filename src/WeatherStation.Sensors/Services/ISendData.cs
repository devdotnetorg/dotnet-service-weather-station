using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherStation.Sensors.Services
{
    public interface ISendData
    {
        public void Connect();
        public Task ConnectAsync();
        public void Close();
        public Task CloseAsync();
        public void Send(IDictionary<string, object> message);
        public void Send(string name, object value);
        public Task SendAsync();
    }
}
