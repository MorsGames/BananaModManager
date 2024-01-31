using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BananaModManager.NewUI;

public static class ModernMessageBox
{
    public static async Task<ContentDialogResult> Show(string text, string caption, string primaryButtonText = "Ok")
    {
        var dialog = new ContentDialog
        {
            XamlRoot = App.MainWindow.RootGrid.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = caption,
            PrimaryButtonText = primaryButtonText,
            DefaultButton = ContentDialogButton.Primary,
            Content = text
        };

        return await dialog.ShowAsync();
    }

    public static async Task<ContentDialogResult> Show(string text, string caption, string primaryButtonText, string secondaryButtonText)
    {
        var dialog = new ContentDialog
        {
            XamlRoot = App.MainWindow.RootGrid.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = caption,
            PrimaryButtonText = primaryButtonText,
            SecondaryButtonText = secondaryButtonText,
            DefaultButton = ContentDialogButton.Primary,
            Content = text
        };

        return await dialog.ShowAsync();
    }
}
