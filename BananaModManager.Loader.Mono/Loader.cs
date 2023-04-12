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
        private static List<Mod> _mods;
        public static void Main()
        {
            try
            {
                Startup.StartModLoader(out _mods, out var _, out bool _, out bool _);

                new Thread(() =>
                {
                    try
                    {
                        Thread.Sleep(8000);
                        Console.WriteLine("Initializing the mods...");

                        CreateCodeRunner();
                    }
                    catch (Exception e)
                    {
                        Startup.ExceptionHandler(e);
                    }
                }).Start();
                Console.WriteLine("Running the game now.");
            }
            catch (Exception e)
            {
                Startup.ExceptionHandler(e);
            }
        }

        private static void CreateCodeRunner()
        {
            var obj = new GameObject("BananaModManagerCodeRunner");
            var runner = obj.AddComponent<CodeRunner>();
            Object.DontDestroyOnLoad(obj);

            foreach (var type in _mods.Where(mod => mod.GetAssembly() != null).SelectMany(mod => mod.GetAssembly().GetTypes()))
            {
                type.GetMethod("OnModStart")?.Invoke(null, null);

                var update = type.GetMethod("OnModUpdate");
                if (update != null) runner.UpdateMethods.Add(update);

                var fixedUpdate = type.GetMethod("OnModFixedUpdate");
                if (fixedUpdate != null) runner.FixedUpdateMethods.Add(fixedUpdate);

                var lateUpdate = type.GetMethod("OnModLateUpdate");
                if (lateUpdate != null) runner.LateUpdateMethods.Add(lateUpdate);

                var gui = type.GetMethod("OnModGUI");
                if (gui != null) runner.GUIMethods.Add(gui);
            }
        }
    }
}