using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordLib.Net.Payloads
{
    internal class HelloPayload
    {
        [JsonProperty("heartbeat_interval")]
        public int HeartbeatInterval { get; internal set; }
    }
}
