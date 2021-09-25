using DiscordLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unicord.WP7.ViewModels
{
    public class MockMessageViewModel
    {
        public string AuthorName { get; set; }

        public string Content { get; set; }

        public string AvatarUrl { get; set; }

        public bool IsOutgoing { get; set; }
    }

    public class MessageViewModel : BaseViewModel
    {
        protected Message message;
        public MessageViewModel(Message message)
            : base()
        {
            this.message = message;
        }

        public ulong Id { get { return message.Id; } }

        public ulong AuthorId { get { return message.Author.Id; } }

        public string AuthorName { get { return message.Author.Username; } }

        public string Content { get { return message.Content; } }

        public string AvatarUrl { get { return CDN.AvatarUrl(message.Author.Id, message.Author.AvatarHash, message.Author.Discriminator); } }

        public string ImageSource
        {
            get
            {
                if (message.Attachments != null && message.Attachments.Length > 0 && message.Attachments[0].Width != 0)
                {
                    var attach = message.Attachments[0];
                    var width = (float)attach.Width;
                    var height = (float)attach.Height;

                    Tools.Scale(ref width, ref height, 384, 600);

                    return string.Concat(message.Attachments[0].ProxyUrl, "?format=jpeg&width=", width, "&height=", height);
                }

                return null;
            }
        }
    }
}
