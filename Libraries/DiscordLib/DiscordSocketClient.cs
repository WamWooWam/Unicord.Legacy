using DiscordLib.EventArgs;
using DiscordLib.Net;
using DiscordLib.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DiscordLib.Net.Payloads;
using WebSocket4Net;
using DiscordLib.Net.WebSocket;
using System.IO;

namespace DiscordLib
{
    public class DiscordSocketClient
    {
        private string _token;
        private DiscordClient _client;

        private Uri _gatewayUri;
        private string _sessionId;

        private int _heartbeatInterval;
        private int _skippedHeartbeats;
        private long? _seq;
        private DateTimeOffset _lastHeartbeat;
        private bool _disconnecting;

        private Task _heartbeatTask;
        private CancellationTokenSource _heartbeatCancellation;

        private int _ping;

        private Task _queueTask;
        private ConcurrentQueue<Func<Task>> _taskQueue;

        private WebSocket _webSocket;
        private PayloadDecompressor _decompressor;
        private TaskCompletionSource<object> _tcs;

        private AverageCounter _decompressionSavings;
        private AverageCounter _decompressionTimings;
        private AverageCounter _dispatchTimings;

        public bool IsConnected { get; set; }

        internal DiscordSocketClient(DiscordClient client, string token)
        {
            _client = client;
            _token = token;
            _taskQueue = new ConcurrentQueue<Func<Task>>();

            _decompressor = new PayloadDecompressor(GatewayCompressionLevel.Stream);

            _decompressionSavings = new AverageCounter(250);
            _decompressionTimings = new AverageCounter(250);
            _dispatchTimings = new AverageCounter(250);
        }

        internal async Task ConnectAsync(Uri gatewayUri)
        {
            _gatewayUri = gatewayUri;
            _tcs = new TaskCompletionSource<object>();

            var uri = new UriBuilder(gatewayUri);
            uri.Query = "v=9&encoding=json&compress=zlib-stream";

            _webSocket = new WebSocket(uri.ToString());
            _webSocket.Opened += OnSocketOpened;
            _webSocket.DataReceived += OnSocketData;
            _webSocket.Error += OnSocketError;
            _webSocket.Closed += OnSocketClosed;
            _webSocket.Open();

            try { await _tcs.Task; }
            catch (TaskCanceledException) { }
        }

        internal Task DisconnectAsync()
        {
            IsConnected = false;
            _disconnecting = true;
            _tcs.TrySetCanceled();

            _webSocket.Close(1000, null);
            _webSocket.Dispose();

            return Task.Delay(0);
        }

        private void OnSocketOpened(object sender, System.EventArgs e)
        {
            _tcs.TrySetResult(null);
        }

        private void OnSocketData(object sender, DataReceivedEventArgs e)
        {
            QueueTask(async () => await DecompressSocketMessageAsync(e.Data));
        }

        private async void OnSocketClosed(object sender, System.EventArgs e)
        {
            IsConnected = false;

            Debug.WriteLine("Socket closed.");

            if (_disconnecting)
            {
                _disconnecting = false;
                return;
            }

            _tcs.TrySetCanceled();
            _heartbeatCancellation.Cancel();
            if (_gatewayUri != null)
                await ConnectAsync(_gatewayUri);
        }

        private void OnSocketError(object sender, ErrorEventArgs e)
        {
            Debug.WriteLine("Socket errored.");
            Debug.WriteLine(e.Exception);
        }

