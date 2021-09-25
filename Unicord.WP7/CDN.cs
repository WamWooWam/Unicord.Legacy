using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unicord.WP7
{
    internal class CDN
    {
        public static string AvatarUrl(ulong id, string avatarHash, string discriminator)
        {
            if (avatarHash != null)
            {
                return string.Format("https://cdn.discordapp.com/avatars/{0}/{1}.png?size=64",id, avatarHash);
            }

            return string.Format("https://cdn.discordapp.com/embed/avatars/{0}.png?size=64", int.Parse(discriminator) % 5);
        }

        public static string IconUrl(ulong id, string iconHash)
        {
            return string.Format("https://cdn.discordapp.com/icons/{0}/{1}.png?size=256", id, iconHash);
        }
    }
}
