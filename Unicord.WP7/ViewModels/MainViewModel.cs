using DiscordLib;
using DiscordLib.EventArgs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Unicord.WP7.ViewModels
{
    public class MainViewModel : BaseViewModel, IDisposable
    {
        private SynchronizationContext _syncContext;

        public MainViewModel()
        {
            _syncContext = SynchronizationContext.Current;

            Guilds = new ObservableCollection<GuildViewModel>(
                App.Current.Discord.Guilds.Values.Select(g => new GuildViewModel(g)));

            DirectMessages = new ObservableCollection<ChannelViewModel>(
                App.Current.Discord.PrivateChannels.Values
                    .Where(c => c.Type == ChannelType.Private)
                    .Select(c => new ChannelViewModel(c)));

            App.Current.Discord.Ready += OnReady;
            App.Current.Discord.GuildCreated += OnGuildCreated;
        }

        private Task OnReady()
        {
            var guilds = new ObservableCollection<GuildViewModel>();
            var folders = App.Current.Discord.CurrentUserSettings.GuildFolders;
            foreach (var folder in folders)
            {
                Guild guild;
                foreach (var guildId in folder.GuildIds)
                {
                    if (App.Current.Discord.Guilds.TryGetValue(guildId, out guild))
                        guilds.Add(new GuildViewModel(guild));
                }
            }

            foreach (var guild in App.Current.Discord.Guilds.Values)
            {
                if (!guilds.Any(g => g.Id == guild.Id))
                    guilds.Insert(0, new GuildViewModel(guild));
            }

            var channels = new ObservableCollection<ChannelViewModel>();
            foreach (var channel in App.Current.Discord.PrivateChannels.Values)
            {
                if (channel.Type == ChannelType.Private)
                    channels.Add(new ChannelViewModel(channel));
            }

            _syncContext.Post((o) =>
            {
                Guilds = guilds;
                DirectMessages = channels;
                InvokePropertyChanged("");
            }, null);
            return TaskEx.Delay(0);
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
            return TaskEx.Delay(0);
        }

        public void Dispose()
        {
            if (App.Current.Discord != null)
            {
                App.Current.Discord.Ready -= OnReady;
                App.Current.Discord.GuildCreated -= OnGuildCreated;
            }
        }
    }
}
