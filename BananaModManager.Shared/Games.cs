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
                // Guest Characters
                "27F4B47A9F37B36BAC14FC4B10C7CBA399D7571E7879CADE9C961697CAE6F8FB",
                // Timer Sound
                "F62DB98D3F1F77717ACE38D706EA431663780EBF5BBB8A751010B92F24AA889F",
                // UI
                "62B4D876FF957A5E681E8820B0CAD61126A86B5890DB6C6FB23CF32BCFCFD517",
                // IL timer
                "66D05F13336A3354A5A34C41598E85FC8C02DBA7ADEE51505EDD3D39BE8096B3",
                // True Ball Customizer
                "FCD4933822018E5C1D5CD0B0825A146EDD21FD4A83929BCEC71BDB421BCF1085",
                // BMOnline
                "B3810E36B069F0BD53EDB4963B6D5B987DF200DE205488A6453C4DC7303A88DB",
                "CAD9AC07692C06DDA7929415DC5DA4C8EB736EAFE7E1233881C178B9E8F9A303",
                "A4DC6B1DDFD54C642D3AB18E8A1CF928666166333ED830B48A26223DB2E57B93"
            },
            WhitelistNames = new List<string>
            {
                "GraphicalTweaks.dll",
                "NoPrompt.dll",
                "DynamicSounds.dll",
                "GuestCharacters.dll",
                "TimerSound.dll",
                "UI Enhancements.dll",
                "ILBattleTimer.dll",
                "TrueBallCustomizer.dll",
                "BMOnline.Client.dll",
                "BMOnline.Common.dll",
                "BMOnline.Mod.dll"
            }
        };

        public static List<Game> List = new List<Game> {BananaBlitzHD, BananaMania};
    }
}