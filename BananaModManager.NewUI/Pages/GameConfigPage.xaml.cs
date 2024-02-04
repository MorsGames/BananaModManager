using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BananaModManager;

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
        ToggleDiscordRPC.IsOn = App.GameConfig.DiscordRPC;
        ToggleSpeedrunMode.IsOn = App.GameConfig.SpeedrunMode;
        ToggleFastRestart.IsOn = App.GameConfig.FastRestart;
        ToggleDisableSaves.IsOn = App.GameConfig.DisableSaves;
        ToggleLegacyMode.IsOn = App.GameConfig.LegacyMode;

        // Hide the cards the game doesn't support
        if (!App.CurrentGame.DiscordRPCSupport)
        {
            CardDiscordRPC.Visibility = Visibility.Collapsed;
        }
        if (!App.CurrentGame.SpeedrunModeSupport)
        {
            CardSpeedrunMode.Visibility = Visibility.Collapsed;
            CardUpdateSpeedrunMods.Visibility = Visibility.Collapsed;
        }
        if (!App.CurrentGame.FastRestartSupport)
        {
            CardFastRestart.Visibility = Visibility.Collapsed;
        }
        if (!App.CurrentGame.DisableSavesSupport)
        {
            CardDisableSaves.Visibility = Visibility.Collapsed;
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
    private void ToggleDisableSaves_OnToggled(object sender, RoutedEventArgs e)
    {
        // If unchanged then don't do anything
        if (App.GameConfig.DisableSaves == ToggleDisableSaves.IsOn)
            return;

        // Otherwise change the setting
        App.GameConfig.DisableSaves = ToggleDisableSaves.IsOn;

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
        Process.Start("explorer", System.IO.Path.Combine(App.ManagerConfig.GetGameDirectory(), "mods"));
    }
}
