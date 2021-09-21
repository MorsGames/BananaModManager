using System.Collections.Generic;

namespace BananaModManager
{
    public static class Games
    {
        public static Game BananaBlitzHD = new Game
        {
            Title = "Super Monkey Ball: Banana Blitz HD",
            ExecutableName = "SMBBBHD.exe",
            ExecutablePath = "SMBBBHD\\",
            AppID = "1061730"
        };

        public static List<Game> List = new List<Game> {BananaBlitzHD};
    }
}