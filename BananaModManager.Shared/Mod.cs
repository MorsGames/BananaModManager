﻿using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace BananaModManager.Shared
{
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
        ///     Returns the internal ID string of the mod.
        /// </summary>
        /// <returns>ID string of the mod.</returns>
        public override string ToString()
        {
            return Info.Id;
        }

        /// <summary>
        ///     Loads the assembly of the mod DLL.
        /// </summary>
        /// <returns>Assembly of the mod DLL.</returns>
        public Assembly GetAssembly()
        {
            return Assembly.LoadFile(Path.Combine(Directory.FullName, Info.DLLFile));
        }
    }
}