namespace Database
{
    /// <summary>
    /// Canonical identity mapping policy shared across Telegram and Twitch modes.
    /// Storage columns retain historical names for backward compatibility.
    /// </summary>
    public static class IdentityMappingPolicy
    {
        /// <summary>
        /// Player.TelegramId stores the platform user identifier (Telegram user ID or Twitch user_id).
        /// </summary>
        public const string PlayerTelegramId = "Player.TelegramId => Platform User ID";

        /// <summary>
        /// Group.GroupId stores the platform group/channel identifier (Telegram chat ID or Twitch broadcaster/channel numeric ID).
        /// </summary>
        public const string GroupGroupId = "Group.GroupId => Platform Group/Channel ID";

        /// <summary>
        /// NotifyGame.UserId stores the same platform user identifier used by Player.TelegramId.
        /// </summary>
        public const string NotifyGameUserId = "NotifyGame.UserId => Platform User ID";

        /// <summary>
        /// NotifyGame.GroupId stores the same platform group/channel identifier used by Group.GroupId.
        /// </summary>
        public const string NotifyGameGroupId = "NotifyGame.GroupId => Platform Group/Channel ID";
    }
}
