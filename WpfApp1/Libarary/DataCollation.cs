using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApp1.Libarary
{
    class DataCollation
    {
        /// <summary>
        /// ListItem新值取代
        /// </summary>
        /// <param name="data">集合物件</param>
        /// <param name="key">索引鍵</param>
        /// <param name="value">要取代的新值</param>
        public static void ListItemReplace(ref IList<KeyValuePair<string, string>> data,string key, string value) 
        {
            var temp = data.FirstOrDefault(m => m.Key == key);
            var Index_temp = data.IndexOf(temp);
            if (Index_temp > 0)
            {
                data[Index_temp] = new KeyValuePair<string, string>(key, value);
            }
            //else {
            //    data.Add(new KeyValuePair<string, string>(key, value));
            //}
        }
    }
}
