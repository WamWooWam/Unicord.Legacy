using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordLib
{
    public class Role : Snowflake<Role>
    {
        /// <summary>
        /// Gets the name of this role.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// Gets the color of this role.
        /// </summary>
        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public int Color { get; set; }

        /// <summary>
        /// Gets whether this role is hoisted.
        /// </summary>
        [JsonProperty("hoist", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsHoisted { get; set; }

        /// <summary>
        /// Gets the position of this role in the role hierarchy.
        /// </summary>
        [JsonProperty("position", NullValueHandling = NullValueHandling.Ignore)]
        public int Position { get; set; }

        /// <summary>
        /// Gets the permissions set for this role.
        /// </summary>
        [JsonProperty("permissions", NullValueHandling = NullValueHandling.Ignore)]
        public Permissions Permissions { get; set; }

        /// <summary>
        /// Gets whether this role is managed by an integration.
        /// </summary>
        [JsonProperty("managed", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsManaged { get; set; }

        /// <summary>
        /// Gets whether this role is mentionable.
        /// </summary>
        [JsonProperty("mentionable", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsMentionable { get; set; }

    }
}
