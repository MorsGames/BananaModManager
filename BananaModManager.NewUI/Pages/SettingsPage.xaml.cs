using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using BananaModManager.Shared;
using CommunityToolkit.WinUI;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace BananaModManager.NewUI;

/// <summary>
///     The page with settings shared across all games OR specific to the mod manager
/// </summary>
public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();

        // Load the defaults!
        TextBoxGameDirectory.Text = App.GameDirectory;
        ToggleOneClick.IsOn = App.UserConfig.OneClick;
        ComboTheme.SelectedIndex = (int) App.UserConfig.Theme;
        ToggleLegacyLayout.IsOn = App.UserConfig.LegacyLayout;

        // Set the same status text
        var isDefaultGame = App.CurrentGame == Games.Default;
        TextGameDirectoryStatus.Text = isDefaultGame ? "No game is detected!" : $"\"{App.CurrentGame.Title}\" is detected!";
        TextGameDirectoryStatus.Foreground = isDefaultGame ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Lime);

        // Disable the cards if the game directory is not set
        // We save all the other BMM settings on the game directory
        if (isDefaultGame)
        {
            CardOneClick.IsEnabled = false;
            CardTheme.IsEnabled = false;
            CardLegacyLayout.IsEnabled = false;
            CardUpdateModLoader.IsEnabled = false;
        }
    }
    private void ToggleOneClick_OnToggled(object sender, RoutedEventArgs e)
    {
        // If unchanged then don't do anything
        if (App.UserConfig.OneClick == ToggleOneClick.IsOn)
            return;

        // Otherwise change the setting
        App.UserConfig.OneClick = ToggleOneClick.IsOn;

        // Actually install the support
        if (App.UserConfig.OneClick)
        {
            // Try to create a registry entry for One-Click install
            GameBanana.InstallOneClick();
        }
        else
        {
            // Remove the registry entry
            GameBanana.DisableOneClick();
        }

        // Save the changes
        App.SaveConfig();
    }
    private void ToggleLegacyLayout_OnToggled(object sender, RoutedEventArgs e)
    {
        // If unchanged then don't do anything
        if (App.UserConfig.LegacyLayout == ToggleLegacyLayout.IsOn)
            return;

        // Otherwise change the setting
        App.UserConfig.LegacyLayout = ToggleLegacyLayout.IsOn;

        // Apply the changes
        App.MainWindow.ApplyManagerConfig();

        // Save the changes
        App.SaveConfig();
    }
    private void ComboTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // If unchanged then don't do anything
        if (App.UserConfig.Theme == (Theme) ComboTheme.SelectedIndex)
            return;

        // Otherwise change the setting
        App.UserConfig.Theme = (Theme) ComboTheme.SelectedIndex;

        // Apply the changes
        App.MainWindow.ApplyManagerConfig();

        // Save the changes
        App.SaveConfig();
    }
    private async void ButtonPickFolder_Click(object sender, RoutedEventArgs e)
    {
        // Create a folder picker
        var openPicker = new Windows.Storage.Pickers.FolderPicker();

        // See the sample code below for how to make the window accessible from the App class.
        var window = App.MainWindow;

        // Retrieve the window handle (HWND) of the current WinUI 3 window.
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        // Initialize the folder picker with the window handle (HWND).
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

        // Set options for your folder picker
        openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
        openPicker.FileTypeFilter.Add("*");

        // Open the picker for the user to pick a folder
        var folder = await openPicker.PickSingleFolderAsync();

        // If no folder is picked then abort!
        if (folder == null)
            return;

        await SaveGameDirectory(folder.Path);
    }
    private async Task SaveGameDirectory(string value)
    {
        // If unchanged then don't do anything
        if (App.GameDirectory == value)
            return;

        // Otherwise change the setting
        App.GameDirectory = value;

        // Save the changes
        await File.WriteAllTextAsync(App.DirectoryFile, App.GameDirectory);

        // Restart!
        App.Restart();
    }
    private async void TextBoxGameDirectory_LostFocus(object sender, RoutedEventArgs e)
    {
        // Save once the user is done editing
        await SaveGameDirectory(TextBoxGameDirectory.Text);
    }
    private async void ButtonUpdateModLoader_Click(object sender, RoutedEventArgs e)
    {
        // Manually triggering the update process
        Update.UpdateModLoader();
        App.SaveConfig();

        // Holy shit louis we are done
        await ModernMessageBox.Show("It's all done!", "Wow that was fast!");
    }
}
