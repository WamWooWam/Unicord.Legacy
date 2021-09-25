using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordLib.Net.Payloads
{
    /// <summary>
    /// Represents data for websocket ready event payload.
    /// </summary>
    internal class ReadyPayload
    {
        [JsonProperty("v")]
        public int GatewayVersion { get; internal set; }
        [JsonProperty("user")]
        public User CurrentUser { get; internal set; }
        [JsonProperty("private_channels")]
        public IList<PrivateChannel> PrivateChannels { get; internal set; }
        [JsonProperty("guilds")]
        public IList<Guild> Guilds { get; internal set; }
        [JsonProperty("relationships", NullValueHandling = NullValueHandling.Ignore)]
        public IList<Relationship> Relationships { get; internal set; }
        [JsonProperty("presences", NullValueHandling = NullValueHandling.Ignore)]
        public IList<Presence> Presences { get; internal set; }

        // [JsonProperty("read_state", NullValueHandling = NullValueHandling.Ignore)]
        // public IList<DiscordReadState> ReadStates { get; private set; }

        /// <summary>
        /// Gets the user settings for this shard
        /// </summary>
        [JsonProperty("user_settings", NullValueHandling = NullValueHandling.Ignore)]
        public UserSettings UserSettings { get; set; }


        // [JsonProperty("user_guild_settings", NullValueHandling = NullValueHandling.Ignore)]
        // public IList<DiscordUserGuildSettings> UserGuildSettings { get; set; }

        /// <summary>
        /// Gets the current session's ID.
        /// </summary>
        [JsonProperty("session_id")]
        public string SessionId { get; internal set; }

        /// <summary>
        /// Gets debug data sent by Discord. This contains a list of servers to which the client is connected.
        /// </summary>
        // [JsonProperty("_trace")]
        // public IReadOnlyList<string> Trace { get; private set; }
    }
}
