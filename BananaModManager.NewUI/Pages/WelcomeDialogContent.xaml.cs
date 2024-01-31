using Microsoft.UI.Xaml.Controls;
using BananaModManager.Shared;

namespace BananaModManager.NewUI;

/// <summary>
///     The popup you get when you first run BananaModManager
/// </summary>
public sealed partial class WelcomeDialogContent : Page
{
    public WelcomeDialogContent()
    {
        InitializeComponent();

        // Let's not hardcode the games...
        foreach (var game in Games.List)
        {
            var textBlock = new TextBlock()
            {
                Text = game.Title,
            };
            PanelGames.Children.Add(textBlock);
        }
    }
}
