using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using BananaModManager.Shared;
using Flash2;
using HarmonyLib;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;
using Console = System.Console;
using ConsoleColor = System.ConsoleColor;
using Delegate = System.Delegate;
using Exception = System.Exception;
using IntPtr = System.IntPtr;
using Math = System.Math;
using Object = UnityEngine.Object;
using Version = System.Version;

namespace BananaModManager.Loader.IL2Cpp
{
    public static class Loader
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern System.IntPtr GetModuleHandle(string lpModuleName);

        private static List<Mod> _mods;
        private static bool _speedrunMode;

        private static List<Tuple<string, string>> _hashList = new List<Tuple<string, string>>();
        private static float _modListSlide = 1f;

        public static List<Mod> Mods => _mods;
        public static Dictionary<string, string> AssetBundles { get; private set; } = new Dictionary<string, string>();
        public static bool SpeedrunMode => _speedrunMode;

        public static List<MethodInfo> UpdateMethods { get; set; } = new List<MethodInfo>();
        public static List<MethodInfo> FixedUpdateMethods { get; set; } = new List<MethodInfo>();
        public static List<MethodInfo> LateUpdateMethods { get; set; } = new List<MethodInfo>();
        public static List<MethodInfo> GUIMethods { get; set; } = new List<MethodInfo>();

