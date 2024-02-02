using BananaModManager.Shared;

namespace BananaModManager;

public class ModsTableItem
{
    public bool Enabled { get; set; }
    public int? Order { get; set; }
    public string Name { get; }
    public string Version { get; }
    public string Author { get; }
    public Mod Mod { get; set; }

    public ModsTableItem(bool enabled, int? order, string name, string version, string author, Mod mod)
    {
        Enabled = enabled;
        Order = order;
        Name = name;
        Version = version;
        Author = author;
        Mod = mod;
    }
}
