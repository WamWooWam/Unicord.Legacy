using System;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordLib
{
    public class Member : Snowflake<Member>
    {
        public override ulong Id
        {
            get
            {
                return User.Id;
            }
            set
            {
                User.Id = value;
            }
        }

        [JsonProperty("user", NullValueHandling = NullValueHandling.Ignore)]
        public User User { get; set; }

        [JsonIgnore]
        public ulong GuildId { get; set; }

        [JsonProperty("nick", NullValueHandling = NullValueHandling.Ignore)]
        public string Nickname { get; set; }

        [JsonProperty("roles", NullValueHandling = NullValueHandling.Ignore)]
        public List<ulong> Roles { get; set; }

        [JsonProperty("joined_at", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime JoinedAt { get; set; }

        [JsonProperty("deaf", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsDeafened { get; set; }

        [JsonProperty("mute", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsMuted { get; set; }

        [JsonProperty("hoisted_role", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? HoistedRole { get; set; }

        public override Member Update(Member member)
        {
            return this;
        }
    }
}
