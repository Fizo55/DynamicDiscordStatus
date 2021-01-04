using System;
using System.IO;
using Newtonsoft.Json;

namespace DynamicDiscordStatus.Helpers
{
    public class ConfigHelper
    {
        public dynamic getConfig()
        {
            var json = File.ReadAllText(Directory.GetParent(Environment.CurrentDirectory).Parent?.Parent?.FullName + "/config/config.json");
            var result = JsonConvert.DeserializeObject<dynamic>(json);
            return result;
        }
    }
}