using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Syntaq.Falcon.Utility
{
    public static class DynamicUtility
    {
        // TODO move t
        public static bool IsPropertyExist(dynamic obj, string name)
        {
            if (obj is ExpandoObject)
                return ((IDictionary<string, object>)obj).ContainsKey(name);

            if (obj is JObject)
            {
                JObject jobj = (JObject)obj;
                var result = jobj[name];
                return result == null ? false : true;
            }
                

            return obj.GetType().GetProperty(name) != null;
        }

        public static bool SetPropertyValue(dynamic obj, string name, string value)
        {
            if (obj is ExpandoObject)
            {
                ((IDictionary<string, object>)obj)[name] = value; ;
            }

            if (obj is JObject)
            {
                JObject jobj = (JObject)obj;
                jobj[name] = value;
            }


            return obj.GetType().GetProperty(name) != null;
        }
    }
}
