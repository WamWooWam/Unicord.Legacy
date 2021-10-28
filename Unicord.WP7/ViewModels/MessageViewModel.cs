using DiscordLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unicord.WP7.Providers;

namespace Unicord.WP7.ViewModels
{
    public class MockMessageViewModel : MessageViewModel
    {
        public MockMessageViewModel()
            : base(null)
        {
        }

        public override string AuthorName { get; set; }

        public override string Content { get; set; }

        public override string AvatarUrl { get; set; }

        public override bool IsOutgoing { get; set; }
    }

    public class MessageViewModel : BaseViewModel, IHasAuthor, IHasUser, IHasChannel, IHasGuild
    {
        protected Message message;

        public MessageViewModel(Message message)
            : base()
        {
            this.message = message;
        }

        public Message Message { get { return message; } }
        public User Author { get { return message.Author; } }
        public Channel Channel { get { return Discord.GetCachedChannel(message.ChannelId); } }
        public Guild Guild { get { return Discord.GetCachedGuild(Channel.GuildId); } }

        public ulong Id
        {
            get { return message.Id; }
        }

        public ulong AuthorId
        {
            get { return message.Author.Id; }
        }

        public virtual string AuthorName
        {
            get { return message.Author.Username; }
            set { }
        }

        public virtual string Content
        {
            get { return message.Content; }
            set { }
        }

        public virtual string AvatarUrl
        {
            get { return CDN.AvatarUrl(message.Author.Id, message.Author.AvatarHash, message.Author.Discriminator); }
            set { }
        }

        public virtual bool IsOutgoing
        {
            get { return AuthorId == Discord.CurrentUser.Id; }
            set { }
        }

        public string ImageSource
        {
            get
            {
                if (message.Attachments != null && message.Attachments.Length > 0 && message.Attachments[0].Width != 0)
                {
                    var attach = message.Attachments[0];
                    var width = (float)attach.Width;
                    var height = (float)attach.Height;

                    Utils.Scale(ref width, ref height, 384, 600);

                    return string.Concat(message.Attachments[0].ProxyUrl, "?format=jpeg&width=", Math.Ceiling(width), "&height=", Math.Ceiling(height));
                }

                return null;
            }
        }

        User IHasUser.User { get { return Author; } }
    }
}
