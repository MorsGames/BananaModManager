using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using BananaModManager.Shared;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using static System.Net.WebRequestMethods;

namespace BananaModManager;

/// <summary>
///     The page with settings shared across all games OR specific to the mod manager
/// </summary>
public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();

        // Load the defaults!
        ToggleOneClick.IsOn = App.ManagerConfig.OneClick;
        ComboTheme.SelectedIndex = App.ManagerConfig.Theme;
        ToggleLegacyLayout.IsOn = App.ManagerConfig.LegacyLayout;

        // Set the same status text
        var isDefaultGame = App.CurrentGame == Games.Default;

        // Disable the following cards if the game directory is not set
        if (isDefaultGame)
        {
            CardUpdateModLoader.IsEnabled = false;
        }

        // Set the profiles list
        ListViewProfiles.ItemsSource = App.ManagerConfig.GameDirectories;
        ListViewProfiles.SelectedIndex = App.ManagerConfig.CurrentProfileIndex;

        // Load ComboxProfiles Options
        if (App.ManagerConfig.GameDirectories[App.ManagerConfig.CurrentProfileIndex] != "")
        {

            ComboProfiles.Items.Add("Default");
            List<string> profileList = new List<string>();
            string[] profiles = Directory.GetFiles(App.ManagerConfig.GameDirectories[App.ManagerConfig.CurrentProfileIndex] + "\\mods", "*.json");
            foreach (string profile in profiles)
            {
                if (!profile.Contains("BananaModManager.json") && !profile.Contains("Default.json"))
                {
                    ComboProfiles.Items.Add(profile.Substring(profile.LastIndexOf("\\") + 1, (profile.Substring(profile.LastIndexOf("\\") + 1).Length - 5)));
                }
            }
            if (App.ManagerConfig.ProfileName != "")
            {
                ComboProfiles.PlaceholderText = (App.ManagerConfig.ProfileName.Substring(0, App.ManagerConfig.ProfileName.Length - 5)); 
            }
            else
            {
                ComboProfiles.PlaceholderText = "Default";
            }
        }

        // Enable or disable the buttons there
        SetListViewButtons();
    }
    private void ToggleOneClick_OnToggled(object sender, RoutedEventArgs e)
    {
        // If unchanged then don't do anything
        if (App.ManagerConfig.OneClick == ToggleOneClick.IsOn)
            return;

        // Otherwise change the setting
        App.ManagerConfig.OneClick = ToggleOneClick.IsOn;

        // Actually install the support
        if (App.ManagerConfig.OneClick)
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
        App.SaveManagerConfig();
    }
    private void ToggleLegacyLayout_OnToggled(object sender, RoutedEventArgs e)
    {
        // If unchanged then don't do anything
        if (App.ManagerConfig.LegacyLayout == ToggleLegacyLayout.IsOn)
            return;

        // Otherwise change the setting
        App.ManagerConfig.LegacyLayout = ToggleLegacyLayout.IsOn;

        // Apply the changes
        App.MainWindow.ApplyManagerConfig();

        // Save the changes
        App.SaveManagerConfig();
    }
    private void ComboTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // If unchanged then don't do anything
        if (App.ManagerConfig.Theme == ComboTheme.SelectedIndex)
            return;

        // Otherwise change the setting
        App.ManagerConfig.Theme = ComboTheme.SelectedIndex;

        // Apply the changes
        App.MainWindow.ApplyManagerConfig();

        // Save the changes
        App.SaveManagerConfig();
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
        if (App.ManagerConfig.GameDirectories[ListViewProfiles.SelectedIndex] == value)
            return;

        // Otherwise change the setting
        App.ManagerConfig.GameDirectories[ListViewProfiles.SelectedIndex] = value;

        // Save the changes
        App.SaveManagerConfig();

        // Restart!
        if (ListViewProfiles.SelectedIndex == App.ManagerConfig.CurrentProfileIndex)
            App.Restart();
        else
        {
            UpdateProfilesList();
            ButtonChangePath.Flyout.Hide();
        }
    }
    private async void ButtonApplyGameDirectory_OnClick(object sender, RoutedEventArgs e)
    {
        // Save once the user is done editing
        await SaveGameDirectory(TextBoxGameDirectory.Text);
    }
    private async void ButtonUpdateModLoader_Click(object sender, RoutedEventArgs e)
    {
        // Manually triggering the update process
        try
        {
            Update.UpdateModLoader();
            await ModernMessageBox.Show("It's all done!", "Wow that was fast!");
        }
        catch (Exception exception)
        {
            await ModernMessageBox.Show(exception.ToString(), "Error!");
        }
    }
    private void ButtonAddYes_OnClick(object sender, RoutedEventArgs e)
    {
        App.ManagerConfig.GameDirectories.Add("");
        UpdateProfilesList();
        App.SaveManagerConfig();
        ButtonAdd.Flyout.Hide();
    }
    private void UpdateProfilesList()
    {
        ListViewProfiles.ItemsSource = null;
        ListViewProfiles.ItemsSource = App.ManagerConfig.GameDirectories;
        App.MainWindow.UpdateProfilesList();
    }
    private void ButtonRemoveYes_OnClick(object sender, RoutedEventArgs e)
    {
        var selection = ListViewProfiles.SelectedIndex;
        App.ManagerConfig.GameDirectories.RemoveAt(selection);
        if (App.ManagerConfig.CurrentProfileIndex == selection)
        {
            App.ManagerConfig.CurrentProfileIndex = 0;
            App.SaveManagerConfig();
            App.Restart();
        }
        else
        {
            if (App.ManagerConfig.CurrentProfileIndex > selection)
                App.ManagerConfig.CurrentProfileIndex--;

            UpdateProfilesList();
            App.SaveManagerConfig();
            ButtonRemove.Flyout.Hide();
        }
    }
    private void ButtonSetActive_OnClick(object sender, RoutedEventArgs e)
    {
        App.ManagerConfig.CurrentProfileIndex = ListViewProfiles.SelectedIndex;
        App.SaveManagerConfig();
        App.Restart();
    }
    private void ListViewProfiles_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SetListViewButtons();
    }
    private void SetListViewButtons()
    {
        ButtonRemove.IsEnabled = true;
        ButtonSetActive.IsEnabled = true;

        if (App.ManagerConfig.GameDirectories.Count <= 1)
        {
            ButtonRemove.IsEnabled = false;
            ButtonSetActive.IsEnabled = false;
        }

        var selection = ListViewProfiles.SelectedIndex;
        var current = App.ManagerConfig.CurrentProfileIndex;
        if (selection == current)
        {
            ButtonSetActive.IsEnabled = false;
        }
    }
    private void ButtonChangePath_OnClick(object sender, RoutedEventArgs e)
    {
        var gameDir = App.ManagerConfig.GameDirectories[ListViewProfiles.SelectedIndex];
        TextBoxGameDirectory.Text = gameDir;

        // Get the game
        var thisGame = Games.Default;
        if (gameDir != "")
        {
            foreach (var game in Games.List)
            {
                if (System.IO.File.Exists(Path.Combine(gameDir, $"{game.ExecutableName}.exe")))
                {
                    thisGame = game;
                    break;
                }
            }
        }

        // Set the same status text
        var isDefaultGame = thisGame == Games.Default;
        TextGameDirectoryStatus.Text = isDefaultGame ? "No game is detected!" : $"\"{thisGame.Title}\" is detected!";
        TextGameDirectoryStatus.Foreground = isDefaultGame ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Lime);
    }

    private void ComboProfiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        App.ManagerConfig.ProfileName = ComboProfiles.SelectedItem.ToString() + ".json";
        App.SaveManagerConfig();
        App.Restart();

    }

    private void ButtonAddProfile_Click(object sender, RoutedEventArgs e)
    {

    }

    private void ButtonCreateProfile_Click(object sender, RoutedEventArgs e)
    {
        if (ComboProfiles.Items.Contains(TextBoxProfileName.Text))
        {
            ModernMessageBox.Show("Check that the profile name is unique and try again.", "Please submit a new profile name!","Got it, no repeats!");
            return;
        }

        App.ManagerConfig.ProfileName = TextBoxProfileName.Text + ".json";
        App.SaveManagerConfig();
        App.Restart();
    }
    
    private void ButtonProfileRemoveYes_OnClick(object sender, RoutedEventArgs e) 
    {
        if (App.ManagerConfig.ProfileName == "Default.json" || App.ManagerConfig.ProfileName == "")
        {
            ModernMessageBox.Show("You cannot delete the default profile.", "Wait a minute, that's the default profile!", "Oops!");
            return;
        }
        System.IO.File.Delete(App.ManagerConfig.GameDirectories[ListViewProfiles.SelectedIndex] + "\\mods\\" + App.ManagerConfig.ProfileName);
        App.ManagerConfig.ProfileName = "";
        App.SaveManagerConfig();
        App.Restart();
    }

    private void ButtonStartFromCopy_Click(object sender, RoutedEventArgs e)
    {
        if (ComboProfiles.Items.Contains(TextBoxProfileName.Text))
        {
            ModernMessageBox.Show("Check that the profile name is unique and try again.", "Please submit a new profile name!", "Got it, no repeats!");
            return;
        }
        App.ManagerConfig.ProfileName = TextBoxProfileName.Text + ".json";
        App.SaveManagerConfig();
        App.SaveGameConfig();
        App.Restart();
    }
}
