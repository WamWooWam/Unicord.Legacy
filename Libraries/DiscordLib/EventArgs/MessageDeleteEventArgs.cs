using DiscordLib.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordLib.EventArgs
{
    public class MessageDeleteEventArgs : AsyncEventArgs
    {
        public Message Message { get; internal set; }
    }
}
