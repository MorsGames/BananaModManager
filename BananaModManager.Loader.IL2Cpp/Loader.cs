using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
using Framework.UI;
using UnityEngine.SceneManagement;
using Console = System.Console;
using ConsoleColor = System.ConsoleColor;
using Delegate = System.Delegate;
using Exception = System.Exception;
using IntPtr = System.IntPtr;
using Math = System.Math;
using Object = UnityEngine.Object;
using Version = System.Version;
using DiscordRPC;

namespace BananaModManager.Loader.IL2Cpp
{
    public static class Loader
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern System.IntPtr GetModuleHandle(string lpModuleName);

        private static List<Mod> _mods;
        private static bool _speedrunMode;
        private static bool _saveMode;
        private static bool _discordRPC = true;
        private static string ClientID = null;

        private static List<string> _SpeedrunList = new List<string>();
        private static float _modListSlide = 1f;

        private static UserConfig userConfig;
        private static Game currentGame;
        private static DiscordRpcClient client;

        public static List<Mod> Mods => _mods;
        public static Dictionary<string, string> AssetBundles { get; private set; } = new Dictionary<string, string>();
        public static bool SpeedrunMode => _speedrunMode;
        public static bool SaveMode => _saveMode;

        public static List<MethodInfo> UpdateMethods { get; set; } = new List<MethodInfo>();
        public static List<MethodInfo> FixedUpdateMethods { get; set; } = new List<MethodInfo>();
        public static List<MethodInfo> LateUpdateMethods { get; set; } = new List<MethodInfo>();
        public static List<MethodInfo> GUIMethods { get; set; } = new List<MethodInfo>();

