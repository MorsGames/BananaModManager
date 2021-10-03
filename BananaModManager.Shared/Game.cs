﻿using System.Diagnostics;

namespace BananaModManager
{
    /// <summary>
    ///     A game that's supported by this mod manager
    /// </summary>
    public class Game
    {
        /// <summary>
        ///     Steam app ID of the game. Used to launch the game.
        /// </summary>
        public string AppID = "0";

        /// <summary>
        ///     The executable name of the mod.
        ///     Currently not used, Steam is used to launch the game instead.
        /// </summary>
        public string ExecutableName = "";

        /// <summary>
        ///     Location of the executable.
        /// </summary>
        public string ExecutablePath = "";

        /// <summary>
        ///     Title of the game.
        /// </summary>
        public string Title = "Unnamed Game";

        /// <summary>
        ///     Whether if the game uses Mono or IL2CPP.
        /// </summary>
        public bool Managed = false;

        public override string ToString()
        {
            return Title;
        }

        public void Run()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "steam://rungameid/" + AppID,
                UseShellExecute = true
            });
        }
    }
}