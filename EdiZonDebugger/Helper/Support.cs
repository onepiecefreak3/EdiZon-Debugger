using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace EdiZonDebugger
{
    public static class Support
    {
        public static bool TryParseJObject(string parse) => TryParseJObject(parse, out var obj, out var error);

        public static bool TryParseJObject(string parse, out JObject obj) => TryParseJObject(parse, out obj, out var error);

        public static bool TryParseJObject(string parse, out JObject obj, out string message)
        {
            message = "";
            obj = new JObject();

            try
            {
                obj = JObject.Parse(parse);
            }
            catch (Exception e)
            {
                message = e.Message;
                return false;
            }

            return true;
        }

        public static bool TryParseJObject<T>(string parse, out T obj) => TryParseJObject(parse, out obj, out var error);

        public static bool TryParseJObject<T>(string parse, out T obj, out string message)
        {
            message = "";
            obj = (T)Activator.CreateInstance(typeof(T));

            try
            {
                obj = JsonConvert.DeserializeObject<T>(parse);
            }
            catch (Exception e)
            {
                message = e.Message;
                return false;
            }

            return true;
        }
    }
}
