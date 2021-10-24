using DiscordLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;
using Unicord.WP7;

namespace Unicord.WP7.ViewModels
{
    public class ChannelGroup : List<Channel>
    {
        public Channel Key { get; set; }

        public ChannelGroup(Channel key, List<Channel> values)
        {
            Key = key;
            AddRange(values);
        }
    }

    public class ChannelPickerGuildViewModel : GuildViewModel
    {
        public ChannelPickerGuildViewModel(Guild guild)
            : base(guild)
        {
            //InitialiseLists();
            RawChannels = new ObservableCollection<Channel>();
        }

        /// <summary>
        /// Asynchronously populates the server list
        /// </summary>
        internal void InitialiseLists()
        {
            Channels = null;
            RawChannels.Clear();

            TaskEx.Run(() =>
            {
                var currentMember = _guild.GetCurrentMember();
                var permissions = _guild.GetDefaultPermissions(currentMember);

                // var channels = Guild.Channels.Values;
                var channels = _guild.Channels.Values;
                var maxPos = channels.Max(c => c.Position) + 1;

                // Use new discord channel category behaviour (new as of 2017 KEKW)

                var orderedChannels = channels.Where(c => c.Type != ChannelType.Category)
                    .Where(c => ShouldShowChannel(c, currentMember))
                    .OrderBy(c => c.Type == ChannelType.Voice)
                    .ThenBy(c => c.Position)
                    .GroupBy(g => g.ParentId != null ? _guild.Channels[g.ParentId.Value] : null)
                    .OrderBy(g => g.Key != null ? g.Key.Position : 0)
                    .SelectMany(g => g.Key != null ? g.Prepend(g.Key) : g);

                foreach (var channel in orderedChannels)
                {
                    this.syncContext.Post((o) => RawChannels.Add((Channel)o), channel);
                }
            });
        }

        private bool ShouldShowChannel(Channel channel, Member currentMember)
        {
            if (currentMember.Id == _guild.OwnerId)
                return true;

            return channel.Type == ChannelType.Category ?
                _guild.Channels.Values.Where(g => g.ParentId == channel.Id).Any(x => x.PermissionsFor(currentMember).HasPermission(Permissions.AccessChannels)) :
                channel.PermissionsFor(currentMember).HasPermission(Permissions.AccessChannels);
        }

        public ObservableCollection<Channel> RawChannels { get; set; }
    }
}
