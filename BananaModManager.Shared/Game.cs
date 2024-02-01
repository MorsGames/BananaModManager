using System.Collections.Generic;
using System.Diagnostics;

namespace BananaModManager.Shared
{
    /// <summary>
    ///     A game that's supported by this mod manager
    /// </summary>
    public class Game
    {
        // <summary>
        //     ID used to identify the game.
        // </summary>
        public string GameID = "Unknown";

        /// <summary>
        ///     Steam app ID of the game. Used to launch the game.
        /// </summary>
        public string SteamAppID = "0";

        /// <summary>
        ///     The executable name of the mod.
        ///     Used to detect which game is the mod is configured for.
        /// </summary>
        public string ExecutableName = "";

        /// <summary>
        ///     Location of the executable.
        ///     Not used but it's there!
        /// </summary>
        public string ExecutablePath = "";

        /// <summary>
        ///     Title of the game.
        /// </summary>
        public string Title = "No Game";

        /// <summary>
        ///     Whether if the game uses Mono or IL2CPP.
        /// </summary>
        public bool Managed = false;

        /// <summary>
        ///     If the game is 64-bit.
        /// </summary>
        public bool X64 = false;

        /// <summary>
        ///     If the game supports speedrun mode.
        /// </summary>
        public bool SpeedrunModeSupport = false;

        /// <summary>
        ///     If the game supports the F11 restart thingy.
        /// </summary>
        public bool FastRestartSupport = false;

        /// <summary>
        ///     If the game supports automatic saving of the clear times.
        /// </summary>
        public bool SaveModeSupport = false;

        /// <summary>
        ///     If the game supports Discord RPC.
        /// </summary>
        public bool DiscordRPCSupport = false;

        /// <summary>
        ///     If the game has any need for a legacy mode setting.
        /// </summary>
        public bool LegacyModeSupport = false;


        //TODO: Shouldn't the following two be a single list?

        /// <summary>
        ///     Hashes of the alowed mods in speedrun mode.
        /// </summary>
        public List<string> Whitelist = new List<string>();

        /// <summary>
        ///     Names of the allowed mods in speedrun mode.
        /// </summary>
        ///
        public List<string> WhitelistNames = new List<string>();

        public override string ToString()
        {
            return Title;
        }

        public void Run()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "steam://rungameid/" + SteamAppID,
                UseShellExecute = true
            });
        }
    }
}
