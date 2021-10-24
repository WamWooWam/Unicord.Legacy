using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unicord.WP7.ViewModels.Cache
{
    public class MessageViewModelCache
    {
        [JsonConstructor]
        public MessageViewModelCache() { }

        public MessageViewModelCache(MessageViewModel model)
        {
            AuthorName = model.AuthorName;
            AvatarUrl = model.AvatarUrl;
            Content = model.Content;
            IsOutgoing = model.IsOutgoing;
        }

        [JsonProperty("author_name")]
        public string AuthorName { get; set; }
        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("outgoing")]
        public bool IsOutgoing { get; set; }
        [JsonIgnore]
        public string ImageSource { get; set; }
    }
}
