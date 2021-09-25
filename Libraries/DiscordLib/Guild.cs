using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordLib
{
    public class Guild : Snowflake<Guild>
    {
        public Guild()
        {
            Members = new ConcurrentDictionary<ulong, Member>();
            Channels = new ConcurrentDictionary<ulong, Channel>();
            Roles = new ConcurrentDictionary<ulong, Role>();
        }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("icon", NullValueHandling = NullValueHandling.Ignore)]
        public string IconHash { get; set; }

        [JsonProperty("splash", NullValueHandling = NullValueHandling.Ignore)]
        public string SplashHash { get; set; }

        [JsonProperty("discovery_splash", NullValueHandling = NullValueHandling.Ignore)]
        public string DiscoverySplashHash { get; set; }

        [JsonProperty("preferred_locale", NullValueHandling = NullValueHandling.Ignore)]
        public string PreferredLocale { get; set; }

        [JsonProperty("owner_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong OwnerId { get; set; }

        [JsonProperty("permissions", NullValueHandling = NullValueHandling.Ignore)]
        public Permissions? Permissions { get; set; }

        [JsonProperty("region", NullValueHandling = NullValueHandling.Ignore)]
        public string VoiceRegion { get; set; }

        [JsonProperty("afk_channel_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? AfkChannelId { get; set; }

        [JsonProperty("afk_timeout", NullValueHandling = NullValueHandling.Ignore)]
        public int AfkTimeout { get; set; }

        // [JsonProperty("verification_level", NullValueHandling = NullValueHandling.Ignore)]
        // public VerificationLevel VerificationLevel { get; set; }

        // [JsonProperty("default_message_notifications", NullValueHandling = NullValueHandling.Ignore)]
        // public DefaultMessageNotifications DefaultMessageNotifications { get; set; }

        // [JsonProperty("explicit_content_filter")]
        // public ExplicitContentFilter ExplicitContentFilter { get; set; }

        [JsonProperty("system_channel_id", NullValueHandling = NullValueHandling.Include)]
        public ulong? SystemChannelId { get; set; }

        // public SystemChannelFlags SystemChannelFlags { get; set; }

        [JsonProperty("widget_enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? WidgetEnabled { get; set; }

        [JsonProperty("widget_channel_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? WidgetChannelId { get; set; }

        [JsonProperty("rules_channel_id")]
        public ulong? RulesChannelId { get; set; }

        [JsonProperty("public_updates_channel_id")]
        public ulong? PublicUpdatesChannelId { get; set; }

        [JsonProperty("application_id")]
        public ulong? ApplicationId { get; set; }

        [JsonProperty("features", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Features { get; set; }

        // [JsonProperty("mfa_level", NullValueHandling = NullValueHandling.Ignore)]
        // public MfaLevel MfaLevel { get; set; }

        [JsonProperty("joined_at", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset JoinedAt { get; set; }

        [JsonProperty("large", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsLarge { get; set; }

        [JsonProperty("unavailable", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsUnavailable { get; set; }

        [JsonProperty("member_count", NullValueHandling = NullValueHandling.Ignore)]
        public int MemberCount { get; set; }

        [JsonProperty("max_members")]
        public int? MaxMembers { get; set; }

        [JsonProperty("max_presences")]
        public int? MaxPresences { get; set; }

        //[JsonProperty("voice_states", NullValueHandling = NullValueHandling.Ignore)]
        //public VoiceState[] VoiceStates { get; set; }

        [JsonProperty("members", NullValueHandling = NullValueHandling.Ignore)]
        internal List<Member> RawMembers { get; set; }

        [JsonProperty("channels", NullValueHandling = NullValueHandling.Ignore)]
        internal List<Channel> RawChannels { get; set; }

        [JsonProperty("roles", NullValueHandling = NullValueHandling.Ignore)]
        internal List<Role> RawRoles { get; set; }

        //[JsonProperty("emojis", NullValueHandling = NullValueHandling.Ignore)]
        //public List<Emoji> Emoji {get;set;}

        // [JsonProperty("invites", NullValueHandling = NullValueHandling.Ignore)]
        // public Invite[] Invites { get; set; }

        [JsonProperty("is_owner", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsOwner { get; set; }

        [JsonProperty("vanity_url_code")]
        public string VanityUrlCode { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("banner")]
        public string Banner { get; set; }

        // [JsonProperty("premium_tier")]
        // public PremiumTier PremiumTier { get; set; }

        [JsonProperty("premium_subscription_count", NullValueHandling = NullValueHandling.Ignore)]
        public int? PremiumSubscriptionCount { get; set; }

        [JsonIgnore]
        public ConcurrentDictionary<ulong, Member> Members { get; private set; }

        [JsonIgnore]
        public ConcurrentDictionary<ulong, Channel> Channels { get; private set; }

        [JsonIgnore]
        public ConcurrentDictionary<ulong, Role> Roles { get; private set; }

        public override Guild Update(Guild other)
        {
            Name = other.Name;
            AfkChannelId = other.AfkChannelId;
            AfkTimeout = other.AfkTimeout;
            //DefaultMessageNotifications = other.DefaultMessageNotifications;
            WidgetChannelId = other.WidgetChannelId;
            WidgetEnabled = other.WidgetEnabled;
            //ExplicitContentFilter = other.ExplicitContentFilter;
            Features = other.Features;
            IconHash = other.IconHash;
            Id = other.Id;
            IsLarge = other.IsLarge;
            //IsSynced = other.IsSynced;
            IsUnavailable = other.IsUnavailable;
            JoinedAt = other.JoinedAt;
            MemberCount = other.MemberCount;
            MaxMembers = other.MaxMembers;
            MaxPresences = other.MaxPresences;
            DiscoverySplashHash = other.DiscoverySplashHash;
            PreferredLocale = other.PreferredLocale;
            //MfaLevel = other.MfaLevel;
            OwnerId = other.OwnerId;
            SplashHash = other.SplashHash;
            SystemChannelId = other.SystemChannelId;
            //SystemChannelFlags = other.SystemChannelFlags;
            //VerificationLevel = other.VerificationLevel;
            RulesChannelId = other.RulesChannelId;
            PublicUpdatesChannelId = other.PublicUpdatesChannelId;
            VoiceRegion = other.VoiceRegion;

            if (other.RawMembers != null)
            {
                foreach (var member in other.RawMembers)
                {
                    member.GuildId = this.Id;
                    Members.AddOrUpdate(member.Id, member, (id, mem) => mem.Update(member));
                }
            }

            if (other.RawChannels != null)
            {
                foreach (var channel in other.RawChannels)
                {
                    channel.GuildId = this.Id;
                    Channels.AddOrUpdate(channel.Id, channel, (id, chan) => chan.Update(channel));
                }
            }

            if (other.RawRoles != null)
            {
                foreach (var role in other.RawRoles)
                {
                    Roles.AddOrUpdate(role.Id, role, (id, r) => r.Update(role));
                }
            }

            RawMembers = Members.Values.ToList();
            RawChannels = Channels.Values.ToList();
            RawRoles = Roles.Values.ToList();

            return this;
        }
    }
}
