using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordLib.Net.Payloads
{
    internal class MessageDeletePayload
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
    }
}
