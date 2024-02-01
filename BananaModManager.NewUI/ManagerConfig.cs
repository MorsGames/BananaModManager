namespace BananaModManager.NewUI;

public class ManagerConfig
{
        /// <summary>
        /// The directory of the game
        /// </summary>
        public string GameDirectory { get; set; } = "";

        ///<summary>
        ///     Enables One-Click support on GameBanana. Adding "bananamodmanager:" before any valid mod URL will also prompt the One-Click installation.
        /// </summary>
        public bool OneClick { get; set; } = false;

        /// <summary>
        /// Activates the not so hidden dark mode
        /// </summary>
        public int Theme { get; set; } = 0;

        /// <summary>
        /// Modifies the layout to look a bit more like the original mod manager
        /// </summary>
        public bool LegacyLayout { get; set; } = false;
}
