using DiscordLib.Net.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace DiscordLib.Net
{
    class MultipartRestRequest : RestRequest
    {
        private IProgress<HttpProgress> progress;

        public Dictionary<string, object> StringValues { get; set; }
        public Dictionary<string, Stream> StreamValues { get; set; }

        public MultipartRestRequest(
            Uri url,
            Dictionary<string, object> stringValues = null,
            Dictionary<string, Stream> streamValues = null,
            string method = "POST",
            IProgress<HttpProgress> progress = null)
            : base(url, method)
        {
            this.progress = progress;
            StringValues = stringValues != null ? new Dictionary<string, object>(stringValues) : new Dictionary<string, object>();
            StreamValues = streamValues != null ? new Dictionary<string, Stream>(streamValues) : new Dictionary<string, Stream>();
        }

        public MultipartRestRequest(
            string route,
            Dictionary<string, object> stringValues = null,
            Dictionary<string, Stream> streamValues = null,
            string method = "POST",
            IProgress<HttpProgress> progress = null)
            : this(new Uri(route, UriKind.Relative), stringValues, streamValues, method, progress)
        {

        }

        public override HttpRequestMessage CreateRequestMessage()
        {
            var message = base.CreateRequestMessage();
            var content = new MultipartFormDataContent();
            foreach (var stringItem in StringValues)
            {
                var value = stringItem.Value;
                if (value is string)
                {
                    content.Add(new StringContent(value as string), stringItem.Key);
                }
                else
                {
                    content.Add(new StringContent(JsonConvert.SerializeObject(value)), stringItem.Key);
                }
            }

            var i = 0;
            foreach (var streamItem in StreamValues)
            {
                content.Add(progress != null ? (HttpContent)new StreamContentWithProgress(streamItem.Value, progress) : new StreamContent(streamItem.Value), string.Format("file{0}", i), streamItem.Key);
                i++;
            }

            message.Content = content;
            return message;
        }
    }
}
