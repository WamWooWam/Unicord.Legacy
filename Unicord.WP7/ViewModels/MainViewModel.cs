using DiscordLib;
using DiscordLib.EventArgs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Unicord.WP7.ViewModels
{
    public class MainViewModel : BaseViewModel, IDisposable
    {
        private SynchronizationContext _syncContext;

        public MainViewModel()
        {
            _syncContext = SynchronizationContext.Current;
            if (Discord == null)
                return;

            Guilds = new ObservableCollection<GuildViewModel>(
                Discord.Guilds.Values.Select(g => new GuildViewModel(g)));

            DirectMessages = new ObservableCollection<ChannelViewModel>(
                Discord.PrivateChannels.Values
                    .Where(c => c.Type == ChannelType.Private)
                    .Select(c => new ChannelViewModel(c))
            );

            Discord.Socket.Ready += OnReady;
            Discord.Socket.GuildCreated += OnGuildCreated;
        }

        private Task OnReady()
        {
            _syncContext.Post((o) =>
            {
                Guilds.Clear();
                DirectMessages.Clear();
            }, null);

            var guilds = new ObservableCollection<GuildViewModel>();
            var folders = Discord.UserSettings.GuildFolders;
            foreach (var folder in folders)
            {
                Guild guild;
                foreach (var guildId in folder.GuildIds)
                {
                    if (Discord.Guilds.TryGetValue(guildId, out guild))
                        guilds.Add(new GuildViewModel(guild));
                }
            }

            foreach (var guild in Discord.Guilds.Values)
            {
                if (!guilds.Any(g => g.Id == guild.Id))
                    guilds.Insert(0, new GuildViewModel(guild));
            }

            var channels = new ObservableCollection<ChannelViewModel>();
            foreach (var channel in Discord.PrivateChannels.Values.OrderBy(c => c.Recipients[0].Username))
            {
                if (channel.Type == ChannelType.Private)
                    channels.Add(new ChannelViewModel(channel));
            }

            foreach (var item in guilds)
            {
                _syncContext.Post((g) => Guilds.Add((GuildViewModel)g), item);
            }

            foreach (var item in channels)
            {
                _syncContext.Post((c) => DirectMessages.Add((ChannelViewModel)c), item);
            }


            //_syncContext.Post((o) =>
            //{
            //    Guilds = guilds;
            //    DirectMessages = channels;
            //    InvokePropertyChanged("");
            //}, null);
            return Task.Delay(0);
        }

        public ObservableCollection<GuildViewModel> Guilds { get; set; }
        public ObservableCollection<ChannelViewModel> DirectMessages { get; set; }

        private Task OnGuildCreated(GuildCreatedEventArgs e)
        {
            //if(!Guilds.Any(g => g.Id == e.Guild.Id))
            //    _syncContext.Post((o) => Guilds.Add(new GuildViewModel(e.Guild)), null);

            var guildModel = Guilds.FirstOrDefault(g => g.Id == e.Guild.Id);
            _syncContext.Post((o) =>
            {
                if (guildModel != null)
                    guildModel.Update(e.Guild);
                else
                    Guilds.Insert(0, new GuildViewModel(e.Guild));
            }, null);

            return Task.Delay(0);
        }

        public void Dispose()
        {
            if (Discord != null)
            {
                Discord.Socket.Ready -= OnReady;
                Discord.Socket.GuildCreated -= OnGuildCreated;
            }
        }
    }
}
