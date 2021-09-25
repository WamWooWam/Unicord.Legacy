using DiscordLib.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordLib.EventArgs
{
    public class GuildCreatedEventArgs : AsyncEventArgs
    {
        public GuildCreatedEventArgs(Guild guild)
        {
            Guild = guild;
        }

        public Guild Guild { get; private set; }
    }
}
