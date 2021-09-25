using DiscordLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unicord.WP7.ViewModels
{
    public class ChannelViewModel : BaseViewModel
    {
        protected Channel channel;
        protected Guild guild;
        protected PrivateChannel dm;

        public ChannelViewModel(Channel channel)
            : base()
        {
            this.channel = channel;
            this.dm = channel as PrivateChannel;
            this.guild = App.Current.Discord.GetCachedGuild(channel.GuildId);
        }
        
        public ulong Id { get { return channel.Id; } }
        public string Name { get { return channel.Name; } }
        public string Topic { get { return channel.Topic; } }
        public User Recipient { get { return dm.Recipients[0]; } }
        public string IconUrl { get { return CDN.AvatarUrl(Recipient.Id, Recipient.AvatarHash, Recipient.Discriminator); } }
    }
}
