using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using BananaModManager.Shared;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BananaModManager.Loader.Mono
{
    public static class Loader
    {
        public static void Main()
        {
            var logFile = $"logs\\bmmlog_{DateTime.Now:yyyyMMdd_HHmmss_fff}.txt";

            try
            {
                var mods = Startup.StartModLoader();

                new Thread(() =>
                {
                    Thread.Sleep(4000);
                    Console.WriteLine("Initializing the mods...");

                    CreateCodeRunner(mods);
                }).Start();
                Console.WriteLine("Running the game now.");
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("[BMM Error] " + e);
                Console.BackgroundColor = ConsoleColor.Black;
                File.WriteAllText(logFile, e.ToString());
                MessageBox.Show(e.ToString(), "BananaModManager Error!", MessageBoxButtons.Ok, MessageBoxIcon.Error);
            }
        }

        private static void CreateCodeRunner(IEnumerable<Mod> mods)
        {
            var obj = new GameObject("BananaModManagerCodeRunner");
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
        }
    }
}