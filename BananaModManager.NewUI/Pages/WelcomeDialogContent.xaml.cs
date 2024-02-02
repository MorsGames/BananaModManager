using BananaModManager.Shared;
using Microsoft.UI.Xaml.Controls;

namespace BananaModManager;

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
