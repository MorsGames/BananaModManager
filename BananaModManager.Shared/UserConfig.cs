using System.Collections.Generic;

namespace BananaModManager.Shared
{
    /// <summary>
    ///     User managed config values of the mod manager.
    /// </summary>
    public class UserConfig
    {
        /// <summary>
        ///     List of all currently enabled mods, in a specific order.
        /// </summary>
        public List<string> ActiveMods { get; set; } = new List<string>();

        /// <summary>
        ///     Config values of all mods.
        /// </summary>
        public Dictionary<string, Dictionary<string, object>> ModConfigs { get; set; } =
            new Dictionary<string, Dictionary<string, object>>();

        /// <summary>
        ///     Displays the console window.
        /// </summary>
        public bool ConsoleWindow { get; set; } = true;

        /// <summary>
        ///     Enables the leaderboards, a whitelist for the approved mods, and displays all the active mods on screen.
        /// </summary>
        public bool SpeedrunMode { get; set; } = true;

    }
}