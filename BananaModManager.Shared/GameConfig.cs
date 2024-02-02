using System.Collections.Generic;

namespace BananaModManager.Shared;

/// <summary>
///     User managed config values of the mod manager.
/// </summary>
public class GameConfig
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

    /// <summary>
    ///     Enables fast restarting and toggling Speedrun Mode by pressing F12, a quicker way to toggle Speedrun Mode for speedrunners.
    /// </summary>
    public bool FastRestart { get; set; } = true;

    /// <summary>
    ///     Enables/Disables saving the game (For prevention of overwriting times).
    /// </summary>
    public bool SaveMode { get; set; } = true;

    /// <summary>
    ///     Enables Discord Rich Presence support
    /// </summary>
    public bool DiscordRPC { get; set; } = false;

    /// <summary>
    ///     Enables Speedrun Mode and Save Mode to run on version 1.0.0
    /// </summary>
    public bool LegacyMode { get; set; } = false;
}