using System;
using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BananaModManager;

/// <summary>
///     The page that shows who made this mod loader
/// </summary>
public sealed partial class AboutPage : Page
{
    public AboutPage()
    {
        InitializeComponent();

        // Set the version text
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        var versionString = $"v{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        TextVersion.Text += versionString;

        // Set the credits part
        TextBoxCredits.Text =
@"--------------------------
Open Source Projects Used:
--------------------------

- UnityDoorstop:
https://github.com/NeighTools/UnityDoorstop

- Il2CppDumper:
https://github.com/Perfare/Il2CppDumper

- Il2CppAssemblyUnhollower:
https://github.com/knah/Il2CppAssemblyUnhollower

- Mono:
https://github.com/mono/mono

- DiscordRichPresence:
https://github.com/Lachee/discord-rpc-csharp

- Windows App SDK:
https://github.com/microsoft/windowsappsdk

- Windows Community Toolkit:
https://github.com/CommunityToolkit/Windows


""I really love bananas!"" -A wise man";
    }
    private async void ButtonShowWelcome_OnClick(object sender, RoutedEventArgs e)
    {
        // Show the welcome message
        var dialog = new ContentDialog
        {
            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            XamlRoot = App.MainWindow.RootGrid.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = "\ud83c\udf4c Welcome to BananaModManager!",
            PrimaryButtonText = "Alright!",
            DefaultButton = ContentDialogButton.Primary,
            Content = new WelcomeDialogContent()
        };

        await dialog.ShowAsync();
    }
}
