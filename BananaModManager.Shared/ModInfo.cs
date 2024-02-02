using System.Collections.Generic;

namespace BananaModManager.Shared;

/// <summary>
///     All the basic information about a mod.
/// </summary>
public class ModInfo
{
    /// <summary>
    ///     Creator of the mod.
    /// </summary>
    public string Author { get; set; } = "";

    /// <summary>
    ///     The creator's website URL.
    /// </summary>
    public string AuthorURL { get; set; } = "";

    /// <summary>
    ///     Creation date of the mod.
    /// </summary>
    public string Date { get; set; } = "";

    /// <summary>
    ///     Description of the mod.
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    ///     Name of the DLL file that will be loaded.
    /// </summary>
    public string DLLFile { get; set; } = "";

    /// <summary>
    ///     Asset bundles that should be patched in.
    /// </summary>
    public List<string> AssetBundles { get; set; } = new();

    /// <summary>
    ///     Internal name of the mod in the "game.author.name" format. This name format is not strictly enforced.
    /// </summary>
    public string Id { get; set; } = "game.author.name";

    /// <summary>
    ///     Name of the mod.
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    ///     Currently unused.
    /// </summary>
    public string UpdateServer { get; set; }

    /// <summary>
    ///     Version of the mod that's used updates. No scheme is enforced, but "MAJOR.MINOR.PATCH" is encouraged.
    /// </summary>
    public string Version { get; set; } = "";


    /// <summary>
    ///     The priority in which the mod should be loaded. Defaults to 5. 0 is the earliest, 5 is the latest.
    /// </summary>
    public string Priority { get; set; } = "5";

    /// <summary>
    ///     Returns the internal ID string of the mod.
    /// </summary>
    /// <returns>ID string of the mod.</returns>
    public override string ToString()
    {
        return Id;
    }
}