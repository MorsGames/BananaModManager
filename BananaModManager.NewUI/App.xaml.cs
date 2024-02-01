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
        public static GameConfig GameConfig;
        public static ManagerConfig ManagerConfig;
        public static MainWindow MainWindow;
        public static Game CurrentGame = Games.Default;
        public static readonly string ManagerConfigFile = "BananaModManager.json";

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected async override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var arguments = args.Arguments.Split(' ');

            if (arguments.Length > 0)
            {
                if (arguments[0] == "-download")
                {
                    var modInfo = arguments[1].Split(',');
                    var downloadURL = modInfo[0].Remove(0,17);
                    var modID = modInfo[1];
                    await GameBanana.InstallMod(downloadURL, modID);

                }
                if (arguments[0] == "--update")
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
                }
            }

            // Load the manager config
            if (File.Exists(ManagerConfigFile))
            {
                var configFile = await File.ReadAllTextAsync(ManagerConfigFile);
                ManagerConfig = configFile.Deserialize<ManagerConfig>();
            }
            else
            {
                ManagerConfig = new ManagerConfig();
            }

            // Load the game config and the mods
            Mods.Load(out GameConfig, ManagerConfig.GetGameDirectory());

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
