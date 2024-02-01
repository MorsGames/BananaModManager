using System.Diagnostics;
using ABI.Microsoft.UI.Xaml.Shapes;
using BananaModManager.Shared;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BananaModManager.NewUI;

/// <summary>
///     The page where you set game specific settings
/// </summary>
public sealed partial class GameConfigPage : Page
{
    public GameConfigPage()
    {
        InitializeComponent();

        // Load the defaults!
        ToggleConsole.IsOn = App.GameConfig.ConsoleWindow;
        ToggleSpeedrunMode.IsOn = App.GameConfig.SpeedrunMode;
        ToggleFastRestart.IsOn = App.GameConfig.FastRestart;
        ToggleSaveMode.IsOn = App.GameConfig.SaveMode;
        ToggleDiscordRPC.IsOn = App.GameConfig.DiscordRPC;
        ToggleLegacyMode.IsOn = App.GameConfig.LegacyMode;

        // Hide the cards the game doesn't support
        // If the game doesn't support any of them then the page is hidden entirely
        if (!App.CurrentGame.SpeedrunModeSupport)
        {
            CardSpeedrunMode.Visibility = Visibility.Collapsed;
            CardUpdateSpeedrunMods.Visibility = Visibility.Collapsed;
        }
        if (!App.CurrentGame.FastRestartSupport)
        {
            CardFastRestart.Visibility = Visibility.Collapsed;
        }
        if (!App.CurrentGame.SaveModeSupport)
        {
            CardSaveMode.Visibility = Visibility.Collapsed;
        }
        if (!App.CurrentGame.DiscordRPCSupport)
        {
            CardDiscordRPC.Visibility = Visibility.Collapsed;
        }
        if (!App.CurrentGame.LegacyModeSupport)
        {
            CardLegacyMode.Visibility = Visibility.Collapsed;
        }
    }
    private void ToggleConsole_OnToggled(object sender, RoutedEventArgs e)
    {
        // If unchanged then don't do anything
        if (App.GameConfig.ConsoleWindow == ToggleConsole.IsOn)
            return;

        // Otherwise change the setting
        App.GameConfig.ConsoleWindow = ToggleConsole.IsOn;

        // Save the changes
        App.SaveGameConfig();
    }
    private void ToggleSpeedrunMode_OnToggled(object sender, RoutedEventArgs e)
    {
        // If unchanged then don't do anything
        if (App.GameConfig.SpeedrunMode == ToggleSpeedrunMode.IsOn)
            return;

        // Otherwise change the setting
        App.GameConfig.SpeedrunMode = ToggleSpeedrunMode.IsOn;

        // Save the changes
        App.SaveGameConfig();
    }
    private void ToggleFastRestart_OnToggled(object sender, RoutedEventArgs e)
    {
        // If unchanged then don't do anything
        if (App.GameConfig.FastRestart == ToggleFastRestart.IsOn)
            return;

        // Otherwise change the setting
        App.GameConfig.FastRestart = ToggleFastRestart.IsOn;

        // Save the changes
        App.SaveGameConfig();
    }
    private void ToggleSaveMode_OnToggled(object sender, RoutedEventArgs e)
    {
        // If unchanged then don't do anything
        if (App.GameConfig.SaveMode == ToggleSaveMode.IsOn)
            return;

        // Otherwise change the setting
        App.GameConfig.SaveMode = ToggleSaveMode.IsOn;

        // Save the changes
        App.SaveGameConfig();
    }
    private void ToggleDiscordRPC_OnToggled(object sender, RoutedEventArgs e)
    {
        // If unchanged then don't do anything
        if (App.GameConfig.DiscordRPC == ToggleDiscordRPC.IsOn)
            return;

        // Otherwise change the setting
        App.GameConfig.DiscordRPC = ToggleDiscordRPC.IsOn;

        // Save the changes
        App.SaveGameConfig();
    }
    private void ToggleLegacyMode_OnToggled(object sender, RoutedEventArgs e)
    {
        // If unchanged then don't do anything
        if (App.GameConfig.LegacyMode == ToggleLegacyMode.IsOn)
            return;

        // Otherwise change the setting
        App.GameConfig.LegacyMode = ToggleLegacyMode.IsOn;

        // Save the changes
        App.SaveGameConfig();
    }
    private async void ButtonUpdateSpeedrunMods_Click(object sender, RoutedEventArgs e)
    {
        // DO IT!!
        await Update.UpdateSpeedrunLegalMods();

        // We are done!
        await ModernMessageBox.Show("Successfully updated all the mods!", "Yay!");
        App.Restart();
    }
    private void CardOpenModsFolder_OnClick(object sender, RoutedEventArgs e)
    {
        // Open the mods folder
        Process.Start("explorer", System.IO.Path.Combine(App.ManagerConfig.GameDirectory, "mods"));
    }
}
