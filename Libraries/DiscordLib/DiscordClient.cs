using DiscordLib.EventArgs;
using DiscordLib.Net;
using DiscordLib.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordLib
{
    public class DiscordClient
    {
        internal static DateTimeOffset DiscordEpoch = new DateTimeOffset(2015, 1, 1, 0, 0, 0, TimeSpan.Zero);

        private Uri _gatewayUrl;
        private ManualResetEvent _connectionLock;

        private ulong _currentUserId;
        
        internal ConcurrentDictionary<ulong, User> userCache;
        internal RingBuffer<Message> messageCache;

        public DiscordRestClient Rest { get; private set; }
        public DiscordSocketClient Socket { get; private set; }

        public User CurrentUser { get { User u; return _currentUserId != 0 && userCache.TryGetValue(_currentUserId, out u) ? u : null; } }
        public UserSettings CurrentUserSettings { get; internal set; }

        public ConcurrentDictionary<ulong, PrivateChannel> PrivateChannels { get; private set; }
        public ConcurrentDictionary<ulong, Guild> Guilds { get; private set; }

        public DiscordClient(string token)
        {
            _connectionLock = new ManualResetEvent(true);
            userCache = new ConcurrentDictionary<ulong, User>();
            messageCache = new RingBuffer<Message>(250);

            Rest = new DiscordRestClient(this, token);
            Socket = new DiscordSocketClient(this, token);
            Guilds = new ConcurrentDictionary<ulong, Guild>();
            PrivateChannels = new ConcurrentDictionary<ulong, PrivateChannel>();
        }

        public async Task ConnectAsync()
        {
            if (!_connectionLock.WaitOne(0)) throw new InvalidOperationException("Already connected!");

            await connectingEvent.InvokeAsync();

            _gatewayUrl = await Rest.GetGatewayUrlAsync().ConfigureAwait(false);

            var currentUser = await Rest.GetCurrentUserAsync().ConfigureAwait(false);
            _currentUserId = currentUser.Id;

            await Socket.ConnectAsync(_gatewayUrl).ConfigureAwait(false);
        }

        public Channel GetCachedChannel(ulong channelId)
        {
            PrivateChannel foundPrivateChannel;
            if (this.PrivateChannels.TryGetValue(channelId, out foundPrivateChannel))
                return foundPrivateChannel;

            Channel foundChannel;
            foreach (var guild in this.Guilds.Values)
                if (guild.Channels.TryGetValue(channelId, out foundChannel))
                    return foundChannel;

            return null;
        }

        public Guild GetCachedGuild(ulong? guildId)
        {
            Guild foundGuild;
            if (guildId.HasValue)
                if (this.Guilds.TryGetValue(guildId.Value, out foundGuild))
                    return foundGuild;

            return null;
        }


        #region Events
        internal AsyncEvent connectingEvent = new AsyncEvent(OnError, "CONNECTING");
        public event AsyncEventHandler Connecting
        {
            add { this.connectingEvent.Register(value); }
            remove { this.connectingEvent.Unregister(value); }
        }

        internal AsyncEvent readyEvent = new AsyncEvent(OnError, "READY");
        public event AsyncEventHandler Ready
        {
            add { this.readyEvent.Register(value); }
            remove { this.readyEvent.Unregister(value); }
        }

        internal AsyncEvent resumedEvent = new AsyncEvent(OnError, "RESUMED");
        public event AsyncEventHandler Resumed
        {
            add { this.resumedEvent.Register(value); }
            remove { this.resumedEvent.Unregister(value); }
        }


        internal AsyncEvent<GuildCreatedEventArgs> guildCreatedEvent
            = new AsyncEvent<GuildCreatedEventArgs>(OnError, "GUILD_CREATED");
        public event AsyncEventHandler<GuildCreatedEventArgs> GuildCreated
        {
            add { this.guildCreatedEvent.Register(value); }
            remove { this.guildCreatedEvent.Unregister(value); }
        }

        internal AsyncEvent<MessageCreateEventArgs> messageCreated
            = new AsyncEvent<MessageCreateEventArgs>(OnError, "MESSAGE_CREATED");
        public event AsyncEventHandler<MessageCreateEventArgs> MessageCreated
        {
            add { this.messageCreated.Register(value); }
            remove { this.messageCreated.Unregister(value); }
        }

        internal AsyncEvent<MessageDeleteEventArgs> messageDeleted
            = new AsyncEvent<MessageDeleteEventArgs>(OnError, "MESSAGE_DELETED");
        public event AsyncEventHandler<MessageDeleteEventArgs> MessageDeleted
        {
            add { this.messageDeleted.Register(value); }
            remove { this.messageDeleted.Unregister(value); }
        }
        #endregion

        private static void OnError(string arg1, Exception arg2)
        {

        }
    }
}
