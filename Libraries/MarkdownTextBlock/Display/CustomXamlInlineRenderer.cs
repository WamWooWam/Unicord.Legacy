using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Markdown.Parse;

namespace System.Windows.Controls.Markdown.Display
{
    public abstract class CustomXamlInlineRenderer
    {
        internal abstract void Render(CustomXamlInlineRenderContext context, MarkdownInline inline);
    }

    public abstract class CustomXamlInlineRenderer<T> : CustomXamlInlineRenderer where T : CustomInline
    {
        internal override void Render(CustomXamlInlineRenderContext context, MarkdownInline inline)
        {
            Render(context, (T)inline);
        }

        public abstract void Render(CustomXamlInlineRenderContext context, T inline);
    }
}
