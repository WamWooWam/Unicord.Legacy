using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace DiscordLib.Net
{
    class JsonRestRequest : JsonRestRequest<object>
    {
        public JsonRestRequest(Uri url, object content, string method = "POST")
            : base(url, content, method) { }

        public JsonRestRequest(string route, object content, string method = "POST")
            : this(new Uri(route, UriKind.Relative), content, method) { }
    }

    class JsonRestRequest<T> : RestRequest
    {
        public T Content { get; set; }

        public JsonRestRequest(Uri url, T content, string method = "POST")
            : base(url, method)
        {
            Content = content;
        }

        public JsonRestRequest(string route, T content, string method = "POST")
            : this(new Uri(route, UriKind.Relative), content, method) { }

        public override HttpRequestMessage CreateRequestMessage()
        {
            var request = base.CreateRequestMessage();
            request.Content = new StringContent(JsonConvert.SerializeObject(Content));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return request;
        }

        public override string ToString()
        {
            return string.Format("JsonWebRequest: {0} {1} -> {2}", Method, Url, JsonConvert.SerializeObject(Content));
        }
    }
}
