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
using TaskEx = System.Threading.Tasks.Task;

namespace DiscordLib
{
    public class DiscordClient
    {
        internal static DateTimeOffset DiscordEpoch = new DateTimeOffset(2015, 1, 1, 0, 0, 0, TimeSpan.Zero);

        private Uri _gatewayUrl;
        private ManualResetEvent _connectionLock;

        internal TaskCompletionSource<object> connectionSource;
        internal ConcurrentDictionary<ulong, User> userCache;
        internal RingBuffer<Message> messageCache;

        public DiscordRestClient Rest { get; private set; }
        public DiscordSocketClient Socket { get; private set; }

        private ulong _currentUserId;
        public User CurrentUser { get { User u; return _currentUserId != 0 && userCache.TryGetValue(_currentUserId, out u) ? u : null; } }
        public UserSettings UserSettings { get; internal set; }

        public ConcurrentDictionary<ulong, PrivateChannel> PrivateChannels { get; private set; }
        public ConcurrentDictionary<ulong, Guild> Guilds { get; private set; }

        public ConcurrentDictionary<ulong, ReadState> ReadStates { get; private set; }

        public DiscordClient(string token)
        {
            _connectionLock = new ManualResetEvent(true);
            connectionSource = new TaskCompletionSource<object>();

            userCache = new ConcurrentDictionary<ulong, User>();
            messageCache = new RingBuffer<Message>(250);

            Rest = new DiscordRestClient(this, token);
            Socket = new DiscordSocketClient(this, token);
            Guilds = new ConcurrentDictionary<ulong, Guild>();
            PrivateChannels = new ConcurrentDictionary<ulong, PrivateChannel>();
            ReadStates = new ConcurrentDictionary<ulong, ReadState>();
        }

        public async Task ConnectAsync()
        {
            try
            {
                if (!_connectionLock.WaitOne(0)) throw new InvalidOperationException("Already connected!");

                await Socket.connectingEvent.InvokeAsync();

                _gatewayUrl = await Rest.GetGatewayUrlAsync().ConfigureAwait(false);

                var currentUser = await Rest.GetCurrentUserAsync().ConfigureAwait(false);
                _currentUserId = currentUser.Id;

                Socket.Ready += () =>
                {
                    connectionSource.TrySetResult(null);
                    return TaskEx.Delay(0);
                };

                await Socket.ConnectAsync(_gatewayUrl).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                connectionSource.TrySetException(ex);
                throw;
            }
        }

        public Task EnsureConnectedAsync()
        {
            return connectionSource.Task;
        }

        public Task DisconnectAsync()
        {
            return Socket.DisconnectAsync();
        }

        public User GetCachedUser(ulong userId)
        {
            User user;
            if (this.userCache.TryGetValue(userId, out user))
                return user;
            return null;
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
            if (guildId.HasValue && this.Guilds.TryGetValue(guildId.Value, out foundGuild))
                return foundGuild;

            return null;
        }
    }
}
