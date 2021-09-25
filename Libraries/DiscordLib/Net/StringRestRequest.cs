using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace DiscordLib.Net
{
    class StringRestRequest : RestRequest
    {
        public string Content { get; set; }
        public string ContentType { get; set; }

        public StringRestRequest(
            Uri url,
            string content,
            string contentType = "application/json",
            string method = "POST")
            : base(url, method)
        {
            Content = content;
            ContentType = contentType;
        }

        public StringRestRequest(
            string route,
            string content,
            string contentType = "application/json",
            string method = "POST")
            : this(new Uri(route, UriKind.Relative), content, contentType, method)
        {

        }

        public override HttpRequestMessage CreateRequestMessage()
        {
            var request = base.CreateRequestMessage();
            request.Content = new StringContent(Content);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
            return request;
        }
    }
}
