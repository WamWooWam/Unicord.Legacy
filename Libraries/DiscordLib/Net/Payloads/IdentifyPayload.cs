using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordLib.Net.Payloads
{
    internal class IdentifyPayload
    {
        public IdentifyPayload()
        {
            ClientProperties = new ClientProperties();
            Compress = false;
        }

        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("properties")]
        public ClientProperties ClientProperties { get; set; }
        [JsonProperty("compress")]
        public bool Compress { get; set; }
    }


    internal class ResumePayload
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        [JsonProperty("seq")]
        public long Sequence { get; set; }
    }


    internal sealed class ClientProperties
    {
        [JsonProperty("$os")]
        public string OperatingSystem { get { return "windows"; } }
        [JsonProperty("$browser")]
        public string Browser { get { return "Internet Explorer 9"; } }
        [JsonProperty("$device")]
        public string Device { get { return "Windows Phone 7"; } }
        [JsonProperty("$referrer")]
        public string Referrer { get { return string.Empty; } }
        [JsonProperty("$referring_domain")]
        public string ReferringDomain { get { return string.Empty; } }
    }
}
