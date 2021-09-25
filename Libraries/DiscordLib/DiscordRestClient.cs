﻿using DiscordLib.Net;
using DiscordLib.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordLib
{
    public class DiscordRestClient
    {
        private RestClient _restClient;
        private DiscordClient _client;

        private const string _ieUA = "Mozilla/5.0 (compatible; MSIE 9.0; Windows Phone OS 7.5; Trident/5.0; IEMobile/9.0; Microsoft; XDeviceEmulator)";
        private const string _botUA = "DiscordBot (https://wamwoowam.co.uk, 1.0.0), Windows Phone 7.5";

        internal DiscordRestClient(DiscordClient client, string token)
        {
            _client = client;
            _restClient = new RestClient(new Uri("https://discordapp.com/api/v9"), _ieUA);
            _restClient.Headers.TryAddWithoutValidation("Authorization", token);
        }

        public async Task<Uri> GetGatewayUrlAsync()
        {
            var request = new RestRequest("gateway");
            var response = await _restClient.SendRequestAsync(request);
            return new Uri(JObject.Parse(response.Content).Value<string>("url"));
        }

        public Task<User> GetCurrentUserAsync()
        {
            return this.GetUserAsync("@me");
        }

        public Task<User> GetUserAsync(ulong userId)
        {
            User user;
            if (_client.userCache.TryGetValue(userId, out user))
                return TaskEx.FromResult(user);

            return GetUserAsync(userId.ToString());
        }

        private async Task<User> GetUserAsync(string userId)
        {
            var request = new RestRequest(string.Format("users/{0}", userId));
            var response = await _restClient.SendRequestAsync<User>(request);
            var user = response.Content;
            return _client.userCache.AddOrUpdate(user.Id, user, (id, u) => u.Update(user));
        }

        public async Task<Message> CreateMessageAsync(ulong channelId, string content, bool tts)
        {
            var request = new JsonRestRequest(string.Format("channels/{0}/messages", channelId), new { content, tts });
            var response = await _restClient.SendRequestAsync<Message>(request);
            return response.Content;
        }


        public async Task<Message> CreateMessageAsync(ulong channelId, string fileName, Stream stream, IProgress<HttpProgress> progress = null)
        {
            var requestStreams = new Dictionary<string, Stream>() { {fileName, stream} };

            var request = new MultipartRestRequest(string.Format("channels/{0}/messages", channelId), streamValues: requestStreams, progress: progress);
            var response = await _restClient.SendRequestAsync<Message>(request);
            return response.Content;
        }

        public async Task<List<Message>> GetMessagesAsync(ulong channelId, int limit, ulong? before = null, ulong? after = null, ulong? around = null)
        {
            var urlparams = new Dictionary<string, string>();
            if (around != null)
                urlparams["around"] = around.Value.ToString(CultureInfo.InvariantCulture);
            if (before != null)
                urlparams["before"] = before.Value.ToString(CultureInfo.InvariantCulture);
            if (after != null)
                urlparams["after"] = after.Value.ToString(CultureInfo.InvariantCulture);
            if (limit > 0)
                urlparams["limit"] = limit.ToString(CultureInfo.InvariantCulture);

            var request = new RestRequest(string.Format("channels/{0}/messages{1}", channelId, BuildQueryString(urlparams)));
            var response = await _restClient.SendRequestAsync<List<Message>>(request);

            return response.Content;
        }

        private static string BuildQueryString(IEnumerable<KeyValuePair<string, string>> values, bool post = false)
        {
            if (values == null || !values.Any())
                return string.Empty;

            var vals_collection = values.Select(xkvp =>
                string.Format("{0}={1}", HttpUtility.UrlEncode(xkvp.Key), HttpUtility.UrlEncode(xkvp.Value)));
            var vals = string.Join("&", vals_collection);

            return !post ? "?" + vals : vals;
        }
    }
}