        public static void Main()
        {
            try
            {
                Startup.StartModLoader(out _mods, out _speedrunMode, out _saveMode, out _discordRPC);

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

                Console.WriteLine("Carrying over user config and checking current game...");

                // Carries over the entire user config for use in other areas
                userConfig = Shared.Mods.LoadUserConfig();
                foreach (Game game in Games.List)
                {
                    if (game.ExecutableName == System.Diagnostics.Process.GetCurrentProcess().ProcessName)
                        currentGame = game;
                }
                Console.WriteLine("Config loaded! " + currentGame.Title + " detected!");
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

                // Discord RPC Setup
                try
                {
                    if (_discordRPC || ClientID != null)
                    {
                        switch (currentGame.AppID)
                        {
                            case "1061730":
                                ClientID = "1095161758357930164";
                                break;
                            case "1316910":
                                ClientID = "1094498140335378472";
                                break;
                        }
                        client = new DiscordRpcClient(ClientID, -1);
                        client.Initialize();
                        Console.WriteLine("Discord Rich Presence Started!");
                    }
                }
                catch (Exception e)
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(e.ToString());
                    Console.BackgroundColor = ConsoleColor.Black;
                }



                // Set default presence

                Console.WriteLine("All done!");

                new Thread(() =>
                {
                    try
                    {
                        Thread.Sleep(12000);

                        if (Mods.Count > 0 && !_speedrunMode)
                        {
                            LeaderboardsDelegateInstance = Dummy;
                            ClassInjector.Detour.Detour(IntPtr.Add(GetModuleHandle("GameAssembly.dll"), 0x296130), LeaderboardsDelegateInstance);
                        }
                        if (!_saveMode)
                        {
                            SaveDelegateInstance = Dummy2;
                            ClassInjector.Detour.Detour(IntPtr.Add(GetModuleHandle("GameAssembly.dll"), 0xE5B820), SaveDelegateInstance);
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
                    Console.WriteLine("Passing mod names to Speedrun Mode display...");

                    foreach (var t in Mods)
                    {
                        var name = t.Info.Title;
                        _SpeedrunList.Add(name);
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
        public delegate void SaveDelegate();
        public static LeaderboardsDelegate LeaderboardsDelegateInstance;
        public static SaveDelegate SaveDelegateInstance;

        public static void Dummy()
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Leaderboards are disabled when mods are used.");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        public static void Dummy2()
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Save Mode is disabled. Best clears will not save.");
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
            // If F11 is pressed, restart and toggle Speedrun Mode
            if (AppInput.GetKeyDown(KeyCode.F11) && userConfig.FastRestart == true)
            {
                Shared.Mods.Save(userConfig.ActiveMods, userConfig.ConsoleWindow, !userConfig.SpeedrunMode, userConfig.OneClick, userConfig.FastRestart, userConfig.SaveMode, userConfig.DiscordRPC);
                Process.Start(new ProcessStartInfo
                {
                    FileName = "steam://rungameid/" + currentGame.AppID,
                    UseShellExecute = true
                });
                Process.GetCurrentProcess().Kill();
            }
            if (_discordRPC)
            {
                    switch (ClientID)
                    {
                        case "1094498140335378472":
                            BananaManiaRPC();
                            break;
                        case "1095161758357930164":
                            BananaBlitzHDRPC();
                            break;
                    }
            }
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
            for (var i = 0; i < _SpeedrunList.Count; i++)
            {
                DrawTextOutline(new Rect(Screen.width - offset.x - offset2 + _modListSlide, offset.y + style.fontSize * i, offset2 - offset.x, style.fontSize),
                    _SpeedrunList[i], outlineSize, style);
            }
        }
        public static void OnDisable()
        {
            client.Dispose();
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
        public static void BananaManiaRPC()
        {
            try
            {
                // Set up to show who you're playing as
                Dictionary<Chara.eKind, string> characters = new Dictionary<Chara.eKind, string>();
                characters.Add(Chara.eKind.Aiai, "Aiai");
                characters.Add(Chara.eKind.Meemee, "Meemee");
                characters.Add(Chara.eKind.Baby, "Baby");
                characters.Add(Chara.eKind.Gongon, "Gongon");
                characters.Add(Chara.eKind.Yanyan, "Yanyan");
                characters.Add(Chara.eKind.Doctor, "Doctor");
                characters.Add(Chara.eKind.Jam, "Jam");
                characters.Add(Chara.eKind.Jet, "Jet");
                characters.Add(Chara.eKind.Sonic, "Sonic");
                characters.Add(Chara.eKind.Tails, "Tails");
                characters.Add(Chara.eKind.Kiryu, "Kiryu");
                characters.Add(Chara.eKind.Beat, "Beat");
                characters.Add(Chara.eKind.Dlc01, "Hello Kitty");
                characters.Add(Chara.eKind.Dlc02, "Morgana");
                characters.Add(Chara.eKind.Dlc03, "Suezo");
                characters.Add(Chara.eKind.GameGear, "the Game Gear");
                characters.Add(Chara.eKind.SegaSaturn, "the Sega Saturn");
                characters.Add(Chara.eKind.Dreamcast, "the Sega Dreamcast");
                var scene = SceneManager.GetActiveScene().name;
                switch (scene)
                {
                    case "Title":
                        client.SetPresence(new RichPresence()
                        {
                            Details = "At the Title Screen!"
                        });
                        break;
                    case "MainMenu":
                        client.SetPresence(new RichPresence()
                        {
                            Details = "Browsing the Main Menu!"
                        });
                        break;
                    default:
                        if (SceneManager.GetSceneByName("PgRace").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgRaceSequence>() == null) return;
                            client.SetPresence(new RichPresence()
                            {
                                Details = "Currently playing Monkey Race!"
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("PgFight").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgFightSequence>() == null) return;
                            int fightCount = GameObject.FindObjectOfType<PgFightSequence>().m_mainCnt;
                            client.SetPresence(new RichPresence()
                            {
                                Details = "Currently playing Monkey Fight!",
                                State = $"Fighting in Round {fightCount + 1}"
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("PgTarget").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgTargetSequence>() == null) return;
                            int roundCount = GameObject.FindObjectOfType<PgTargetSequence>()._currentRoundIndex_k__BackingField;
                            client.SetPresence(new RichPresence()
                            {
                                Details = "Currently playing Monkey Target!",
                                State = $"Flying through Round {roundCount + 1}"
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("PgBilliards").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgBilliardsSequence>() == null) return;
                            client.SetPresence(new RichPresence()
                            {
                                Details = "Currently playing Monkey Billiards!"
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("PgBowling").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgBowlingSequence>() == null) return;
                            client.SetPresence(new RichPresence()
                            {
                                Details = "Currently playing Monkey Bowling!"
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("PgGolf").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgGolfSequence>().m_golfMode == null) return;
                            int holeCount = Object.FindObjectOfType<PgGolfSequence>().m_golfMode.m_holeNo;
                            client.SetPresence(new RichPresence()
                            {
                                Details = "Currently playing Monkey Golf!",
                                State = $"Currently on Hole {holeCount + 1}."
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("PgBoat").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgBoatSequence>() == null) return;
                            client.SetPresence(new RichPresence()
                            {
                                Details = "Currently playing Monkey Boat!",
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("PgDogfight").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgDogfightSequence>() == null) return;
                            client.SetPresence(new RichPresence()
                            {
                                Details = "Currently playing Monkey Dogfight!",
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("PgFutsal").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgFutsalGameInfo>() == null) return;
                            int lScore= (int)GameObject.FindObjectOfType<PgFutsalGameInfo>().score[0];
                            int rScore = (int)GameObject.FindObjectOfType<PgFutsalGameInfo>().score[1];
                            client.SetPresence(new RichPresence()
                            {
                                Details = "Currently playing Monkey Soccer!",
                                State = $"Current Game: {lScore} - {rScore}"
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("PgBaseball").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgBaseballSequence>().gameData.oneTeamData == null) return;
                            int lScore = GameObject.FindObjectOfType<PgBaseballSequence>().gameData.oneTeamData.totalScore;
                            int rScore = GameObject.FindObjectOfType<PgBaseballSequence>().gameData.twoTeamData.totalScore;
                            client.SetPresence(new RichPresence()
                            {
                                Details = $"Currently playing Monkey Baseball!",
                                State = $"Current Game: {lScore} - {rScore}"
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("PgTennis").isLoaded)
                        {
                            if (GameObject.FindObjectOfType<PgTennisScore>().m_PointCount == null) return;
                            string lScore = GameObject.Find("Score0").GetComponent<PgTennisScore>().m_PointCount.count.ToString().Remove(0,1);
                            string rScore = GameObject.Find("Score1").GetComponent<PgTennisScore>().m_PointCount.count.ToString().Remove(0, 1);
                            client.SetPresence(new RichPresence()
                            {
                                Details = $"Currently playing Monkey Tennis!",
                                State = $" Current Game: {lScore} - {rScore}"
                            });
                            return;
                        }
                        if (SceneManager.GetSceneByName("MainGame").isLoaded)
                        {
                            string modeName = "";
                            string stageName = "";
                            if (GameObject.FindObjectOfType<MainGameStage>() == null) return;
                            GameObject MGS = GameObject.FindObjectOfType<MainGameStage>().gameObject;
                            if (GameObject.FindObjectOfType<Player>() == null) return;
                            string mode = MGS.GetComponent<MainGameStage>().m_GameKind.ToString();
                            switch (mode)
                            {
                                case "Story":
                                    modeName = "In Story Mode: " + GameObject.Find("Text_world").GetComponent<RubyTextMeshProUGUI>().m_text;
                                    break;
                                case "Challenge":
                                    modeName = "In " + GameObject.Find("Text_world").GetComponent<RubyTextMeshProUGUI>().m_text;
                                    break;
                                case "Practice Mode":
                                    modeName = "In Practice Mode:";
                                    break;
                                case "TimeAttack":
                                    modeName = "Ranking Challenge: " + GameObject.Find("Text_world").GetComponent<RubyTextMeshProUGUI>().m_text;
                                    break;
                                case "Reverse":
                                    modeName = "In Reverse Mode:";
                                    break;
                                case "Rotten":
                                    modeName = "In Dark Banana Mode:";
                                    break;
                                case "Golden":
                                    modeName = "In Golden Banana Mode:";
                                    break;
                            }
                            stageName = MGS.GetComponent<MainGameStage>().m_mgStageDatum.stageName;
                            // Set Presence Details as {Mode}: {Stage Name}
                            // Set Presence State as "Playing as {Character}
                            GameObject player = GameObject.Find("Player(Clone)");
                            // Every stage name is stored in all caps
                            client.SetPresence(new RichPresence()
                            {
                                Details = $"{modeName}: {stageName.Substring(0, 1) + stageName.Remove(0,1).ToLower()}",
                                State = $"Playing as {characters[(player.GetComponent<Player>().charaKind)]}"
                            });
                        }
                        break;
                }
            }
            catch
            {

            }
        }
        private static void BananaBlitzHDRPC()
        {

        }
    }
    }
