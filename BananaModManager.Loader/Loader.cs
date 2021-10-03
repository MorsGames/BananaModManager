using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using BananaModManager.Shared;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BananaModManager.Loader
{
    public static class Loader
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

        public static void Main()
        {
            try
            {
                ShowConsoleWindow();

                Console.WriteLine("BananaModManager by Mors!");

                // Get the current game
                Game currentGame = null;
                foreach (var game in Games.List)
                {
                    if (game.ExecutableName == System.Diagnostics.Process.GetCurrentProcess().ProcessName)
                        currentGame = game;
                }

                Console.WriteLine("Detected " + currentGame.Title);

                Mods.Load(out var activeMods);

                Console.WriteLine("Found " + activeMods.Count + " active mods out of " + Mods.List.Count + ".");

                var mods = new List<Mod>();

                foreach (var mod in activeMods.Select(modId => Mods.List[modId]))
                {
                    mods.Add(mod);

                    Console.WriteLine("Loading " + mod.Info.Title + " (" + mod + ")");

                    foreach (var type in mod.GetAssembly().GetTypes())
                    {
                        // Load the config dictionary
                        var config = Mods.LoadModConfig(mod.Info, Mods.LoadUserConfig(),
                            Mods.LoadDefaultModConfig(mod.Directory));

                        // We need to convert it before we can pass it on
                        var converted = Mods.ConvertConfig(config);

                        // We add the directory as a config.
                        converted.Add("Directory", mod.Directory.FullName);

                        // Time to invoke
                        type.GetMethod("OnModLoad")?.Invoke(null, new object[] {converted});

                        // Time to replace the existing assets
                        // TODO: Asset bundles could be made use of maybe?
                    }
                }

                // TODO: Referring to Unity stuff here makes Banana Mania crash?
                new Thread(() =>
                {
                    Thread.Sleep(4000);
                    Console.WriteLine("Initializing the mods...");

                    // Create the CodeRunner GameObject
                    var obj = new GameObject();
                    var runner = obj.AddComponent<CodeRunner>();
                    Object.DontDestroyOnLoad(obj);

                    foreach (var type in mods.SelectMany(mod => mod.GetAssembly().GetTypes()))
                    {
                        type.GetMethod("OnModStart")?.Invoke(null, null);

                        var update = type.GetMethod("OnModUpdate");
                        if (update != null)
                            runner.UpdateMethods.Add(update);

                        var fixedUpdate = type.GetMethod("OnModFixedUpdate");
                        if (fixedUpdate != null)
                            runner.FixedUpdateMethods.Add(fixedUpdate);

                        var lateUpdate = type.GetMethod("OnModLateUpdate");
                        if (lateUpdate != null)
                            runner.LateUpdateMethods.Add(lateUpdate);

                        var gui = type.GetMethod("OnModGUI");
                        if (gui != null)
                            runner.GUIMethods.Add(gui);
                    }
                }).Start();
                Console.WriteLine("Running the game now.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                MessageBox.Show(e.ToString(), "Mod Error!", MessageBoxButtons.Ok, MessageBoxIcon.Error);
            }
        }
    }
}