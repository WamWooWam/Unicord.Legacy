using System;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordLib
{
    public class Message : Snowflake<Message>
    {
        [JsonProperty("channel_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong ChannelId { get; set; }

        [JsonProperty("author", NullValueHandling = NullValueHandling.Ignore)]
        public User Author { get; set; }

        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public string Content { get; set; }

        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Timestamp { get; set; }

        [JsonProperty("edited_timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? EditedTimestamp { get; set; }

        [JsonProperty("tts", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsTTS { get; set; }

        [JsonProperty("mention_everyone", NullValueHandling = NullValueHandling.Ignore)]
        public bool MentionEveryone { get; set; }

        [JsonProperty("attachments", NullValueHandling = NullValueHandling.Ignore)]
        public Attachment[] Attachments { get; set; }


        ///// <summary>
        ///// Gets files attached to this message.
        ///// </summary>
        //[JsonIgnore]
        //public IReadOnlyList<DiscordAttachment> Attachments
        //    => this._attachments;

        ///// <summary>
        ///// Gets embeds attached to this message.
        ///// </summary>
        //[JsonIgnore]
        //public IReadOnlyList<DiscordEmbed> Embeds
        //    => this._embeds;

        ///// <summary>
        ///// Gets reactions used on this message.
        ///// </summary>
        //[JsonIgnore]
        //public IReadOnlyList<DiscordReaction> Reactions
        //    => this._reactions;

        ///// <summary>
        ///// Gets users or members mentioned by this message.
        ///// </summary>
        //[JsonIgnore]
        //public IReadOnlyList<DiscordComponent> Components
        //    => this._components;


        [JsonProperty("mentions", NullValueHandling = NullValueHandling.Ignore)]
        public List<User> MentionedUsers { get; set; }

        [JsonProperty("mention_roles", NullValueHandling = NullValueHandling.Ignore)]
        public List<ulong> MentionedRoles { get; set; }

        [JsonProperty("pinned", NullValueHandling = NullValueHandling.Ignore)]
        public bool Pinned { get; set; }

        [JsonProperty("webhook_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? WebhookId { get; set; }

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public MessageType? MessageType { get; set; }

        ///// <summary>
        ///// Gets the message activity in the Rich Presence embed.
        ///// </summary>
        //[JsonProperty("activity", NullValueHandling = NullValueHandling.Ignore)]
        //public DiscordMessageActivity Activity { get; internal set; }

        ///// <summary>
        ///// Gets the message application in the Rich Presence embed.
        ///// </summary>
        //[JsonProperty("application", NullValueHandling = NullValueHandling.Ignore)]
        //public DiscordMessageApplication Application { get; internal set; }

        //[JsonProperty("message_reference", NullValueHandling = NullValueHandling.Ignore)]
        //internal InternalDiscordMessageReference? _reference { get; set; }


        [JsonProperty("flags", NullValueHandling = NullValueHandling.Ignore)]
        public MessageFlags? Flags { get; set; }

        [JsonProperty("referenced_message", NullValueHandling = NullValueHandling.Ignore)]
        public Message ReferencedMessage { get; set; }
    }

    public enum MessageType : int
    {
        Default = 0,
        RecipientAdd = 1,
        RecipientRemove = 2,
        Call = 3,
        ChannelNameChange = 4,
        ChannelIconChange = 5,
        ChannelPinnedMessage = 6,
        GuildMemberJoin = 7,
        UserPremiumGuildSubscription = 8,
        TierOneUserPremiumGuildSubscription = 9,
        TierTwoUserPremiumGuildSubscription = 10,
        TierThreeUserPremiumGuildSubscription = 11,
        ChannelFollowAdd = 12,
        GuildDiscoveryDisqualified = 14,
        GuildDiscoveryRequalified = 15,
        Reply = 19
    }

    [Flags]
    public enum MessageFlags
    {
        Crossposted = 1 << 0,
        IsCrosspost = 1 << 1,
        SuppressedEmbeds = 1 << 2
    }

}
