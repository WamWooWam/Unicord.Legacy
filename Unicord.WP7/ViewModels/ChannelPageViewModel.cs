using DiscordLib;
using DiscordLib.EventArgs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicord.WP7.ViewModels
{
    public class MockChannelPageViewModel
    {
        public MockChannelPageViewModel()
        {
            Messages = new ObservableCollection<MockMessageViewModel>();
            Messages.Add(new MockMessageViewModel() { Content = "Hi there!", IsOutgoing = true });
            Messages.Add(new MockMessageViewModel() { Content = "```\nThis is a test\n```" });
        }

        public ObservableCollection<MockMessageViewModel> Messages { get; set; }
    }

    public class ChannelPageViewModel : ChannelViewModel
    {
        public ChannelPageViewModel(Channel channel)
            : base(channel)
        {
            Messages = new ObservableCollection<MessageViewModel>();
            App.Current.Discord.MessageCreated += OnMessageCreated;
            App.Current.Discord.MessageDeleted += OnMessageDeleted;
        }

        public string Title { get { return !string.IsNullOrWhiteSpace(channel.Name) ? channel.Name : dm.Recipients[0].Username; } }

        public string SubTitle { get { return guild == null ? "direct messages" : guild.Name; } }

        public ObservableCollection<MessageViewModel> Messages { get; set; }

        public async Task LoadAsync()
        {
            var messages = await App.Current.Discord.Rest.GetMessagesAsync(channel, 25);
            syncContext.Post(d =>
            {
                Messages.Clear();
                foreach (var item in messages.Select(m => new MessageViewModel(m)).Reverse())
                    Messages.Add(item);
            }, this);
        }

        private Task OnMessageCreated(MessageCreateEventArgs e)
        {
            if (e.Message.ChannelId == channel.Id)
                syncContext.Post(a => Messages.Add(new MessageViewModel(e.Message)), null);

            return TaskEx.Delay(0);
        }

        private Task OnMessageDeleted(MessageDeleteEventArgs e)
        {
            if (e.Message.ChannelId == channel.Id)
                syncContext.Post(a => Messages.Remove(Messages.FirstOrDefault(m => m.Id == e.Message.Id)), null);

            return TaskEx.Delay(0);
        }
    }
}
