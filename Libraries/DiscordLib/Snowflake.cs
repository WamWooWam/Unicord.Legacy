using System;
using System.Net;
using Newtonsoft.Json;

namespace DiscordLib
{
    public abstract class Snowflake<T> where T : Snowflake<T>
    {
        [JsonProperty("id")]
        public virtual ulong Id { get; set; }

        [JsonIgnore]
        public DateTimeOffset CreationTimestamp { get { return DiscordClient.DiscordEpoch.AddMilliseconds(Id >> 22); } }

        public virtual T Update(T other)
        {
            return (T)this;
        }

        public static implicit operator ulong(Snowflake<T> t)
        {
            return t.Id;
        }
    }
}
