using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Controls.Markdown.Helpers;
using System.Windows.Controls.Markdown.Parse;

namespace Unicord.WP7.Markdown
{
    public class DiscordInlineParser : CustomInlineParser
    {
        private static Dictionary<char, TimestampFormat> TimespanTypeMap = new Dictionary<char, TimestampFormat>() 
        {
            {'f', TimestampFormat.ShortDateTime},
            {'F', TimestampFormat.LongDateTime},
            {'t', TimestampFormat.ShortTime},
            {'T', TimestampFormat.LongTime},
            {'d', TimestampFormat.ShortDate},
            {'D', TimestampFormat.LongDate},
            {'R', TimestampFormat.Relative}
        };

        public override char[] TripChars
        {
            get { return new[] { '<' }; }
        }

        public override InlineParseResult Parse(string markdown, int start, int maxEnd)
        {
            if (start >= maxEnd - 1)
            {
                return null;
            }

            // Check the start sequence.
            var startSequence = markdown[start];
            if (startSequence != '<')
            {
                return null;
            }

            // Find the end of the span.
            var innerStart = start + 1;
            var innerEnd = ParseHelpers.IndexOf(markdown, '>', innerStart, maxEnd);
            if (innerEnd == -1)
            {
                return null;
            }

            // The span must contain at least one character.
            if (innerStart == innerEnd || (innerEnd - innerStart) < 4)
            {
                return null;
            }

            var text = markdown.Substring(innerStart, innerEnd - innerStart);

            var builder = new StringBuilder();
            var index = 0;
            var type = MentionType.User;
            char c;

            switch (text[index])
            {
                case '@':
                    // user or role mention

                    index++;
                    c = text[index];
                    type = MentionType.User;
                    ParseUserRoleMention(text, builder, ref index, ref type, c);
                    break;
                case '#':
                    // channel mention

                    index++;
                    c = text[index];
                    type = MentionType.Channel;

                    if (char.IsDigit(c))
                    {
                        ReadDigits(text, builder, ref index);
                    }
                    break;
                case 't':
                    index += 2;
                    type = MentionType.Timestamp;
                    return ParseTimestampMention(text, builder, index, innerStart, innerEnd);
                case 'a':
                    // animated emote
                    index += 2;
                    type = MentionType.Emote;
                    return ParseEmoteMention(text, builder, index, innerStart, innerEnd);
                case ':':
                    // normal emote
                    index++;
                    type = MentionType.Emote;
                    return ParseEmoteMention(text, builder, index, innerStart, innerEnd);
                default:
                    return null;
            }

            ulong id;
            if (ulong.TryParse(builder.ToString(), out id))
            {
                return new InlineParseResult(new DiscordInline() { Id = id, DiscordType = type }, innerStart - 1, innerEnd + 1);
            }

            return null;
        }

        private static void ParseUserRoleMention(string text, StringBuilder builder, ref int index, ref MentionType type, char c)
        {
            if (c == '!')
            {
                index++;
                ReadDigits(text, builder, ref index);
            }
            else if (char.IsDigit(c))
            {
                ReadDigits(text, builder, ref index);
            }
            else if (c == '&')
            {
                index++;
                ReadDigits(text, builder, ref index);
                type = MentionType.Role;
            }
        }

        private static InlineParseResult ParseTimestampMention(string text, StringBuilder builder, int index, int innerStart, int innerEnd)
        {
            string timestampText;
            TimestampFormat format;

            // after the : is a timespan flag
            var nextIndex = text.IndexOf(':', index);
            if (nextIndex != -1)
            {
                timestampText = text.Substring(index, nextIndex - index);
                if (!TimespanTypeMap.TryGetValue(text[nextIndex + 1], out format))
                {
                    format = TimestampFormat.ShortDateTime;
                    Debug.WriteLine("Unable to find timespan type " + text[index]);
                }
            }
            else
            {
                timestampText = text.Substring(index);
                format = TimestampFormat.ShortDateTime;
            }

            //Debug.WriteLine(timestampText);
            //Debug.WriteLine(format);

            long timestamp;
            if (!long.TryParse(timestampText, out timestamp))
            {
                return null;
            }

            return new InlineParseResult(new DiscordInline() { DiscordType = MentionType.Timestamp, Timestamp = timestamp, TimestampFormat = format}, innerStart - 1, innerEnd + 1);
        }

        private static InlineParseResult ParseEmoteMention(string text, StringBuilder builder, int index, int innerStart, int innerEnd)
        {
            var nextIndex = text.IndexOf(':', index);
            if (nextIndex == -1)
            {
                return null;
            }

            var emoteName = text.Substring(index, nextIndex - index);
            nextIndex++;

            ReadDigits(text, builder, ref nextIndex);

            ulong id;
            if (ulong.TryParse(builder.ToString(), out id))
            {
                return new InlineParseResult(new DiscordInline() { Id = id, DiscordType = MentionType.Emote, Text = emoteName }, innerStart - 1, innerEnd + 1);
            }

            return null;
        }

        private static void ReadDigits(string text, StringBuilder builder, ref int index)
        {
            char c;
            while (index < text.Length && (char.IsDigit(c = text[index])))
            {
                builder.Append(c);
                index++;
            }
        }
    }
}
