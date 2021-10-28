using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unicord.WP7.ViewModels.Cache
{
    public class ChannelPageViewModelCache
    {
        [JsonConstructor]
        public ChannelPageViewModelCache() { }

        public ChannelPageViewModelCache(ChannelPageViewModel model)
        {
            Title = model.Title;
            SubTitle = model.SubTitle;
            Messages = model.Messages.Skip(Math.Max(0, model.Messages.Count - 8))
                                     .Select(m => new MessageViewModelCache(m))
                                     .ToArray();
        }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("subtitle")]
        public string SubTitle { get; set; }

        [JsonProperty("messages")]
        public MessageViewModelCache[] Messages { get; set; }
    }
}
