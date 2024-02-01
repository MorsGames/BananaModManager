using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using BananaModManager.Shared;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media.Animation;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BananaModManager.NewUI;


/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private AppWindow _appWindow;
    private AppWindowTitleBar _titleBar;
    private const string _namespaceString = "BananaModManager.NewUI.";

    public MainWindow()
    {
        InitializeComponent();

        // Set the text stuff
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        var versionString = $"v{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        Title = $"BananaModManager {versionString}";

        // Get the app window
        _appWindow = GetAppWindowForCurrentWindow();

        // Hide system title bar
        _titleBar = _appWindow.TitleBar;
        _titleBar.ExtendsContentIntoTitleBar = true;
        _titleBar.ButtonBackgroundColor = Colors.Transparent;
        _titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        TitleBarTextBlock.Text = Title;

        // Resize the window
        var oldSize = _appWindow.Size;
        _appWindow.Resize(new SizeInt32()
        {
            Width = (int) (oldSize.Width * 0.875),
            Height = (int) (oldSize.Height)
        });

        // Load the manager settings and apply!
        ApplyManagerConfig();

        // Hide the mods and game config tabs if there is not game
        if (App.CurrentGame == Games.Default)
        {
            NavViewItemMods.Visibility = Visibility.Collapsed;
            NavViewItemGameConfig.Visibility = Visibility.Collapsed;
        }
    }

    // Apply the manager settings
    public void ApplyManagerConfig()
    {
        if (App.UserConfig.LegacyLayout)
        {
            NavView.PaneDisplayMode = NavigationViewPaneDisplayMode.Top;
            NavView.AlwaysShowHeader = false;
            ContentFrame.Margin = new Thickness(0, 24, 0, 0);
        }
        else
        {
            NavView.PaneDisplayMode = NavigationViewPaneDisplayMode.Left;
            NavView.AlwaysShowHeader = true;
            ContentFrame.Margin = new Thickness(0, -8, 0, 0);
            NavView.IsPaneOpen = false;
        }

        var content = (FrameworkElement) Content;
        content.RequestedTheme = (ElementTheme) App.UserConfig.Theme;
        switch (App.UserConfig.Theme)
        {
            case Theme.Default:
            default:
                var color = content.ActualTheme == ElementTheme.Dark ? Colors.White : Colors.Black;
                _titleBar.ButtonForegroundColor = color;
                break;
            case Theme.Light:
                _titleBar.ButtonForegroundColor = Colors.Black;
                break;
            case Theme.Dark:
                _titleBar.ButtonForegroundColor = Colors.White;
                break;
        }
    }

    // Retrieve the AppWindow instance associated with the current window
    private AppWindow GetAppWindowForCurrentWindow()
    {
        var hWnd = WindowNative.GetWindowHandle(this);
        var wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(wndId);
    }

    // Handles the loaded event of the navigation view
    private async void NavView_Loaded(object sender, RoutedEventArgs e)
    {
        // Style the header!
        NavView.Header = new TextBlock()
        {
            Text = "",
            Margin = new Thickness(-16, -20, 0, 24)
        };

        // Add handler for ContentFrame navigation
        ContentFrame.Navigated += On_Navigated;

        // NavigationView doesn't load any page by default, so load home page
        if (NavViewItemMods.Visibility == Visibility.Collapsed)
            NavView.SelectedItem = NavView.MenuItems[2];
        else
            NavView.SelectedItem = NavView.MenuItems[0];

        // Semi-unrelated to the NavView but this is the best place for it
        await FirstTimeSetup();
    }
    private async Task FirstTimeSetup()
    {
        // FIRST do the update check
        using (var wc = new WebClient())
        {
            try
            {
                wc.Headers.Add("user-agent", "request");
                var jsonData = wc.DownloadString(new System.Uri("https://api.github.com/repos/MorsGames/BananaModManager/releases/latest"));
                var parsedJson = jsonData.Deserialize<Release>();
                var bmmVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
                if (parsedJson.tag_name != $"v{bmmVersion}")
                {
                    // Ask if the user wants to update
                    var question = await ModernMessageBox.Show($"Your version of BananaModManager is out of date! Would you like to download the latest version?\n \n Patch Notes: \n{parsedJson.body}", "Update Available!",
                        "Update!", "Remind me later");

                    // If so go ahead!
                    if (question == ContentDialogResult.Primary)
                    {
                        // Run before updating to make sure that the folder is clean for updating
                        try
                        {
                            var newPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "New\\");
                            if (Directory.Exists(newPath))
                            {
                                Directory.Delete(newPath, true);
                            }
                            var downloadPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Download.zip");
                            if (File.Exists(downloadPath))
                            {
                                File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Download.zip"));
                            }
                        }
                        catch (Exception e)
                        {
                            await ModernMessageBox.Show(e.ToString(), "Error!");
                        }
                        await Update.Download();
                        var startInfo = new ProcessStartInfo($"{AppDomain.CurrentDomain.BaseDirectory}\\New\\BananaModManager.exe")
                        {
                            Arguments = "--update"
                        };
                        Process.Start(startInfo);
                        Process.GetCurrentProcess().Kill();
                    }
                }
            }
            catch (Exception e)
            {
                await ModernMessageBox.Show(e.ToString(), "Error!");
            }
        }

        // Check if a path is entered. If not, this is the first time we are opening this app!!! Probably
        if (App.GameDirectory == "")
        {
            // Show the welcome message
            var dialog = new ContentDialog
            {
                // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
                XamlRoot = RootGrid.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "\ud83c\udf4c Welcome to BananaModManager!",
                PrimaryButtonText = "Alright!",
                DefaultButton = ContentDialogButton.Primary,
                Content = new WelcomeDialogContent()
            };

            await dialog.ShowAsync();
        }
        // Show a message when an invalid game is detected
        else if (App.CurrentGame == Games.Default)
        {
            await ModernMessageBox.Show("Make sure to enter a valid game path in the settings.", "Invalid game selected!");
        }
        // We need to setup the game if it is not!
        else if (!File.Exists(App.PathConvert("BananaModManager.Shared.dll")))
        {
            await ModernMessageBox.Show("It seems like the mod loader is not installed! We will be doing that now.", "Umm...", "Alright!");
            Update.UpdateModLoader();
            App.SaveConfig();
            await ModernMessageBox.Show("It's all done! Now you're ready to go and get some mods!", "Yay!!!");
        }
        // Show a message telling the users to install some mods!
        else if (Mods.List.Count == 0)
        {
            await ModernMessageBox.Show("Just so you know, you don't have any mods installed right now! Go and get some first!", "No mods installed!");
        }
    }

    // Handles the selection changed event of the navigation view
    private void NavView_SelectionChanged(NavigationView sender,
        NavigationViewSelectionChangedEventArgs args)
    {
        // The settings item is its own thing sometimes for some reason
        if (args.IsSettingsSelected)
        {
            NavigatePage(typeof(SettingsPage), args.RecommendedNavigationTransitionInfo);
        }
        // For anything else...
        else if (args.SelectedItemContainer != null)
        {
            var navPageType = Type.GetType(_namespaceString + args.SelectedItemContainer.Tag);
            NavigatePage(navPageType, args.RecommendedNavigationTransitionInfo);
        }
    }

    // Handles the back navigation event for the navigation view
    // The button is normally not visible, but the functionality is there
    private void NavView_BackRequested(NavigationView sender,
        NavigationViewBackRequestedEventArgs args)
    {
        GoBackPage();
    }

    // Handles the event when navigation to a content frame fails
    private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    }

    // Updates the navigation view based on the content frame navigation.
    private void On_Navigated(object sender, NavigationEventArgs e)
    {
        // If we can go back then the button should be enabled
        // ...it's not visible normally but don't think about it too much
        NavView.IsBackEnabled = ContentFrame.CanGoBack;

        // Get the header
        var header = (TextBlock)NavView.Header;

        // The settings item is its own thing sometimes for some reason
        if (NavView.IsSettingsVisible && ContentFrame.SourcePageType == typeof(SettingsPage))
        {
            NavView.SelectedItem = (NavigationViewItem)NavView.SettingsItem;

            // Change the header text
            header.Text = "Settings";
        }
        // For anything else...
        else if (ContentFrame.SourcePageType != null)
        {
            // Select the nav view item that corresponds to the page being navigated to
            // In case we navigated without clicking on a nav item
            NavView.SelectedItem = NavView.MenuItems.Concat(NavView.FooterMenuItems)
                .OfType<NavigationViewItem>()
                .First(i => (_namespaceString + i.Tag).Equals(ContentFrame.SourcePageType.FullName));

            // Change the header text to the page name
            header.Text = ((NavigationViewItem) NavView.SelectedItem)?.Content?.ToString();
        }
        else
        {
            header.Text = "";
        }

        // This is a bit hacky but we don't want the header to show up in the about screen
        header.Text = (header.Text == "About") ? "" : header.Text;

        // Close the sidebar pls
        NavView.IsPaneOpen = false;
    }

    // Navigates to the specified page type with the given transition info
    private void NavigatePage(
        Type navPageType,
        NavigationTransitionInfo transitionInfo)
    {
        // Get the page type before navigation so you can prevent duplicate
        // entries in the backstack
        var preNavPageType = ContentFrame.CurrentSourcePageType;

        // Only navigate if the selected page isn't currently loaded
        // There's no need to reload the active page
        if (navPageType is not null && !Type.Equals(preNavPageType, navPageType))
        {
            ContentFrame.Navigate(navPageType, null, transitionInfo);
        }
    }

    // Navigates back to the previous page if possible
    private bool GoBackPage()
    {
        // If it's not possible, don't do it
        if (!ContentFrame.CanGoBack)
            return false;

        // Also don't go back if the nav pane is overlaid
        if (NavView.IsPaneOpen &&
            NavView.DisplayMode is NavigationViewDisplayMode.Compact or NavigationViewDisplayMode.Minimal)
            return false;

        // If it's all good, go back!
        ContentFrame.GoBack();
        return true;
    }
    private void NavView_OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        // Close the sidebar pls
        NavView.IsPaneOpen = false;
    }
}