        private async Task DecompressSocketMessageAsync(byte[] data)
        {
            var watch = Stopwatch.StartNew();
            using (var stream = new MemoryStream())
            {
                if (!_decompressor.TryDecompress(data, stream))
                    return;

                _decompressionSavings.SubmitSample((double)(stream.Length - data.Length) / stream.Length);
                _decompressionTimings.SubmitSample(watch.Elapsed.TotalMilliseconds);
                watch.Restart();

                stream.Position = 0;
                var payload = await RestClient.DeserializeFromStreamAsync<GatewayPayload>(stream);
                switch (payload.OpCode)
                {
                    case GatewayOpCode.Dispatch:
                        await HandleDispatchAsync(payload);
                        break;
                    case GatewayOpCode.Heartbeat:
                        break;
                    case GatewayOpCode.Reconnect:
                        break;
                    case GatewayOpCode.InvalidSession:
                        await HandleInvalidSessionAsync();
                        break;
                    case GatewayOpCode.Hello:
                        await HandleHelloAsync(payload.Data.ToObject<HelloPayload>());
                        break;
                    case GatewayOpCode.HeartbeatAck:
                        await HandleHeartbeatAckAsync();
                        break;
                    default:
                        break;
                }
                
                _dispatchTimings.SubmitSample(watch.Elapsed.TotalMilliseconds);
            }
        }
        
        private async Task HandleHelloAsync(HelloPayload helloPayload)
        {
            if (_heartbeatCancellation != null && !_heartbeatCancellation.IsCancellationRequested)
                _heartbeatCancellation.Cancel();

            _heartbeatCancellation = new CancellationTokenSource();
            _heartbeatInterval = helloPayload.HeartbeatInterval;
            _heartbeatTask = Task.Run(new Func<Task>(HeartbeatAsync));

            if (_sessionId == null)
            {
                await IdentifyAsync();
            }
            else
            {
                await ResumeAsync();
            }
        }

        private async Task IdentifyAsync()
        {
            var identify = new IdentifyPayload() { Token = _token };
            await SendAsync(GatewayOpCode.Identify, identify);
        }

        private async Task ResumeAsync()
        {
            var resume = new ResumePayload() { Token = _token, SessionId = _sessionId, Sequence = _seq.GetValueOrDefault() };
            await SendAsync(GatewayOpCode.Resume, resume);
        }

        private async Task HandleInvalidSessionAsync()
        {
            _sessionId = null;

            await Task.Delay(new Random().Next(1000, 5000)); // yes this is what you're meant to do lol
            await IdentifyAsync();
        }

        private async Task HeartbeatAsync()
        {
            var token = _heartbeatCancellation.Token;
            while (!token.IsCancellationRequested)
            {
                await SendHeartbeatAsync();
                await Task.Delay(_heartbeatInterval);

                Debug.WriteLine("Average compression savings: {0:N2}%, {1:N2}ms",
                    _decompressionSavings.GetAverage() * 100,
                    _decompressionTimings.GetAverage());

                int count;
                double min, max, avg;
                _dispatchTimings.GetAverage(out count, out min, out max, out avg);

                Debug.WriteLine("Average dispatch time: {0:N2}ms (min: {1:N2}ms, max: {2:N2}ms, samples: {3})", avg, min, max, count);
            }        
        }

        private async Task HandleDispatchAsync(GatewayPayload payload)
        {
            _seq = payload.Sequence;

            Debug.WriteLine("Got event: {0}", payload.EventName);

            switch (payload.EventName.ToLowerInvariant())
            {
                case "ready":
                    await HandleReadyAsync(new GatewayPayload<ReadyPayload>(payload));
                    break;
                case "resumed":
                    await HandleResumedAsync(new GatewayPayload<object>(payload));
                    break;
                case "guild_create":
                    await HandleGuildCreateAsync(new GatewayPayload<Guild>(payload));
                    break;
                case "message_create":
                    await HandleMessageCreateAsync(new GatewayPayload<Message>(payload));
                    break;
                case "message_delete":
                    await HandleMessageDeleteAsync(new GatewayPayload<MessageDeletePayload>(payload));
                    break;
                default:
                    break;
            }
        }

