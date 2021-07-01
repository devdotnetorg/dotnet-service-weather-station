using System.Collections.Generic;
using System.Linq;

namespace WeatherStation.Sensors.Helpers
{
    public static class AppHelper
    {
        public static string DictionaryToString(IDictionary<string, object> dictionary)
        {
            string dictionaryString = string.Empty;
            dictionary.ToList().ForEach(pair =>dictionaryString += pair.Key + " : " + pair.Value + ", ");
            return dictionaryString.TrimEnd(',', ' ');
        }
    }
}
