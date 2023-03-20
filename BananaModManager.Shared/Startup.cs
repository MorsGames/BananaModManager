using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using BananaModManager.Loader;

namespace BananaModManager.Shared
{
    public static class Startup
    {
        private const int SW_SHOW = 5;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private static void ShowConsoleWindow()
        {
            var handle = GetConsoleWindow();

            if (handle == IntPtr.Zero)
                AllocConsole();
            else
                ShowWindow(handle, SW_SHOW);
        }

        public static void StartModLoader(out List<Mod> mods, out bool speedrunMode)
        {
            Mods.Load(out var activeMods, out var consoleWindow, out speedrunMode, out bool oneClick, out bool fastRestart);

            if (consoleWindow)
                ShowConsoleWindow();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("BananaModManager by Mors!");
            Console.WriteLine("Speedrun fixes created by iswimfly!");
            Console.ForegroundColor = ConsoleColor.White;

            // Get the current game
            Game currentGame = null;
            foreach (var game in Games.List)
            {
                if (game.ExecutableName == System.Diagnostics.Process.GetCurrentProcess().ProcessName)
                    currentGame = game;
            }

            Console.WriteLine("Detected " + currentGame.Title + " (" + (currentGame.Managed ? "Mono" : "ILCpp") + ")");

            Console.WriteLine("Found " + activeMods.Count + " active mods out of " + Mods.List.Count + ".");

            mods = new List<Mod>();

            foreach (var mod in activeMods.Select(modId => Mods.List[modId]))
            {
                if (speedrunMode)
                {
                    string Hash = "";
                    byte[] hashvalue;
                    using (SHA256 SHA256 = SHA256.Create())
                    {
                        using (FileStream fileStream = File.OpenRead(mod.Directory.FullName + "\\" + mod.Info.DLLFile))
                        {
                            fileStream.Position = 0;
                            hashvalue = SHA256.ComputeHash(fileStream);
                            for (int i = 0; i < hashvalue.Length; i++)
                            {
                                Hash += $"{hashvalue[i]:X2}";
                            }
                        }

                    }
                    if (!currentGame.Whitelist.Contains(Hash) && currentGame.WhitelistNames.Contains(mod.Info.DLLFile))
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Nice try, cheater! " + mod.Info.Title + "'s Hash is different. The file is modified!");
                        Console.BackgroundColor = ConsoleColor.Black;
                        continue;
                    }
                    if (!currentGame.WhitelistNames.Contains(mod.Info.DLLFile))
                    {
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("Skipped loading " + mod.Info.Title + ". It's not whitelisted for the speedrun mode!");
                        Console.BackgroundColor = ConsoleColor.Black;
                        continue;
                    }
                }

                mods.Add(mod);

                Console.WriteLine("Loading " + mod.Info.Title + " (" + mod + ")");

                // Load the config dictionary
                var config = Mods.LoadModConfig(mod.Info, Mods.LoadUserConfig(),
                    Mods.LoadDefaultModConfig(mod.Directory));

                // We need to convert it before we can pass it on
                var converted = Mods.ConvertConfig(config);

                // We add the directory as a config
                converted.Add("Directory", mod.Directory.FullName);

                // Time to invoke
                if (mod.GetAssembly() == null)
                    continue;

                foreach (var type in mod.GetAssembly().GetTypes())
                {
                    mod.Types = (List<Type>)type.GetMethod("OnModLoad")?.Invoke(null, new object[] { converted });
                }
            }
        }
        public static void ExceptionHandler(Exception e)
        {
            var logFile = $"logs\\bmmlog_{DateTime.Now:yyyyMMdd_HHmmss_fff}.txt";

            if (!Directory.Exists("logs"))
                Directory.CreateDirectory("logs");

            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("[BMM Error] " + e);
            Console.BackgroundColor = ConsoleColor.Black;

            File.WriteAllText(logFile, e.ToString());
            MessageBox.Show(e.ToString(), "BananaModManager Error!", MessageBoxButtons.Ok,
                MessageBoxIcon.Error);
        }
    }
}