        private async Task HandleReadyAsync(GatewayPayload<ReadyPayload> payload)
        {
            IsConnected = true;

            var readyPayload = payload.Data;
            _sessionId = readyPayload.SessionId;
            _client.UserSettings = readyPayload.UserSettings;

            foreach (var guild in readyPayload.Guilds)
            {
                guild.Update(guild);
                _client.Guilds.AddOrUpdate(guild.Id, guild, (id, g) => g.Update(guild));
            }

            foreach (var privateChannel in readyPayload.PrivateChannels)
            {
                for (int i = 0; i < privateChannel.Recipients.Count; i++)
                {
                    var recipient = privateChannel.Recipients[i];
                    privateChannel.Recipients[i] = _client.userCache.AddOrUpdate(recipient.Id, recipient, (id, u) => u.Update(recipient));
                }

                privateChannel.Update(privateChannel);
                _client.PrivateChannels.AddOrUpdate(privateChannel.Id, privateChannel, (id, dm) => (PrivateChannel)dm.Update(privateChannel));
            }

            await readyEvent.InvokeAsync();
        }

        private async Task HandleResumedAsync(GatewayPayload<object> payload)
        {
            IsConnected = true;

            Debug.WriteLine("Session resumed!");
            await resumedEvent.InvokeAsync();
        }

        private async Task HandleGuildCreateAsync(GatewayPayload<Guild> payload)
        {
            var newGuild = payload.Data;
            var result = _client.Guilds.AddOrUpdate(newGuild.Id, newGuild, (id, guild) => guild.Update(newGuild));

            await guildCreatedEvent.InvokeAsync(new GuildCreatedEventArgs(result));
        }

        private async Task HandleMessageCreateAsync(GatewayPayload<Message> payload)
        {
            var message = payload.Data;
            if (message.ChannelId != 0)
                _client.messageCache.Add(message);

            var ea = new MessageCreateEventArgs() { Message = message, };
            await messageCreated.InvokeAsync(ea);
        }

        private async Task HandleMessageDeleteAsync(GatewayPayload<MessageDeletePayload> payload)
        {
            var messageId = payload.Data.Id;
            var channelId = payload.Data.ChannelId;
            var guildId = payload.Data.GuildId;

            var channel = _client.GetCachedChannel(channelId);
            var guild = _client.GetCachedGuild(guildId);

            Message msg;
            if (channel == null || !_client.messageCache.TryGet(xm => xm.Id == messageId && xm.ChannelId == channelId, out msg))
                msg = new Message { Id = messageId, ChannelId = channelId, };

            _client.messageCache.Remove(xm => xm.Id == msg.Id && xm.ChannelId == channelId);

            var ea = new MessageDeleteEventArgs() { Message = msg };
            await messageDeleted.InvokeAsync(ea);
        }

        private async Task SendHeartbeatAsync()
        {
            Debug.WriteLine("Sending heartbeat...");
            await SendAsync(GatewayOpCode.Heartbeat, _seq);

            this._lastHeartbeat = DateTimeOffset.Now;
            Interlocked.Increment(ref this._skippedHeartbeats);
        }

        internal Task HandleHeartbeatAckAsync()
        {
            Interlocked.Decrement(ref this._skippedHeartbeats);

            var ping = (int)(DateTime.Now - this._lastHeartbeat).TotalMilliseconds;
            this._ping = ping;

            Debug.WriteLine("Got heartbeat ack in {0}ms!", ping);

            return Task.Delay(0);
        }

        private void QueueTask(Func<Task> task)
        {
            _taskQueue.Enqueue(task);

            if (_queueTask == null || _queueTask.IsCompleted)
                _queueTask = RunQueueAsync();
        }

        private async Task RunQueueAsync()
        {
            Func<Task> task;
            while (_taskQueue.TryDequeue(out task))
            {
                try
                {
                    await task();
                }
                catch (Exception ex)
                {
                    //_logger.LogError(ex, "An error occured in the task queue!");
                    Debug.WriteLine("An error occured in the task queue!, {0}", ex);
                }
            }
        }

        private Task SendAsync<T>(GatewayOpCode op, T data)
        {
            var gatewayPayload = new GatewayPayload<T>() { OpCode = op, Data = data };
            return SendAsync(JsonConvert.SerializeObject(gatewayPayload));
        }

        private Task SendAsync(string message)
        {
            Debug.WriteLine("^ {0}", message);
            return Task.Run(() => _webSocket.Send(message));
        }

        private static void OnError(string arg1, Exception arg2)
        {

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
    }
}
