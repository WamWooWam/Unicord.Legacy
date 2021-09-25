using System;
using System.Net;
using Newtonsoft.Json;

namespace DiscordLib
{
    public class Error
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
