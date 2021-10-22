using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DiscordLib
{
    public class ReadState : Snowflake<ReadState>
    {
        [JsonProperty("mention_count")]
        public int MentionCount { get; set; }

        [JsonProperty("last_message_id")]
        public ulong LastMessageId { get; set; }

        [JsonProperty("last_pin_timestamp")]
        public DateTimeOffset LastPinTimestamp { get; set; }
    }
}
