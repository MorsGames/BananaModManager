using System.Collections.Generic;

namespace BananaModManager
{
    public static class Games
    {
        public static Game BananaBlitzHD = new()
        {
            Title = "Super Monkey Ball: Banana Blitz HD",
            ExecutableName = "SMBBBHD",
            ExecutablePath = "SMBBBHD\\",
            AppID = "1061730",
            Managed = true
        };
        public static Game BananaMania = new()
        {
            Title = "Super Monkey Ball: Banana Mania",
            ExecutableName = "smbbm",
            ExecutablePath = "smbbm\\",
            AppID = "1316910",
            Managed = false,
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
                "4A9B5B06FEFF81B2C3E7D0E287D7AF285357F8A12EC1DAA9F266A8C84F7B2BAD",
                // IL Timer
                "F8C8CC2FA8130722AC0E03AFA506EF94BDD4BF40F6C28DC0218FCF1439835ACD",
                // True Ball Customizer
                "FCD4933822018E5C1D5CD0B0825A146EDD21FD4A83929BCEC71BDB421BCF1085",
                // Bingo UI
                "BB6403BEED413CC04E2EC600F283C5AD2604B7FDC843E6738EF3D1B3811BA069",
                // BMOnline
                "C10831F1C77AAEE71DCC85DD49F7CC9B9209BA4C1249441E6976B7A3C455C066"
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
                "ILBattleTimer.dll",
                "TrueBallCustomizer.dll",
                "BingoUI.dll",
                "BMOnline.Client.dll",
                "BMOnline.Common.dll",
                "BMOnline.Mod.dll"
            }
        };

        public static List<Game> List = new List<Game> {BananaBlitzHD, BananaMania};
    }
}