        public static void Main()
        {
            try
            {
                Startup.StartModLoader(out _mods, out _speedrunMode);

                Console.WriteLine("Setting up the hooks...");

                LogSupport.ErrorHandler += delegate (string str)
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("[Hooking Error] " + str);
                    Console.BackgroundColor = ConsoleColor.Black;
                };
                LogSupport.InfoHandler += Console.WriteLine;
                LogSupport.TraceHandler += Console.WriteLine;
                LogSupport.WarningHandler += delegate (string str)
                {
                    Console.BackgroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("[Hooking Warning] " + str);
                    Console.BackgroundColor = ConsoleColor.Black;
                };

                var version = Version.Parse(Process.GetCurrentProcess().MainModule.FileVersionInfo.FileVersion);

                var revision = Math.Abs(version.Revision);
                if (revision != 0)
                {
                    var numberOfDigits = (int)Math.Floor(Math.Log10(revision) + 1);

                    if (numberOfDigits >= 2)
                        revision = (int)Math.Truncate(revision / Math.Pow(10, numberOfDigits - 2));
                }
                Console.WriteLine("Unity version: " + version.Major + "." + version.Minor + "." + revision);
                UnityVersionHandler.Initialize(version.Major, version.Minor, revision);

                ClassInjector.Detour = new UnhollowerDetour();

                Console.WriteLine("All done!");

                new Thread(() =>
                {
                    try
                    {
                        Thread.Sleep(12000);

                        if (Mods.Count > 0)
                        {
                            LeaderboardsDelegateInstance = Dummy;

                            ClassInjector.Detour.Detour(IntPtr.Add(GetModuleHandle("GameAssembly.dll"), 0xa9d990),
                                LeaderboardsDelegateInstance);
                        }

                        Console.WriteLine("Initializing the mods...");
                        CreateCodeRunner();
                    }
                    catch (Exception e)
                    {
                        Startup.ExceptionHandler(e);
                    }

                }).Start();

                // Add the asset bundles into the dictionary
                /*foreach (var mod in Mods)
                {
                    foreach (var assetBundle in mod.Info.AssetBundles)
                    {
                        Console.WriteLine("Found asset bundle: " + assetBundle);
                        AssetBundles.Add(assetBundle, Path.Combine(mod.Directory.FullName, assetBundle));
                    }
                }

                // Patch them all
                Console.WriteLine("Patching the asset bundles.");
                var harmony = Harmony.CreateAndPatchAll(typeof(AssetBundleCachePatch));

                foreach (var patchedMethod in harmony.GetPatchedMethods())
                {
                    Console.WriteLine("Patched method: " + patchedMethod.Name);
                }*/

                // Calculate the hashes to display in the speedrun mode
                if (_speedrunMode)
                {
                    Console.WriteLine("Calculating the hashes.");

                    foreach (var t in Mods)
                    {
                        var name = t.Info.Title;
                        var hash = Hash(t.Info.Id + new FileInfo(t.GetFullPath()).Length);

                        _hashList.Add(new Tuple<string, string>(name, hash));
                    }
                }

                Console.WriteLine("Running the game now.");
            }
            catch (Exception e)
            {
                Startup.ExceptionHandler(e);
            }
        }

        public delegate void LeaderboardsDelegate();
        public static LeaderboardsDelegate LeaderboardsDelegateInstance;

        public static void Dummy()
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Leaderboards are disabled when mods are used.");
            Console.BackgroundColor = ConsoleColor.Black;
        }

        private static void CreateCodeRunner()
        {
            var obj = new GameObject("BananaModManagerCodeRunner");
            Object.DontDestroyOnLoad(obj);

            ClassInjector.RegisterTypeInIl2Cpp<CodeRunner>();

            var runner = new CodeRunner(obj.AddComponent(Il2CppType.Of<CodeRunner>()).Pointer);

            // Go through each mod
            foreach (var mod in Mods)
            {
                // Register the types
                if (mod.Types != null)
                {
                    foreach (var usedType in mod.Types)
                    {
                        ClassInjector.RegisterTypeInIl2Cpp(usedType);
                    }
                }

                // Check each class if there's an assembly
                if (mod.GetAssembly() == null)
                    continue;

                foreach (var type in mod.GetAssembly().GetTypes())
                {
                    // Only look for one that's called "Main"
                    if (type.Name != "Main")
                        continue;

                    // Add it to the code runner
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

            if (!SpeedrunMode)
                return;

            try
            {
                _modListSlide = GameManager.IsPause() ? 1f : (Mathf.Lerp(_modListSlide, MainGameUI.Hud.isActive ? 0f : 1f, Time.deltaTime * 16f));
            }
            catch
            {
                _modListSlide = 1f;
            }

            // Only draw when not outside the screen
            if (!(_modListSlide < 1f))
                return;

            var style = new GUIStyle();
            var ratio = Screen.height / 1080f;

            // Set the initial offset and font size
            style.fontSize = Math.Max(16, (int)(24.0f * ratio));
            var offset = new Vector2(156f - _modListSlide * 384f, 160f) * ratio;
            var offset2 = 160f * ratio;

            // Draw the title
            DrawTextOutline(new Rect(Screen.width - offset.x - offset2 + _modListSlide, offset.y, Screen.width, style.fontSize),
                "Loaded Mods:", (int)Mathf.Ceil(ratio), style);

            // Change the font size and add extra offset for the rest
            style.fontSize = Math.Max(12, (int)(16.0f * ratio));
            offset.y = 192f * ratio;

            // Size of the outline
            var outlineSize = (int)Mathf.Ceil(ratio);

            // Draw them all
            for (var i = 0; i < _hashList.Count; i++)
            {
                DrawTextOutline(new Rect(Screen.width - offset.x - offset2 + _modListSlide, offset.y + style.fontSize * i, offset2 - offset.x, style.fontSize),
                    _hashList[i].Item1, outlineSize, style);
                DrawTextOutline(new Rect(Screen.width - offset.x + _modListSlide, offset.y + style.fontSize * i, Screen.width, style.fontSize),
                    _hashList[i].Item2, outlineSize, style);
            }
        }

        private static string Hash(string str)
        {
            var allowedSymbols = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
            const int length = 6;
            var hash = new char[length];

            for (var i = 0; i < str.Length; i++)
            {
                hash[i % length] = (char)(hash[i % length] ^ str[i]);
            }

            for (var i = 0; i < length; i++)
            {
                hash[i] = allowedSymbols[hash[i] % allowedSymbols.Length];
            }

            return new string(hash);
        }

        private static void DrawTextOutline(Rect r, string t, int strength, GUIStyle style)
        {
            if (strength > 0)
            {

                style.normal.textColor = new Color(0f, 0f, 0f, 1f);

                int i;
                for (i = -strength; i <= strength; i++)
                {
                    GUI.Label(new Rect(r.x - strength, r.y + i, r.width, r.height), t, style);
                    GUI.Label(new Rect(r.x + strength, r.y + i, r.width, r.height), t, style);
                }
                for (i = -strength + 1; i <= strength - 1; i++)
                {
                    GUI.Label(new Rect(r.x + i, r.y - strength, r.width, r.height), t, style);
                    GUI.Label(new Rect(r.x + i, r.y + strength, r.width, r.height), t, style);
                }
            }

            style.normal.textColor = Color.white;

            GUI.Label(r, t, style);
        }
    }
}