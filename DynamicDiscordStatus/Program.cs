using System;
using System.IO;
using System.Net;
using DynamicDiscordStatus.DTO;
using Newtonsoft.Json;

namespace DynamicDiscordStatus
{
    public class Program
    {
        private const string STATUS_URL = "https://discordapp.com/api/v8/users/@me/settings";
        /*
         * To get the token press ctrl+shift+I go to console and put
         * var req=webpackJsonp.push([[],{extra_id:(e,r,t)=>e.exports=t},[["extra_id"]]]);for(let e in req.c)if(req.c.hasOwnProperty(e)){let r=req.c[e].exports;if(r&&r.__esModule&&r.default)for(let e in r.default)"getToken"===e&&console.log(r.default.getToken())}
         */
        private const string TOKEN = "";
        
        public static void Main(string[] args)
        {
            var objectToSerialize = new StatusDTO
            {
                text = "ahahahaha",
                emoji_id = null,
                emoji_name = null
            };

            var result = JsonConvert.SerializeObject(objectToSerialize);
            result = "{\"custom_status\": "+ result +"}";
            Console.WriteLine(result);
            ChangeStatus(result);
        }

        private static void ChangeStatus(string json)
        {
            var httpRequest = (HttpWebRequest) WebRequest.Create(STATUS_URL);
            
            httpRequest.Method = "PATCH";
            httpRequest.ContentType = "application/json";
            httpRequest.Accept = "application/json";
            httpRequest.Headers.Add("Authorization", TOKEN);

            var data = json;

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }
            
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using var streamReader = new StreamReader(httpResponse.GetResponseStream() ?? throw new InvalidOperationException("Got a null reference exception (not normal ..)"));
            var result = streamReader.ReadToEnd();
            var information = JsonConvert.DeserializeObject<dynamic>(result);
            if (information.statusCode == 200) return;
            Console.WriteLine("Invalid Status Code: " + information.statusCode);
        }
    }
}