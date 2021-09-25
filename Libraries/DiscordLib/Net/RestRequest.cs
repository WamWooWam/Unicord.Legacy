using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace DiscordLib.Net
{
    internal class RestRequest
    {
        public RestRequest(Uri url, string method = "GET")
        {
            Url = url;
            Method = method;
            Headers = new Dictionary<string, string>();
        }

        public RestRequest(string route, string method = "GET")
            : this(new Uri(route, UriKind.Relative), method)
        {
        }

        public Uri Url { get; private set; }
        public string Method { get; private set; }
        public IDictionary<string, string> Headers { get; private set; }

        public virtual HttpRequestMessage CreateRequestMessage()
        {
            var message = new HttpRequestMessage(new HttpMethod(Method), Url);
            foreach (var header in Headers)
                message.Headers.Add(header.Key, header.Value);

            message.Headers.CacheControl = new CacheControlHeaderValue() { NoCache = true };
            message.Headers.IfModifiedSince = DateTimeOffset.UtcNow;

            return message;
        }

        public override string ToString()
        {
            return string.Format("RestRequest: {0} /{1}", Method, Url);
        }
    }
}
