using System;
using System.IO;
using System.Net;
using System.Threading;
using DynamicDiscordStatus.DTO;
using DynamicDiscordStatus.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DynamicDiscordStatus
{
    public class Program
    {
        private const string StatusUrl = "https://discordapp.com/api/v8/users/@me/settings";
        private static readonly dynamic _config = new ConfigHelper().getConfig();

        public static void Main(string[] args)
        {
            dynamic obj = File.ReadAllText(Directory.GetParent(Environment.CurrentDirectory).Parent?.Parent?.FullName + "/config/status.json");
            dynamic json = JArray.Parse(obj);
            while (true)
            {
                if ((bool)_config.UseSpecificTimer)
                {
                    foreach (var status in json)
                    {
                        if (!TimeSpan.TryParse((string) status.schedule, out var timer)) continue;
                        if (DateTime.Now.Hour != timer.Hours || DateTime.Now.Minute != timer.Minutes) continue;

                        var objectToSerialize = new StatusDTO
                        {
                            text = (string) status.text,
                            emoji_id = (string) status.emoji_id,
                            emoji_name = (string) status.emoji_name
                        };

                        var result = JsonConvert.SerializeObject(objectToSerialize);
                        result = "{\"custom_status\": " + result + "}";
                        ChangeStatus(result);
                        Thread.Sleep(60000);
                    }
                }
                else
                {
                    foreach (var status in json)
                    {
                        var objectToSerialize = new StatusDTO
                        {
                            text = (string) status.text,
                            emoji_id = (string) status.emoji_id,
                            emoji_name = (string) status.emoji_name
                        };

                        var result = JsonConvert.SerializeObject(objectToSerialize);
                        result = "{\"custom_status\": " + result + "}";
                        ChangeStatus(result);
                        Thread.Sleep((int)_config.TimeBetweenStatusChange);
                    }
                }
            }
        }

        private static void ChangeStatus(string json)
        {
            var httpRequest = (HttpWebRequest) WebRequest.Create(StatusUrl);
            
            httpRequest.Method = "PATCH";
            httpRequest.ContentType = "application/json";
            httpRequest.Accept = "application/json";
            httpRequest.Headers.Add("Authorization", (string) _config.token);

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
            }
            
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using var streamReader = new StreamReader(httpResponse.GetResponseStream() ?? throw new InvalidOperationException("Got a null reference exception (not normal ..)"));
            var result = streamReader.ReadToEnd();
            var information = JsonConvert.DeserializeObject<dynamic>(result);
            if (information.statusCode == 200 || information.statusCode == null) return;
            Console.WriteLine("Invalid Status Code: " + information.statusCode);
        }
    }
}
