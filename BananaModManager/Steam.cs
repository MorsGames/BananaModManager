using System.IO;
using Microsoft.Win32;

namespace BananaModManager
{
    public static class Steam
    {
        public static string Location { get; set; }

        /// <summary>
        ///     Initializes Steam stuff.
        /// </summary>
        public static void Init()
        {
            // Gets Steam's registry key
            var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Valve\\Steam")
                      ?? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                          .OpenSubKey("SOFTWARE\\Wow6432Node\\Valve\\Steam");

            // Sets the location if the key exists
            if (key != null && key.GetValue("InstallPath") is string steamPath)
                Location = steamPath;
        }

        /// <summary>
        ///     Checks if the game at that certain Steam path exists.
        /// </summary>
        /// <param name="path">The relative path.</param>
        /// <returns></returns>
        public static bool CheckGame(string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(directory))
                return false;
            return File.Exists(path) && !(
                File.Exists(Path.Combine(directory, "steamclient64.dll")) ||
                File.Exists(Path.Combine(directory, "steamclient.dll")));
        }
    }
}