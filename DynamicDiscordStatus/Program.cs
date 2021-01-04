using System;
using System.IO;
using System.Net;
using System.Threading;
using DynamicDiscordStatus.DTO;
using DynamicDiscordStatus.Helpers;
using Newtonsoft.Json;

namespace DynamicDiscordStatus
{
    public class Program
    {
        private const string StatusUrl = "https://discordapp.com/api/v8/users/@me/settings";
        private static readonly dynamic _config = new ConfigHelper().getConfig();

        public static void Main(string[] args)
        {
            while (true)
            {
                if ((bool)_config.UseSpecificTimer)
                {
                    // TODO : json
                    // if (DateTime.Now.Hour == 6 && DateTime.Now.Minute == 20)
                    Thread.Sleep(60000);
                }
                else
                {
                    var objectToSerialize = new StatusDTO
                    {
                        text = "test",
                        emoji_id = null,
                        emoji_name = null
                    };

                    var result = JsonConvert.SerializeObject(objectToSerialize);
                    result = "{\"custom_status\": " + result + "}";
                    Console.WriteLine(result);
                    //ChangeStatus(result);
                    Thread.Sleep((int)_config.TimeBetweenStatusChange);
                }
            }
        }

        private static void ChangeStatus(string json)
        {
            var httpRequest = (HttpWebRequest) WebRequest.Create(StatusUrl);
            
            httpRequest.Method = "PATCH";
            httpRequest.ContentType = "application/json";
            httpRequest.Accept = "application/json";
            httpRequest.Headers.Add("Authorization", _config.token);

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