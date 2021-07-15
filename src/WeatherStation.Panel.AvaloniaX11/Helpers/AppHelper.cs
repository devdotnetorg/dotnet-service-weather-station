using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherStation.Panel.AvaloniaX11.Helpers
{
    public static  class AppHelper
    {
        public static string DictionaryToString(IDictionary<string, object> dictionary)
        {
            string dictionaryString = string.Empty;
            dictionary.ToList().ForEach(pair => dictionaryString += pair.Key + " : " + pair.Value + ", ");
            return dictionaryString.TrimEnd(',', ' ');
        }
    }    
}
