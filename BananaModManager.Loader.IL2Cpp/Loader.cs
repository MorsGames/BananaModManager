using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using BananaModManager.Shared;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Runtime;
using UnhollowerRuntimeLib;

namespace BananaModManager.Loader.IL2Cpp
{
    public static class Loader
    {
        public static List<Mod> Mods { get; private set; }

        public static void Main()
        {
            var logFile = $"logs\\bmmlog_{DateTime.Now:yyyyMMdd_HHmmss_fff}.txt";

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

                ClassInjector.RegisterTypeInIl2Cpp<CodeRunner>(true);

                new Thread(() =>
                {
                    Thread.Sleep(4000);
                    Console.WriteLine("Initializing the mods...");

                    CodeRunner.Create();
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
    }
}