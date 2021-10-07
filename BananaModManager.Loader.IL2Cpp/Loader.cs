using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using BananaModManager.Shared;
using Il2CppSystem;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;
using UnityEngine;
using Console = System.Console;
using ConsoleColor = System.ConsoleColor;
using Exception = System.Exception;
using Math = System.Math;
using Object = UnityEngine.Object;
using Version = System.Version;

namespace BananaModManager.Loader.IL2Cpp
{
    public static class Loader
    {
        public static List<Mod> Mods { get; private set; }
        public static List<MethodInfo> UpdateMethods { get; set; } = new List<MethodInfo>();
        public static List<MethodInfo> FixedUpdateMethods { get; set; } = new List<MethodInfo>();
        public static List<MethodInfo> LateUpdateMethods { get; set; } = new List<MethodInfo>();
        public static List<MethodInfo> GUIMethods { get; set; } = new List<MethodInfo>();

        public static void Main()
        {
            try
            {
                Mods = Startup.StartModLoader();

                Console.WriteLine("Setting up the hooks...");

                LogSupport.ErrorHandler += delegate(string str)
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("[Hooking Error] " + str);
                    Console.BackgroundColor = ConsoleColor.Black;
                };
                LogSupport.InfoHandler += Console.WriteLine;
                LogSupport.TraceHandler += Console.WriteLine;
                LogSupport.WarningHandler += delegate(string str)
                {
                    Console.BackgroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("[Hooking Warning] " + str);
                    Console.BackgroundColor = ConsoleColor.Black;
                };

                var version = Version.Parse(Process.GetCurrentProcess().MainModule.FileVersionInfo.FileVersion);

                var revision = Math.Abs(version.Revision);
                if (revision != 0)
                {
                    var numberOfDigits = (int) Math.Floor(Math.Log10(revision) + 1);

                    if (numberOfDigits >= 2)
                        revision = (int) Math.Truncate(revision / Math.Pow(10, numberOfDigits - 2));
                }
                Console.WriteLine("Unity version: " + version.Major + "." + version.Minor + "." + revision);
                UnityVersionHandler.Initialize(version.Major, version.Minor, revision);

                ClassInjector.Detour = new UnhollowerDetour();

                Console.WriteLine("All done!");

                new Thread(() =>
                {
                    try
                    {
                        Thread.Sleep(4000);
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

            ClassInjector.RegisterTypeInIl2Cpp<CodeRunner>();

            var runner = new CodeRunner(obj.AddComponent(Il2CppType.Of<CodeRunner>()).Pointer);

            foreach (var mod in Mods)
            {
                if (mod.Types != null)
                {
                    foreach (var usedType in mod.Types)
                    {
                        ClassInjector.RegisterTypeInIl2Cpp(usedType);
                    }
                }

                foreach (var type in mod.GetAssembly().GetTypes())
                {
                    if (type.Name != "Main")
                        continue;

                    type.GetMethod("OnModStart")?.Invoke(null, null);

                    var update = type.GetMethod("OnModUpdate");
                    if (update != null)
                        UpdateMethods.Add(update);

                    var fixedUpdate = type.GetMethod("OnModFixedUpdate");
                    if (fixedUpdate != null)
                        FixedUpdateMethods.Add(fixedUpdate);

                    var lateUpdate = type.GetMethod("OnModLateUpdate");
                    if (lateUpdate != null)
                        LateUpdateMethods.Add(lateUpdate);

                    var gui = type.GetMethod("OnModGUI");
                    if (gui != null)
                        GUIMethods.Add(gui);
                }
            }
        }

        public static void InvokeUpdate()
        {
            foreach (var method in UpdateMethods) method.Invoke(null, null);
        }

        public static void InvokeFixedUpdate()
        {
            foreach (var method in FixedUpdateMethods) method.Invoke(null, null);
        }

        public static void InvokeLateUpdate()
        {
            foreach (var method in LateUpdateMethods) method.Invoke(null, null);
        }

        public static void InvokeGUI()
        {
            foreach (var method in GUIMethods) method.Invoke(null, null);
        }
    }
}