using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace DiscordLib.Net
{
    class RestResponse<T>
    {
        public int ResponseCode { get; private set; }
        public string ReasonPhrase { get; private set; }
        public T Content { get; private set; }

        public RestResponse(int responseCode, string reasonPhrase, T content)
        {
            ResponseCode = responseCode;
            ReasonPhrase = reasonPhrase;
            Content = content;
        }
    }
}
