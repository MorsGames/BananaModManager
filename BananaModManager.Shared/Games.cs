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
                "bm.mors.graphicaltweaks",
                "bm.ceejiggy.noprompt",
                "bm.snowman.dynamicroll",
                "bm.snowman.guestcharacter",
                "bm.viictiinii.timersound",
                "iswimfly.HUD",
                "iswimfly.ILBattle
            }
        };

        public static List<Game> List = new List<Game> {BananaBlitzHD, BananaMania};
    }
}
