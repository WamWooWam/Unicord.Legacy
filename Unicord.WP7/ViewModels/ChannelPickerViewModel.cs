using DiscordLib;
using DiscordLib.EventArgs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Unicord.WP7.ViewModels
{
    public class ChannelPickerViewModel
    {
        private SynchronizationContext _syncContext;

        public ChannelPickerViewModel(ulong guildId)
        {
            _syncContext = SynchronizationContext.Current;
            Guild = new ChannelPickerGuildViewModel(App.Current.Discord.Guilds[guildId]);
        }

        public ChannelPickerGuildViewModel Guild { get; set; }
    }
}
