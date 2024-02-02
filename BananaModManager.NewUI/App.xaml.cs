using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using BananaModManager.Shared;
using Path = System.IO.Path;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BananaModManager.NewUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow MainWindow { get; private set; }
        public static string ManagerConfigFile => "BananaModManager.json";
        public static GameConfig GameConfig { get; private set; }
        public static ManagerConfig ManagerConfig { get; private set; }
        public static Game CurrentGame { get; private set; } = Games.Default;
        public static string GameBananaDownloadURL { get; private set; } = "";
        public static string GameBananaModId { get; private set; } = "";
        public static bool KeepRunningAfterModInstall { get; private set; } = false;

        private static Mutex _mutex = new Mutex(true, "BananaModManager");

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            // Change the working directory AS SOON AS POSSIBLE
            // For some reason one click links set it to... system32? lol ok;
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var arguments = Environment.GetCommandLineArgs();

            if (arguments.Length > 1)
            {
                if (arguments[1] == "-download")
                {
                    var modInfo = arguments[2].Split(',');
                    GameBananaDownloadURL = modInfo[0].Remove(0,17);
                    GameBananaModId = modInfo[1];

                }
                if (arguments[1] == "--update")
                {
                    try
                    {
                        Update.DoUpdate();
                        Update.UpdateModLoader();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                    Environment.Exit(0);
                    return;
                }
            }

            if (!_mutex.WaitOne(TimeSpan.Zero, true))
            {
                // Normally just show an error message
                if (GameBananaModId == "")
                {
                    MessageBox.Show("Another instance of the application is already running.", "BananaModManager", MessageBoxButtons.Ok, MessageBoxIcon.Error);
                    Environment.Exit(0);
                    return;
                }
                // If this instance is launched by GameBanana, close the older one!
                else
                {
                    var currentProcess = Process.GetCurrentProcess();
                    var processes = Process.GetProcessesByName(currentProcess.ProcessName);

                    foreach (var process in processes)
                    {
                        if (process.Id == currentProcess.Id)
                            continue;

                        process.CloseMainWindow();
                        process.WaitForExit();
                    }

                    KeepRunningAfterModInstall = true;
                }
            }

            // Load the manager config
            if (File.Exists(ManagerConfigFile))
            {
                var configFile = File.ReadAllText(ManagerConfigFile);
                ManagerConfig = configFile.Deserialize<ManagerConfig>();
            }
            else
            {
                ManagerConfig = new ManagerConfig();
            }

            // Load the game config and the mods
            var gameConfig = GameConfig;
            Mods.Load(out gameConfig, ManagerConfig.GetGameDirectory());
            GameConfig = gameConfig;

            // Detect the current game
            if (ManagerConfig.GetGameDirectory() != "")
            {
                foreach (var game in Games.List)
                {
                    if (File.Exists(Path.Combine(ManagerConfig.GetGameDirectory(), $"{game.ExecutableName}.exe")))
                    {
                        CurrentGame = game;
                        break;
                    }
                }
            }

            // Create the main window
            MainWindow = new MainWindow();
            MainWindow.Activate();
            _mutex.ReleaseMutex();
        }

        public static void SaveGameConfig()
        {
            if (ManagerConfig.GetGameDirectory() != "")
                Mods.Save(GameConfig, ManagerConfig.GetGameDirectory());
        }

        public static void SaveManagerConfig()
        {
            File.WriteAllText(ManagerConfigFile, ManagerConfig.Serialize());
        }

        public static void Restart()
        {
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Environment.Exit(0);
        }

        public static string PathConvert(string path)
        {
            return Path.Combine(ManagerConfig.GetGameDirectory(), path);
        }
    }
}
