using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordLib.Net
{
    public class DiscordApiException : Exception
    {
        public DiscordApiException(int code, string message) : base(message) { this.HResult = code; }
        public DiscordApiException(int code, string message, Exception inner) : base(message, inner) { this.HResult = code; }
    }
}
