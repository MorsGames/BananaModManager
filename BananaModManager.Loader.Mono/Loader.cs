using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using BananaModManager.Shared;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BananaModManager.Loader.Mono;

public static class Loader
{
    private static List<Mod> _mods;
    public static void Main()
    {
        try
        {
            Startup.StartModLoader(out _mods, out var gameConfig, out var currentGame);

            new Thread(() =>
            {
                try
                {
                    Thread.Sleep(12000);
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
        Object.DontDestroyOnLoad(obj);

        var runner = obj.AddComponent<CodeRunner>();

        var priorityCheck = 0;
        while (priorityCheck < 6)
        {
            foreach (var mod in _mods)
            {
                // If not the correct priority
                if (priorityCheck != Convert.ToInt32(mod.Info.Priority))
                    continue;

                // Check each class if there's an assembly
                var assembly = mod.GetAssembly();

                if (assembly == null)
                    continue;

                // Go through each class
                foreach (var type in assembly.GetTypes())
                {
                    // Only look for one that's called "Main"
                    if (type.Name != "Main")
                        continue;

                    // Add it to the code runner
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
            priorityCheck++;
        }
    }
}
