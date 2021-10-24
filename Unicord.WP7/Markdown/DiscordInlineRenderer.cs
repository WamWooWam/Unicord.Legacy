using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Markdown.Display;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using Humanizer;
using Unicord.WP7.Providers;
using DiscordLib;
using System.Windows.Media;
using Unicord.WP7.Tools;
using System.Diagnostics;

namespace Unicord.WP7.Markdown
{
    public class DiscordInlineRenderer : CustomXamlInlineRenderer<DiscordInline>
    {
        private const int EMOTE_SIZE = 32;

        public override void Render(CustomXamlInlineRenderContext context, DiscordInline inline)
        {
            var client = App.Current.EnsureDiscordClient();
            var hasDiscord = context.DataContext as IHasDiscordClient;
            if (hasDiscord != null)
                client = hasDiscord.Discord; // just in case we ever do multiple accounts

            Guild guild = null;
            var hasGuild = context.DataContext as IHasGuild;
            if (hasGuild != null)
                guild = hasGuild.Guild; // doesn't guarantee a guild is present

            switch (inline.DiscordType)
            {
                case MentionType.User:
                    RenderUser(context, inline, client, guild);
                    break;
                case MentionType.Channel:
                    RenderChannel(context, inline, guild);
                    break;
                case MentionType.Role:
                    RenderRole(context, inline, guild);
                    break;
                case MentionType.Emote:
                    RenderEmote(context, inline);
                    break;
                case MentionType.Timestamp:
                    RenderTimestamp(context, inline);
                    break;
                default:
                    Debug.WriteLine("Unknown inline type");
                    break;
            }

        }

        private static void RenderUser(CustomXamlInlineRenderContext context, DiscordInline inline, DiscordClient client, Guild guild)
        {
            Run run = new Run()
            {
                FontWeight = FontWeights.SemiBold,
                TextDecorations = TextDecorations.Underline,
                Text = string.Format("<@{0}>", inline.Id)
            };

            try
            {
                if (client == null) return;

                var user = client.GetCachedUser(inline.Id);
                if (user == null) return;

                Member member;
                if (guild != null && guild.Members.TryGetValue(inline.Id, out member))
                {
                    run.Text = string.Format("@{0}", member.Nickname ?? user.Username);

                    Role role = member.Roles.Select(r => guild.Roles.TryGetValue(r, out role) ? role : null)
                                            .Where(r => r != null)
                                            .OrderByDescending(r => r.Position)
                                            .FirstOrDefault(r => r.Color != 0);
                    if (role != null)
                        run.Foreground = GetBrushForRole(role);

                }
                else
                {
                    run.Text = string.Format("@{0}", user.Username);
                }
            }
            finally
            {
                context.Inlines.Add(run);
            }
        }

        private static void RenderRole(CustomXamlInlineRenderContext context, DiscordInline inline, Guild guild)
        {
            Run run = new Run()
            {
                FontWeight = FontWeights.SemiBold,
                TextDecorations = TextDecorations.Underline,
                Text = string.Format("<@&{0}>", inline.Id)
            };

            try
            {
                if (guild == null) return;

                Role role;
                if (guild.Roles.TryGetValue(inline.Id, out role))
                {
                    run.Text = string.Format("@{0}", role.Name);
                    if (role.Color != 0)
                        run.Foreground = GetBrushForRole(role);
                }
            }
            finally
            {
                context.Inlines.Add(run);
            }
        }

        private static void RenderChannel(CustomXamlInlineRenderContext context, DiscordInline inline, Guild guild)
        {
            Run run = new Run()
            {
                FontWeight = FontWeights.SemiBold,
                TextDecorations = TextDecorations.Underline,
                Text = string.Format("<#{0}>", inline.Id)
            };

            try
            {
                if (guild == null) return;

                Channel channel;
                if (guild.Channels.TryGetValue(inline.Id, out channel))
                {
                    run.Text = string.Format("#{0}", channel.Name);
                }
            }
            finally
            {
                context.Inlines.Add(run);
            }
        }

        private static void RenderEmote(CustomXamlInlineRenderContext context, DiscordInline inline)
        {
            var uri = CDN.EmoteUrl(inline.Id);
            var ui = new InlineUIContainer()
            {
                FontSize = EMOTE_SIZE,
                Child = new Image()
                {
                    Source = new BitmapImage(new Uri(uri)),
                    Height = EMOTE_SIZE,
                    MaxWidth = EMOTE_SIZE * 3,
                    Margin = new Thickness(0, -4, 0, -8)
                }
            };

            context.Inlines.Add(ui);
        }

        private static void RenderTimestamp(CustomXamlInlineRenderContext context, DiscordInline inline)
        {
            var run = new Run() { FontWeight = FontWeights.SemiBold };
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(inline.Timestamp).ToLocalTime();

            switch (inline.TimestampFormat)
            {
                case TimestampFormat.ShortDateTime:
                    run.Text = string.Format("{0} {1}", dateTime.ToShortDateString(), dateTime.ToShortTimeString());
                    break;
                case TimestampFormat.LongDateTime:
                    run.Text = string.Format("{0} {1}", dateTime.ToLongDateString(), dateTime.ToLongTimeString());
                    break;
                case TimestampFormat.ShortTime:
                    run.Text = dateTime.ToShortTimeString();
                    break;
                case TimestampFormat.LongTime:
                    run.Text = dateTime.ToLongTimeString();
                    break;
                case TimestampFormat.ShortDate:
                    run.Text = dateTime.ToShortDateString();
                    break;
                case TimestampFormat.LongDate:
                    run.Text = dateTime.ToLongDateString();
                    break;
                case TimestampFormat.Relative:
                    run.Text = dateTime.Humanize();
                    break;
                default:
                    break;
            }

            context.Inlines.Add(run);
        }

        private static Brush GetBrushForRole(Role role)
        {
            if (role.Id == 206833489981603851 || role.Id == 648524791019732992)
            {
                var collection = new GradientStopCollection()
                {
                    new GradientStop() { Color = Colors.Red, Offset = 0 },
                    new GradientStop() { Color = Colors.Orange, Offset = 0.17 },
                    new GradientStop() { Color = Colors.Yellow, Offset = 0.33 },
                    new GradientStop() { Color = Colors.Green, Offset = 0.50 },
                    new GradientStop() { Color = Colors.Blue, Offset = 0.67 },
                    new GradientStop() { Color = Color.FromArgb(0xff, 0x4b, 0x00, 0x82), Offset = 0.84 },
                    new GradientStop() { Color = Color.FromArgb(0xff, 0xee, 0x82, 0xee), Offset = 1 }
                };

                return new LinearGradientBrush(collection, 0);
            }

            var colour = Color.FromArgb(255, (byte)((role.Color & 0xff0000) >> 16), (byte)((role.Color & 0xff00) >> 8), (byte)(role.Color & 0xff));
            var hsbColor = HSBColor.FromColor(colour);
            hsbColor.B = Math.Min(hsbColor.B * 1.1, 1);

            return new SolidColorBrush(hsbColor.ToColor());
        }
    }
}
