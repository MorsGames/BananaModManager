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
        ToggleConsole.IsOn = App.UserConfig.ConsoleWindow;
        ToggleSpeedrunMode.IsOn = App.UserConfig.SpeedrunMode;
        ToggleFastRestart.IsOn = App.UserConfig.FastRestart;
        ToggleSaveMode.IsOn = App.UserConfig.SaveMode;
        ToggleDiscordRPC.IsOn = App.UserConfig.DiscordRPC;
        ToggleLegacyMode.IsOn = App.UserConfig.LegacyMode;

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
        if (App.UserConfig.ConsoleWindow == ToggleConsole.IsOn)
            return;

        // Otherwise change the setting
        App.UserConfig.ConsoleWindow = ToggleConsole.IsOn;

        // Save the changes
        App.SaveConfig();
    }
    private void ToggleSpeedrunMode_OnToggled(object sender, RoutedEventArgs e)
    {
        // If unchanged then don't do anything
        if (App.UserConfig.SpeedrunMode == ToggleSpeedrunMode.IsOn)
            return;

        // Otherwise change the setting
        App.UserConfig.SpeedrunMode = ToggleSpeedrunMode.IsOn;

        // Save the changes
        App.SaveConfig();
    }
    private void ToggleFastRestart_OnToggled(object sender, RoutedEventArgs e)
    {
        // If unchanged then don't do anything
        if (App.UserConfig.FastRestart == ToggleFastRestart.IsOn)
            return;

        // Otherwise change the setting
        App.UserConfig.FastRestart = ToggleFastRestart.IsOn;

        // Save the changes
        App.SaveConfig();
    }
    private void ToggleSaveMode_OnToggled(object sender, RoutedEventArgs e)
    {
        // If unchanged then don't do anything
        if (App.UserConfig.SaveMode == ToggleSaveMode.IsOn)
            return;

        // Otherwise change the setting
        App.UserConfig.SaveMode = ToggleSaveMode.IsOn;

        // Save the changes
        App.SaveConfig();
    }
    private void ToggleDiscordRPC_OnToggled(object sender, RoutedEventArgs e)
    {
        // If unchanged then don't do anything
        if (App.UserConfig.DiscordRPC == ToggleDiscordRPC.IsOn)
            return;

        // Otherwise change the setting
        App.UserConfig.DiscordRPC = ToggleDiscordRPC.IsOn;

        // Save the changes
        App.SaveConfig();
    }
    private void ToggleLegacyMode_OnToggled(object sender, RoutedEventArgs e)
    {
        // If unchanged then don't do anything
        if (App.UserConfig.LegacyMode == ToggleLegacyMode.IsOn)
            return;

        // Otherwise change the setting
        App.UserConfig.LegacyMode = ToggleLegacyMode.IsOn;

        // Save the changes
        App.SaveConfig();
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
        Process.Start("explorer", System.IO.Path.Combine(App.GameDirectory, "mods"));
    }
}
