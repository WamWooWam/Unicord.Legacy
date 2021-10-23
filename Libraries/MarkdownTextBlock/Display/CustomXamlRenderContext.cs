using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Markdown.Parse;
using System.Windows.Documents;

namespace System.Windows.Controls.Markdown.Display
{
    public class CustomXamlInlineRenderContext
    {
        private XamlRenderer _renderer;
        private XamlRenderContext _renderContext;

        internal CustomXamlInlineRenderContext(
            XamlRenderer renderer, 
            XamlRenderContext renderContext,
            InlineCollection inlines,
            TextElement parent)
        {
            _renderer = renderer;
            _renderContext = renderContext;

            Parent = parent;
            Inlines = inlines;
        }

        public TextElement Parent { get; private set; }
        public InlineCollection Inlines { get; private set; }
        public object DataContext { get { return _renderContext.DataContext; } }

        public void RenderInlineChildren(InlineCollection inlineCollection, IList<MarkdownInline> inlineElements, TextElement parent)
        {
            _renderer.RenderInlineChildren(inlineCollection, inlineElements, parent, _renderContext);
        }
    }
}
