using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using BananaModManager.Shared;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Path = System.IO.Path;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BananaModManager;

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
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "Error!", MessageBoxButtons.Ok, MessageBoxIcon.Error);
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
            if (!configFile.Contains("ProfileName"))
            {
                ManagerConfig.ProfileName = "Default.json";
                File.Delete(ManagerConfigFile);
                File.WriteAllText(ManagerConfigFile, ManagerConfig.Serialize());
                if (File.Exists(ManagerConfig.GetGameDirectory() + "\\mods\\" + "BananaModManager.json"))
                {
                    File.Copy(ManagerConfig.GetGameDirectory() + "\\mods\\" + "BananaModManager.json", ManagerConfig.GetGameDirectory() + "\\mods\\" + "Default.json", true);
                }
            }
        }
        else
        {
            ManagerConfig = new ManagerConfig();
        }

        // Load the game config and the mods
        if (ManagerConfig.GetGameDirectory() != "")
        {
            var gameConfig = GameConfig;
            try
            {
                Mods.Load(out gameConfig, ManagerConfig.GetGameDirectory(), ManagerConfig.ProfileName);
            }
            catch
            {
                MessageBox.Show("An error has been found in your mods folder! Please ensure all mods are installed correctly, then try launching again.", "Error!");
                Process.GetCurrentProcess().Kill();
            }
            GameConfig = gameConfig;
        }

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
        if (arguments.Length > 1)
        {
            if (arguments[1] == "--postUpdate")
            {
                try
                {
                    Update.UpdateModLoader();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Failed to automatically update the mod loader. Ensure a valid directory is set then try again.");
                }
            }
        }
        _mutex.ReleaseMutex();
    }

    public static void SaveGameConfig()
    {
        if (ManagerConfig.GetGameDirectory() != "")
            Mods.Save(GameConfig, ManagerConfig.GetGameDirectory(), App.ManagerConfig.ProfileName);
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
