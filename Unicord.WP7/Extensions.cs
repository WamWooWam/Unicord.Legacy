using DiscordLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unicord.WP7
{
    public static class Extensions
    {
        /// <summary>
        /// Calculates permissions for a given member.
        /// thanks max lol
        /// </summary>
        /// <param name="mbr">Member to calculate permissions for.</param>
        /// <returns>Calculated permissions for a given member.</returns>
        public static Permissions PermissionsFor(this Channel chan, Member mbr)
        {
            if (chan.Type == ChannelType.Private || chan.Type == ChannelType.Group || chan.GuildId == 0)
                return Permissions.None;

            var guild = App.Current.Discord.Guilds[chan.GuildId];
            if (guild.OwnerId == mbr.Id)
                return (Permissions)(-1);

            Permissions perms;

            // assign @everyone permissions
            var everyoneRole = guild.Roles[guild.Id];
            perms = everyoneRole.Permissions;

            // roles that member is in
            var mbRoles = mbr.Roles.Where(xr => xr != everyoneRole.Id).ToArray();

            // assign permissions from member's roles (in order)
            perms |= mbRoles.Aggregate(Permissions.None, (c, role) => c | guild.Roles[role].Permissions);

            // Adminstrator grants all permissions and cannot be overridden
            if ((perms & Permissions.Administrator) == Permissions.Administrator)
                return (Permissions)(-1);

            // channel overrides for roles that member is in
            var mbRoleOverrides = mbRoles
                .Select(xr => chan.PermissionOverwrites.FirstOrDefault(xo => xo.Id == xr))
                .Where(xo => xo != null)
                .ToList();

            // assign channel permission overwrites for @everyone pseudo-role
            var everyoneOverwrites = chan.PermissionOverwrites.FirstOrDefault(xo => xo.Id == everyoneRole.Id);
            if (everyoneOverwrites != null)
            {
                perms &= ~everyoneOverwrites.Denied;
                perms |= everyoneOverwrites.Allowed;
            }

            // assign channel permission overwrites for member's roles (explicit deny)
            perms &= ~mbRoleOverrides.Aggregate(Permissions.None, (c, overs) => c | overs.Denied);
            // assign channel permission overwrites for member's roles (explicit allow)
            perms |= mbRoleOverrides.Aggregate(Permissions.None, (c, overs) => c | overs.Allowed);

            // channel overrides for just this member
            var mbOverrides = chan.PermissionOverwrites.FirstOrDefault(xo => xo.Id == mbr.Id);
            if (mbOverrides == null) return perms;

            // assign channel permission overwrites for just this member
            perms &= ~mbOverrides.Denied;
            perms |= mbOverrides.Allowed;

            return perms;
        }


        public static Permissions GetDefaultPermissions(this Guild g, Member m)
        {
            // default permissions
            const Permissions def = Permissions.None;

            if (g == null)
            {
                return def;
            }

            if (g.OwnerId == m.Id)
            {
                return ~def;
            }

            Permissions perms;

            // assign @everyone permissions
            var everyoneRole = g.Roles[g.Id];
            perms = everyoneRole.Permissions;

            // roles that member is in
            var mbRoles = m.Roles.Select(r => g.Roles[r]).ToArray();
            perms |= mbRoles.Aggregate(def, (c, role) => c | role.Permissions);

            return perms;
        }

        public static Member GetCurrentMember(this Guild g)
        {
            User user = App.Current.Discord.CurrentUser;
            Member member;
            if (g.Members.TryGetValue(user.Id, out member)) return member;

            member = new Member() { User = user, GuildId = g.Id };

            return g.Members.AddOrUpdate(user.Id, member, (id, m) => m.Update(member));
        }

        struct PrependEnumerable<T> : IEnumerable<T>
        {
            private T item;
            private IEnumerable<T> items;

            internal PrependEnumerable(IEnumerable<T> items, T item)
            {
                this.item = item;
                this.items = items;
            }

            public IEnumerator<T> GetEnumerator()
            {
                yield return item;
                foreach (var nextItem in items)
                    yield return nextItem;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (IEnumerator)GetEnumerator();
            }
        }

        /// <summary>
        /// Provides a dumb implementation of System.Linq.Enumerable.Prepend from .NET 4.7+
        /// This will throw StackOverflowExceptions if used excessively lol
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> list, T item)
        {
            return new PrependEnumerable<T>(list, item);
        }

        public static bool IsText(this ChannelType type)
        {
            return type == ChannelType.Text || type == ChannelType.Private || type == ChannelType.Group || type == ChannelType.News;
        }
    }
}
