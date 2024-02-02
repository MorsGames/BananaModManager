using System.Collections.Generic;

namespace BananaModManager.Shared;

public static class Games
{
    public static Game Default = new();
    public static Game BananaBlitzHD = new()
    {
        GameID = "BananaBlitzHD",
        Title = "Super Monkey Ball: Banana Blitz HD",
        ExecutableName = "SMBBBHD",
        ExecutablePath = "SMBBBHD\\",
        SteamAppID = "1061730",
        Managed = true,
        X64 = false
    };
    public static Game BananaMania = new()
    {
        GameID = "BananaMania",
        Title = "Super Monkey Ball: Banana Mania",
        ExecutableName = "smbbm",
        ExecutablePath = "smbbm\\",
        SteamAppID = "1316910",
        Managed = false,
        X64 = true,
        SpeedrunModeSupport = true,
        FastRestartSupport = true,
        SaveModeSupport = true,
        DiscordRPCSupport = true,
        LegacyModeSupport = true,

        Whitelist = new List<string>() {
            //Graphical Tweaks
            "D79D69527DF63DDF35389DD05464A655BCE156CA484026E3249F7576250A6A14",
            // No Prompt
            "B678793A497954A4E05A76352781A07EC209087946506308E1E7722D5F2BFBAD",
            // Dynamic Roll
            "5B0BA8297E40FCF0ACFA47A6E2B12BB65DA96BD96202AF4C242715F31EEE2BDC",
            // Dynamic Roll Multiplayer
            "AB7B3FBEDB0A6FE51C63DE3ACDD94C18218A780B07B256C107A85B67A7ACB0BF",
            // Guest Characters
            "27F4B47A9F37B36BAC14FC4B10C7CBA399D7571E7879CADE9C961697CAE6F8FB",
            // Guest Characters Multiplayer
            "9E9B316CC6D5F349011694CCF0A1172E1621BF6D3B30D7562B0EB179A0661563",
            // Timer Sound
            "F62DB98D3F1F77717ACE38D706EA431663780EBF5BBB8A751010B92F24AA889F",
            // UI Enhancements
            "5768E7F0C37001E417084B3DD1775262B7A1AB43D1CF57B05AFFCFAA83A499A2",
            // IL Battle Timer
            "6E63EA9F81735FF6B87C8A3E093D6569045890B9B4ADC90D1EF03B3DE08F0B60",
            // True Ball Customizer
            "FCD4933822018E5C1D5CD0B0825A146EDD21FD4A83929BCEC71BDB421BCF1085",
            // Bingo UI
            "74925F51E9D3EF58026599FD524872F6F684125F325CE6E5026B4F346B9F7EEB",
            // BMOnline
            "797DABD2A15D95BB390729400D2DB384353A24D5787A2F1935BFB427EF3C8E9A",
            // Online IL Battle Timer
            "5FAE77E29ACA4A047282B21AD7F40FD1A325E1CC2047A04C1E5B6E04B1A6AEC9"
        },
        WhitelistNames = new List<string>
        {
            "GraphicalTweaks.dll",
            "NoPrompt.dll",
            "DynamicSounds.dll",
            "DynamicSoundsPlusMultiplayer.dll",
            "GuestCharacters.dll",
            "GuestCharactersMultiplayer.dll",
            "TimerSound.dll",
            "UI Enhancements.dll",
            "IL Battle Timer.dll",
            "TrueBallCustomizer.dll",
            "BingoUI.dll",
            "OnlineILBattleTimer.dll",
            "BMOnline.Mod.dll"
        }
    };
    public static Game Paperball = new()
    {
        GameID = "Paperball",
        Title = "Paperball",
        ExecutableName = "Paperball",
        ExecutablePath = "Paperball\\",
        SteamAppID = "1198510",
        Managed = true,
        X64 = true
    };

    public static List<Game> List = new() {BananaBlitzHD, BananaMania, Paperball};
}