using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordLib
{
    public class Attachment : Snowflake<Attachment>
    {
        [JsonProperty("filename", NullValueHandling = NullValueHandling.Ignore)]
        public string FileName { get; set; }

        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public int FileSize { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }

        [JsonProperty("proxy_url", NullValueHandling = NullValueHandling.Ignore)]
        public string ProxyUrl { get; set; }

        [JsonProperty("height", NullValueHandling = NullValueHandling.Ignore)]
        public int Height { get; set; }

        [JsonProperty("width", NullValueHandling = NullValueHandling.Ignore)]
        public int Width { get; set; }

        [JsonProperty("content_type", NullValueHandling = NullValueHandling.Ignore)]
        public string ContentType { get; set; }

    }
}
