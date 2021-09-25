using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordLib
{
    public class Channel : Snowflake<Channel>
    {
        /// <summary>
        /// Gets ID of the guild to which this channel belongs.
        /// </summary>
        [JsonProperty("guild_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong GuildId { get; set; }

        /// <summary>
        /// Gets ID of the category that contains this channel.
        /// </summary>
        [JsonProperty("parent_id", NullValueHandling = NullValueHandling.Include)]
        public ulong? ParentId { get; set; } // lets fucking go

        /// <summary>
        /// Gets the name of this channel.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// Gets the type of this channel.
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public ChannelType Type { get; set; }

        /// <summary>
        /// Gets the position of this channel.
        /// </summary>
        [JsonProperty("position", NullValueHandling = NullValueHandling.Ignore)]
        public int Position { get; set; }

        /// <summary>
        /// Gets a collection of permission overwrites for this channel.
        /// </summary>
        [JsonProperty("permission_overwrites", NullValueHandling = NullValueHandling.Ignore)]
        public PermissionOverwrite[] PermissionOverwrites { get; set; }

        /// <summary>
        /// Gets the channel's topic. This is applicable to text channels only.
        /// </summary>
        [JsonProperty("topic", NullValueHandling = NullValueHandling.Ignore)]
        public virtual string Topic { get; set; }

        /// <summary>
        /// Gets the ID of the last message sent in this channel. This is applicable to text channels only.
        /// </summary>
        [JsonProperty("last_message_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong LastMessageId { get; set; }

        /// <summary>
        /// Gets this channel's bitrate. This is applicable to voice channels only.
        /// </summary>
        [JsonProperty("bitrate", NullValueHandling = NullValueHandling.Ignore)]
        public int Bitrate { get; set; }

        /// <summary>
        /// Gets this channel's user limit. This is applicable to voice channels only.
        /// </summary>
        [JsonProperty("user_limit", NullValueHandling = NullValueHandling.Ignore)]
        public int UserLimit { get; set; }

        /// <summary>
        /// <para>Gets the slow mode delay configured for this channel.</para>
        /// <para>All bots, as well as users with <see cref="Permissions.ManageChannels"/> or <see cref="Permissions.ManageMessages"/> permissions in the channel are exempt from slow mode.</para>
        /// </summary>
        [JsonProperty("rate_limit_per_user")]
        public int? PerUserRateLimit { get; set; }

        /// <summary>
        /// Gets whether this channel is an NSFW channel.
        /// </summary>
        [JsonProperty("nsfw")]
        public bool IsNSFW { get; set; }
    }

    /// <summary>
    /// Represents a channel's type.
    /// </summary>
    public enum ChannelType : int
    {
        /// <summary>
        /// Indicates that this is a text channel.
        /// </summary>
        Text = 0,

        /// <summary>
        /// Indicates that this is a private channel.
        /// </summary>
        Private = 1,

        /// <summary>
        /// Indicates that this is a voice channel.
        /// </summary>
        Voice = 2,

        /// <summary>
        /// Indicates that this is a group direct message channel.
        /// </summary>
        Group = 3,

        /// <summary>
        /// Indicates that this is a channel category.
        /// </summary>
        Category = 4,

        /// <summary>
        /// Indicates that this is a news channel.
        /// </summary>
        News = 5,

        /// <summary>
        /// Indicates that this is a store channel.
        /// </summary>
        Store = 6,

        /// <summary>
        /// Indicates that this is a stage channel.
        /// </summary>
        Stage = 13,

        /// <summary>
        /// Indicates unknown channel type.
        /// </summary>
        Unknown = int.MaxValue
    }
}
