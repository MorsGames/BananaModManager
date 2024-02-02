using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace BananaModManager.Shared;

/// <summary>
///     An instance of a mod.
/// </summary>
public class Mod
{
    /// <summary>
    ///     Modified config values of the mod.
    /// </summary>
    public Dictionary<string, ConfigItem> Config { get; set; }

    /// <summary>
    ///     Default config values of the mod.
    /// </summary>
    public Dictionary<string, ConfigItem> DefaultConfig { get; set; }

    /// <summary>
    ///     Where the mod actually is.
    /// </summary>
    public DirectoryInfo Directory { get; set; }

    /// <summary>
    ///     Whether if the mod is enabled or not.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    ///     Currently unused.
    /// </summary>
    public bool HasUpdates => !string.IsNullOrEmpty(Info.UpdateServer);

    /// <summary>
    ///     All the basic information about the mod.
    /// </summary>
    public ModInfo Info { get; set; }

    /// <summary>
    /// A list of types that should be registered in the mod. Only used in IL2Cpp mods.
    /// </summary>
    public List<Type> Types { get; set; }

    /// <summary>
    ///     Returns the internal ID string of the mod.
    /// </summary>
    /// <returns>ID string of the mod.</returns>
    public override string ToString()
    {
        return Info.Id;
    }

    /// <summary>
    ///     Returns the full path for the mod DLL.
    /// </summary>
    /// <returns>The full path for the mod DLL.</returns>
    public string GetFullPath()
    {
        return Path.Combine(Directory.FullName, Info.DLLFile);
    }

    /// <summary>
    ///     Loads the assembly of the mod DLL.
    /// </summary>
    /// <returns>Assembly of the mod DLL.</returns>
    public Assembly GetAssembly()
    {
        return Info.DLLFile == "" ? null : Assembly.LoadFrom(GetFullPath());
    }
}