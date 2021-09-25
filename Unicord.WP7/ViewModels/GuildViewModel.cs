using DiscordLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Unicord.WP7.ViewModels
{
    public class GuildViewModel : BaseViewModel
    {
        protected Guild _guild;
        public GuildViewModel(Guild guild)
        {
            _guild = guild;
            Channels = new ObservableCollection<ChannelViewModel>();
            foreach (var item in guild.Channels)
                Channels.Add(new ChannelViewModel(item.Value));
        }

        public ulong Id { get { return _guild.Id; } }

        public string Name { get { return _guild.Name; } }

        public string IconUrl { get { return CDN.IconUrl(_guild.Id, _guild.IconHash); } }

        public bool IsAvailable { get { return !_guild.IsUnavailable; } }

        public ObservableCollection<ChannelViewModel> Channels { get; set; }

        internal void Update(Guild guild)
        {
            InvokePropertyChanged("Name");
            InvokePropertyChanged("IconUrl");
            InvokePropertyChanged("IsAvailable");
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
