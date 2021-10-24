using DiscordLib;
using System.Windows;
using Unicord.WP7.Controls;
using Unicord.WP7.ViewModels;
using Unicord.WP7.ViewModels.Cache;

namespace Unicord.WP7.Converters
{
    public class MessageTypeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate IncomingTemplate { get; set; }
        public DataTemplate OutgoingTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Message)
            {
                var message = item as Message;
                if (message.Author.Id == App.Current.Discord.CurrentUser.Id)
                {
                    return OutgoingTemplate;
                }


                return IncomingTemplate;
            }

            if (item is MessageViewModel)
            {
                var messageVM = item as MessageViewModel;
                if (messageVM.IsOutgoing)
                {
                    return OutgoingTemplate;
                }
            }

            if (item is MessageViewModelCache)
            {
                var messageVM = item as MessageViewModelCache;
                if (messageVM.IsOutgoing)
                {
                    return OutgoingTemplate;
                }
            }

            return IncomingTemplate;
        }
    }
}
