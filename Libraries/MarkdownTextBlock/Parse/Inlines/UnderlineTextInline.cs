// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls.Markdown.Helpers;

namespace System.Windows.Controls.Markdown.Parse
{
    /// <summary>
    /// Represents a span that contains Underline text.
    /// </summary>
    internal class UnderlineTextInline : MarkdownInline, IInlineContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnderlineTextInline"/> class.
        /// </summary>
        public UnderlineTextInline()
            : base(MarkdownInlineType.Underline)
        {
        }

        /// <summary>
        /// Gets or sets the contents of the inline.
        /// </summary>
        public IList<MarkdownInline> Inlines { get; set; }

        /// <summary>
        /// Returns the chars that if found means we might have a match.
        /// </summary>
        internal static void AddTripChars(List<InlineTripCharHelper> tripCharHelpers)
        {
            tripCharHelpers.Add(new InlineTripCharHelper() { FirstChar = '_', Method = InlineParseMethod.Underline });
        }

        /// <summary>
        /// Attempts to parse a underline text span.
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location to start parsing. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <returns> A underline bold text span, or <c>null</c> if this is not a underline text span. </returns>
        internal static InlineParseResult Parse(string markdown, int start, int maxEnd)
        {
            if (start >= maxEnd - 1)
            {
                return null;
            }

            // Check the start sequence.
            string startSequence = markdown.Substring(start, 2);
            if (startSequence != "__")
            {
                return null;
            }

            // Find the end of the span.  The end sequence (either '**' or '__') must be the same
            // as the start sequence.
            int innerStart = start + 2;
            int innerEnd = ParseHelpers.IndexOf(markdown, startSequence, innerStart, maxEnd);
            if (innerEnd == -1)
            {
                return null;
            }

            // The span must contain at least one character.
            if (innerStart == innerEnd)
            {
                return null;
            }

            // The first character inside the span must NOT be a space.
            if (ParseHelpers.IsWhiteSpace(markdown[innerStart]))
            {
                return null;
            }

            // The last character inside the span must NOT be a space.
            if (ParseHelpers.IsWhiteSpace(markdown[innerEnd - 1]))
            {
                return null;
            }

            // We found something!
            var result = new UnderlineTextInline
            {
                Inlines = ParseHelpers.ParseInlineChildren(markdown, innerStart, innerEnd)
            };

            Debug.WriteLine(string.Format("{0} {1}", markdown[innerStart], markdown[innerEnd]));

            return new InlineParseResult(result, start, innerEnd + 2);
        }

        /// <summary>
        /// Converts the object into it's textual representation.
        /// </summary>
        /// <returns> The textual representation of this object. </returns>
        public override string ToString()
        {
            if (Inlines == null)
            {
                return base.ToString();
            }

            return "*" + string.Join(string.Empty, Inlines) + "*";
        }
    }
}
