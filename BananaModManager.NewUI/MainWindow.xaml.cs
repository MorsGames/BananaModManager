using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Windows.System;
using Windows.Graphics;
using BananaModManager.Shared;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BananaModManager;


/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private AppWindow _appWindow;
    private AppWindowTitleBar _titleBar;
    private const string _namespaceString = "BananaModManager.";

    public MainWindow()
    {
        InitializeComponent();

        // Set the text stuff
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        var versionString = $"v{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        Title = $"BananaModManager {versionString}";

        // Get the app window
        var hWnd = WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        _appWindow = AppWindow.GetFromWindowId(windowId);

        // Set the icon
        _appWindow.SetIcon("ProgramIcon.ico");

        // Hide system title bar
        _titleBar = _appWindow.TitleBar;
        _titleBar.ExtendsContentIntoTitleBar = true;
        _titleBar.ButtonBackgroundColor = Colors.Transparent;
        _titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        TitleBarTextBlock.Text = Title;

        // Resize the window
        /*var oldSize = _appWindow.Size;
        _appWindow.Resize(new SizeInt32()
        {
            Width = (int) (oldSize.Width * 0.875),
            Height = (int) (oldSize.Height)
        });*/

        // Set the minimum size
        MinWindowSize.Set(hWnd, 800, 600);

        // Load the manager settings and apply!
        ApplyManagerConfig();

        // Hide the mods and game config tabs if there is not game
        if (App.CurrentGame == Games.Default)
        {
            NavViewItemMods.Visibility = Visibility.Collapsed;
            NavViewItemGameConfig.Visibility = Visibility.Collapsed;
        }

        UpdateProfilesList();
    }

    public void UpdateProfilesList()
    {
        // Load the profiles (UI calls them games)
        // If there are no profiles then hide the tab
        if (App.ManagerConfig.GameDirectories.Count <= 1)
        {
            NavViewItemProfiles.Visibility = Visibility.Collapsed;
        }
        // Otherwise...
        else
        {
            NavViewItemProfiles.Visibility = Visibility.Visible;
            var itemsList = new List<NavigationViewItem>();

            // Add the header
            var header = new NavigationViewItem()
            {
                Content = "Switch game:",
                SelectsOnInvoked = false,
                IsEnabled = false,
                Margin = new Thickness(0, -4, 0, -4)
            };
            itemsList.Add(header);

            // Go through each profile
            for (var i = 0; i < App.ManagerConfig.GameDirectories.Count; i++)
            {
                var gameDir = App.ManagerConfig.GameDirectories[i];

                // Get the name
                var gameName = "Unknown Game";
                if (gameDir != "")
                {
                    foreach (var game in Games.List)
                    {
                        if (File.Exists(Path.Combine(gameDir, $"{game.ExecutableName}.exe")))
                        {
                            gameName = game.Title;
                            break;
                        }
                    }
                }
                var menuItem = new NavigationViewItem
                {
                    Content = gameName,
                    SelectsOnInvoked = false,
                };

                if (i == App.ManagerConfig.CurrentProfileIndex)
                {
                    menuItem.Icon = new FontIcon()
                    {
                        Glyph = "\uE73E"
                    };
                }

                // Weird C# quirk requires us to do this
                var index = i;

                // Add the handler
                menuItem.Tapped += (o, args) => MenuItemOnTapped(o, args, index);

                // Add it to the list :D
                itemsList.Add(menuItem);
            }

            NavViewItemProfiles.MenuItemsSource = itemsList;
        }
    }

    private void MenuItemOnTapped(object sender, TappedRoutedEventArgs e, int index)
    {
        if (App.ManagerConfig.CurrentProfileIndex == index)
            return;
        App.ManagerConfig.CurrentProfileIndex = index;
        App.SaveManagerConfig();
        App.Restart();
    }

    // Apply the manager settings
    public void ApplyManagerConfig()
    {
        if (App.ManagerConfig.LegacyLayout)
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
        switch (App.ManagerConfig.Theme)
        {
            case 0:
                content.RequestedTheme = ElementTheme.Light;
                _titleBar.ButtonForegroundColor = Colors.Black;
                break;
            case 1:
                content.RequestedTheme = ElementTheme.Dark;
                _titleBar.ButtonForegroundColor = Colors.White;
                break;
            case 2:
            default:
                content.RequestedTheme = ElementTheme.Default;
                var color = content.ActualTheme == ElementTheme.Dark ? Colors.White : Colors.Black;
                _titleBar.ButtonForegroundColor = color;
                break;
        }
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
        // First check if the user is an idiot and put the mod manager on the same folder as the game. DON'T DO THAT ANYMORE
        foreach (var game in Games.List)
        {
            var exeName = $"{game.ExecutableName}.exe";
            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (File.Exists(Path.Combine(currentDir, exeName)))
            {
                await ModernMessageBox.Show("If you're coming from an older version of BananaModManager you should download it again from the project's GitHub page and extract it to a different folder.", "Your BananaModManager installation is on the same folder as the game!", "Sorry, won't do it again!");

                if (Uri.TryCreate("https://github.com/MorsGames/BananaModManager/releases", UriKind.Absolute, out var link))
                {
                    await Launcher.LaunchUriAsync(link);
                }

                Environment.Exit(0);
            }
        }

        // Then do the update check
        using (var client = new WebClient())
        {
            try
            {
                client.Headers.Add("user-agent", "request");
                var jsonData = client.DownloadString(new System.Uri("https://api.github.com/repos/MorsGames/BananaModManager/releases/latest"));
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
                        // Download the update and run the executable.
                        await Update.DownloadAndRun();

                        // We need to close the current process so the updater can update
                        Environment.Exit(0);
                    }
                }
            }
            catch (Exception e)
            {
                await ModernMessageBox.Show(e.ToString(), "Error!");
            }
        }

        // Check if a path is entered. If not, this is the first time we are opening this app!!! Probably
        if (App.ManagerConfig.GetGameDirectory() == "" && App.ManagerConfig.GameDirectories.Count == 1)
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

            await ModernMessageBox.Show("To get started, click \"Change Path\" and enter the path to the folder containing the game's files.", "Important!", "Okay, okay, I got it!");
        }
        // Show a message when an invalid game is detected
        else if (App.CurrentGame == Games.Default)
        {
            await ModernMessageBox.Show("Make sure to enter a valid game path in the settings.", "Invalid game selected!");
        }
        // The game is valid, we can continue
        else
        {
            // If one click is enabled AND there's a mod incoming then boy o boy!
            if (App.ManagerConfig.OneClick && App.GameBananaModId != "")
            {
                // Create the WebClient
                var client = new WebClient();

                using (client)
                {
                    // Ask the big question
                    var question = await ModernMessageBox.Show($"Do you want to download and install the mod \"{GameBanana.GetModName(App.GameBananaModId, client)}\" to \"{App.CurrentGame.Title}\"?", "Download mod?",
                        "Yes", "No");

                    // She said yes!!!!
                    if (question == ContentDialogResult.Primary)
                    {
                        await GameBanana.InstallMod(App.GameBananaDownloadURL, App.GameBananaModId, client);
                        if (App.KeepRunningAfterModInstall)
                            App.Restart();
                        else
                            Environment.Exit(0);
                    }
                    else
                    {
                        if (!App.KeepRunningAfterModInstall)
                            Environment.Exit(0);
                    }
                }
            }

            // We need to setup the game if it is not!
            if (!File.Exists(App.PathConvert("BananaModManager.Shared.dll")))
            {
                await ModernMessageBox.Show("It seems like the mod loader is not installed! We will be doing that now.", "Mod loader not found", "Alright!");
                try
                {
                    Update.UpdateModLoader();
                    App.SaveGameConfig();
                    await ModernMessageBox.Show("It's all done! Now you're ready to go and get some mods!", "Yay!!!");
                }
                catch (Exception e)
                {
                    await ModernMessageBox.Show(e.ToString(), "Error!");
                }
            }
            // Show a message telling the users to install some mods!
            else if (Mods.List.Count == 0)
            {
                await ModernMessageBox.Show("Just so you know, you don't have any mods installed right now! Go and get some first!", "No mods are installed!");
            }
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
