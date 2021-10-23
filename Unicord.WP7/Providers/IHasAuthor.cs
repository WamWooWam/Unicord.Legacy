using DiscordLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unicord.WP7.Providers
{
    internal interface IHasAuthor
    {
        User Author { get; }
    }
}
