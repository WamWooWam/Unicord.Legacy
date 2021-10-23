using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Markdown.Helpers;

namespace System.Windows.Controls.Markdown.Parse
{
    public abstract class CustomInline : MarkdownInline
    {
        public CustomInline()
            : base(MarkdownInlineType.Custom)
        {
        }
    }

    public abstract class CustomInlineParser
    {
        public abstract char[] TripChars { get; }
        public abstract InlineParseResult Parse(string markdown, int start, int maxEnd);
    }
}
