namespace BananaModManager.Shared
{
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
        public string DLLFile { get; set; } = "mod.dll";

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
        ///     Returns the internal ID string of the mod.
        /// </summary>
        /// <returns>ID string of the mod.</returns>
        public override string ToString()
        {
            return Id;
        }
    }
}