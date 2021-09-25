using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordLib
{
    public class PermissionOverwrite : Snowflake<PermissionOverwrite>
    {
        /// <summary>
        /// Gets the type of the overwrite. Either "role" or "member".
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public OverwriteType Type { get; internal set; }

        /// <summary>
        /// Gets the allowed permission set.
        /// </summary>
        [JsonProperty("allow", NullValueHandling = NullValueHandling.Ignore)]
        public Permissions Allowed { get; internal set; }

        /// <summary>
        /// Gets the denied permission set.
        /// </summary>
        [JsonProperty("deny", NullValueHandling = NullValueHandling.Ignore)]
        public Permissions Denied { get; internal set; }

        /// <summary>
        /// Checks whether given permissions are allowed, denied, or not set.
        /// </summary>
        /// <param name="permission">Permissions to check.</param>
        /// <returns>Whether given permissions are allowed, denied, or not set.</returns>
        public PermissionLevel CheckPermission(Permissions permission)
        {
            if ((Allowed & permission) != 0)
                return PermissionLevel.Allowed;
            if ((Denied & permission) != 0)
                return PermissionLevel.Denied;
            return PermissionLevel.Unset;
        }
    }

    /// <summary>
    /// Represents a channelpermissions overwrite's type.
    /// </summary>
    public enum OverwriteType : int
    {
        /// <summary>
        /// Specifies that this overwrite applies to a role.
        /// </summary>
        Role,

        /// <summary>
        /// Specifies that this overwrite applies to a member.
        /// </summary>
        Member
    }
}
