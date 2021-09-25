using System;
using System.Net;
using Newtonsoft.Json;

namespace DiscordLib
{
    public class User : Snowflake<User>
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("discriminator")]
        public string Discriminator { get; set; }

        [JsonProperty("avatar")]
        public string AvatarHash { get; set; }

        public override User Update(User other)
        {
            Username = other.Username;
            Discriminator = other.Discriminator;
            AvatarHash = other.AvatarHash;

            return this;
        }
    }
}
