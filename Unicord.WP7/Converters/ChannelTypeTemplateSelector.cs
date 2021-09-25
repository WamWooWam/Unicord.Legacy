using DiscordLib;
using System.Windows;
using Unicord.WP7.Controls;

namespace Unicord.WP7.Converters
{
    public class ChannelTypeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextChannelTemplate { get; set; }
        public DataTemplate VoiceChannelTemplate { get; set; }
        public DataTemplate CategoryTemplate { get; set; }
        public DataTemplate DMChannelTemplate { get; set; }
        public DataTemplate GroupChannelTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Channel)
            {
                var channel = item as Channel;
                switch (channel.Type)
                {
                    case ChannelType.Text:
                    case ChannelType.Store:
                    case ChannelType.News:
                    case ChannelType.Unknown:
                        return TextChannelTemplate;
                    case ChannelType.Voice:
                        return VoiceChannelTemplate ?? TextChannelTemplate;
                    case ChannelType.Private:
                        return DMChannelTemplate;
                    case ChannelType.Group:
                        return GroupChannelTemplate ?? DMChannelTemplate;
                    case ChannelType.Category:
                        return CategoryTemplate;
                }
            }

            return TextChannelTemplate;
        }
    }
}
