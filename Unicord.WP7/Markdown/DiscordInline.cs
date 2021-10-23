using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Markdown.Display;
using System.Windows.Controls.Markdown.Helpers;
using System.Windows.Controls.Markdown.Parse;

namespace Unicord.WP7.Markdown
{
    public class DiscordInline : CustomInline
    {
        public MentionType DiscordType { get; set; }

        public ulong Id { get; set; }

        public string Text { get; set; }

        public long Timestamp { get; set; }

        public TimestampFormat TimestampFormat { get; set; }
    }
}
