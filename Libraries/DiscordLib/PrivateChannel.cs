using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordLib
{
    public class PrivateChannel : Channel
    {
        [JsonProperty("recipients", NullValueHandling = NullValueHandling.Ignore)]
        public List<User> Recipients { get; internal set; }

        [JsonProperty("icon", NullValueHandling = NullValueHandling.Ignore)]
        public string IconHash { get; internal set; }
    }
}
