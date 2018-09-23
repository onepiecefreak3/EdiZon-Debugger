﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using EdiZonDebugger.Models;

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

        public static bool IsUsingInstead(string content, out EdiZonConfig config)
        {
            config = new EdiZonConfig();

            try
            {
                config = JsonConvert.DeserializeObject<EdiZonConfig>(content);
                return !String.IsNullOrEmpty(config.useInstead);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsBeta(string content)
        {
            var config = JsonConvert.DeserializeObject<EdiZonConfig>(content);

            return config.beta;
        }

        public static bool TryParseConfig(string content, out EdiZonConfig config, out string message)
        {
            message = "";

            if (!TryParseJObject(content, out config, out message))
                return false;

            config.configs = new Dictionary<string, EdiZonConfig.VersionConfig>();
            foreach (var obj in JObject.Parse(content))
                if (obj.Key != "useInstead" && obj.Key != "beta")
                    config.configs.Add(obj.Key, obj.Value.ToObject<EdiZonConfig.VersionConfig>());

            return true;
        }
    }
}
