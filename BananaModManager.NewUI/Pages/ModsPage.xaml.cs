using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.System;
using BananaModManager.Shared;
using CommunityToolkit.WinUI.Controls;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Win32;

namespace BananaModManager.NewUI;

/// <summary>
///     This is where most of the shit happens
/// </summary>
public sealed partial class ModsPage : Page
{
    public ObservableCollection<ModsTableItem> ModsTableList = new();
    private ModsTableItem _draggedItem;
    public ModsTableItem SelectedMod => (ModsTableItem)DataGridMods.SelectedItem;

    public ModsPage()
    {
        InitializeComponent();

        // Load the mods into the list!
        LoadList();
    }

    private void LoadList()
    {
        // Load the enabled mods!
        var order = 1;
        foreach (var mod in App.GameConfig.ActiveMods.Select(x => Mods.List[x]))
        {
            mod.Enabled = true;
            AddMod(mod, order);
            order++;
        }

        // Then the rest
        foreach (var mod in Mods.List.Select(x => x.Value))
        {
            AddMod(mod, null);
        }

        // Update the data grid
        DataGridMods.ItemsSource = ModsTableList;
        DataGridMods.SelectedIndex = 0;

        // If the list is empty then change some text around
        if (ModsTableList.Count == 0)
        {
            DataGridMods.Visibility = Visibility.Collapsed;
            PanelConfigButtons.Visibility = Visibility.Collapsed;
            TextModName.Text = "No mods found!";
            TextModAuthor.Visibility = Visibility.Collapsed;
            TextModDescription.Visibility = Visibility.Collapsed;
            DropDownButtonModOptions.IsEnabled = false;
        }
    }
    private void AddMod(Mod mod, int? order)
    {
        // Don't add if it already exists
        if (ModsTableList.Any(_ => _.Mod.Info.Id == mod.Info.Id))
            return;

        ModsTableList.Add(new ModsTableItem(mod.Enabled, order, mod.Info.Title, mod.Info.Version, mod.Info.Author, mod));
    }

    private void DataGridMods_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        var headerName = e.Column.Header.ToString();
        switch (headerName)
        {
            // The enabled tab should be smoll
            case "Enabled":
                e.Column.MaxWidth = 45;
                e.Column.MinWidth = 45;
                e.Column.Header = "";
                break;
            case "Order":
            /*e.Column.MaxWidth = 45;
            e.Column.MinWidth = 45;
            e.Column.Header = "Ord.";
            break;*/
            // Do not put this into the grid
            case "Mod":
                e.Cancel = true;
                break;
        }
    }

    private void DataGridMods_OnCurrentCellChanged(object sender, EventArgs e)
    {
        // This is a very hacky way to force the grid to always select the checkbox
        // so we don't have to triple click to change it
        DataGridMods.CurrentColumn = DataGridMods.Columns.First();
        DataGridMods.BeginEdit();

        LoadSidebar();
    }


    // ----------------------------------------------
    // DESCRIPTION LINKS
    // ----------------------------------------------

    private async void TextModDescription_OnLinkClicked(object sender, LinkClickedEventArgs e)
    {
        if (Uri.TryCreate(e.Link, UriKind.Absolute, out var link))
        {
            await Launcher.LaunchUriAsync(link);
        }
    }

    // ----------------------------------------------
    // CONFIG STUFF
    // ----------------------------------------------

    private void LoadSidebar()
    {
        // Modify the sidebar
        var item = SelectedMod;
        if (item == null)
            return;

        // Title and stuff
        TextModName.Text = item.Name;
        TextModAuthor.Text = $"by {item.Author}";
        TextModDescription.Text = item.Mod.Info.Description;

        // The author link
        var valid = Uri.TryCreate(item.Mod.Info.AuthorURL, UriKind.Absolute, out var uriResult) && uriResult.Scheme == Uri.UriSchemeHttp;
        if (valid)
        {
            HyperlinkAuthorWebsite.NavigateUri = uriResult;
            HyperlinkAuthorWebsite.Visibility = Visibility.Visible;
        }
        else
            HyperlinkAuthorWebsite.Visibility = Visibility.Collapsed;

        // The most interesting part, config stuff!

        // First, clear the existing ones
        PanelConfig.Children.Clear();

        // Now load them all!
        string currentCategory = null;

        foreach (var config in item.Mod.Config)
        {
            var category = config.Value.Category;

            if (category == null)
                category = "Config";

            // If the category has changed, add a category title
            if (category != currentCategory)
            {
                AddCategoryTitle(category);
                currentCategory = category;
            }

            // Add a card for the current config item
            AddCard(config.Key, config.Value);
        }

        // If there's no config, there's no need for the defaults button
        PanelConfigButtons.Visibility = item.Mod.Config.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    private void AddCategoryTitle(string title)
    {
        // Create an instance of Style
        var style = new Style(typeof(TextBlock));

        // Add a Setter for the Margin property
        style.Setters.Add(new Setter(MarginProperty, new Thickness(1, 24, 0, 6)));

        var header = new TextBlock
        {
            Text = title,
            Style = style,
            FontSize = 18
        };

        PanelConfig.Children.Add(header);
    }

    private void AddCard(string name, ConfigItem configItem)
    {
        // Add spaces to the setting names on the fly
        var result = new StringBuilder();

        for (var i = 0; i < name.Length; i++)
        {
            var currentChar = name[i];
            var previousChar = (i > 0) ? name[i - 1] : '\0';

            if (i > 0 && char.IsUpper(currentChar) && char.IsLower(previousChar))
            {
                // Add space before uppercase letter (excluding the first character)
                result.Append(' ');
            }

            result.Append(currentChar);
        }

        // Create the card
        var card = new SettingsCard
        {
            Header = result.ToString(),
            Description = configItem.Description,
            Padding = new Thickness(12, 12, 12, 12),
            Resources = new ResourceDictionary
            {
                {
                    "SettingsCardWrapThreshold", 256.0
                },
                {
                    "SettingsCardWrapNoIconThreshold", 256.0
                }
            }
        };

        card.Content = configItem.Value switch
        {
            // Add the content to the card
            bool => BoolControl(configItem),
            float => FloatControl(configItem),
            string => StringControl(configItem),
            _ => card.Content
        };

        // Add it to the panel!
        PanelConfig.Children.Add(card);
    }
    private static ToggleSwitch BoolControl(ConfigItem configItem)
    {
        var toggleSwitch = new ToggleSwitch
        {
            IsOn = (bool)configItem.Value
        };

        toggleSwitch.Toggled += (sender, args) => { configItem.Value = toggleSwitch.IsOn; };

        return toggleSwitch;
    }

    private static NumberBox FloatControl(ConfigItem configItem)
    {
        var numberBox = new NumberBox
        {
            Value = (float) configItem.Value,
            SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Hidden,
            LargeChange = 1f,
            SmallChange = 0.1f
        };

        numberBox.ValueChanged += (sender, args) =>
        {
            if (double.IsNaN(numberBox.Value))
                numberBox.Value = 0;

            configItem.Value = numberBox.Value;
        };

        return numberBox;
    }

    private static TextBox StringControl(ConfigItem configItem)
    {
        var textBox = new TextBox
        {
            Text = (string) configItem.Value
        };

        textBox.TextChanged += (sender, args) => { configItem.Value = textBox.Text; };

        return textBox;
    }


    // ----------------------------------------------
    // GRID SELECTION
    // ----------------------------------------------

    private void DataGridMods_OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        // Get the position of the pointer relative to the DataGrid
        var pointerPosition = e.GetCurrentPoint(DataGridMods);

        // Get the item under the pointer. It's what we are selecting!
        var item = FindItemUnderCursor(pointerPosition.Position);

        // Select it! We are doing this manually cus dragging
        if (item != null)
            DataGridMods.SelectedIndex = ModsTableList.IndexOf(item);

        // Hack. Stay in the editing mode always
        DataGridMods.BeginEdit();
    }

    private void DataGridMods_OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        foreach (var selectedItem in DataGridMods.SelectedItems)
        {
            var item = selectedItem as ModsTableItem;
            DataGridMods.CommitEdit();
            item.Enabled = !item.Enabled;
            DataGridMods.BeginEdit();
        }
    }


    // ----------------------------------------------
    // GRID REORDERING
    // ----------------------------------------------

    private void ReorderList()
    {
        // Give all the values new orders
        var order = 1;

        foreach (var item in ModsTableList)
        {
            if (item.Enabled)
            {
                item.Order = order;
                order++;
            }
            else
            {
                item.Order = null;
            }
        }

        // Reorder the list actually
        ModsTableList = new ObservableCollection<ModsTableItem>(ModsTableList.OrderBy(x => x.Order ?? int.MaxValue));

        // Update the data grid
        DataGridMods.ItemsSource = ModsTableList;
    }

    private void DataGridMods_OnDragStarting(UIElement sender, DragStartingEventArgs args)
    {
        // The selected item is the dragged item
        _draggedItem = SelectedMod;

        // Set the dragged item as the DataContext
        args.Data.SetText(_draggedItem.Name);

        // Show a "move" icon when dragging
        args.Data.RequestedOperation = DataPackageOperation.Move;

        // Hack to make the drag silhouette invisible
        args.DragUI.SetContentFromBitmapImage(new BitmapImage());
    }

    private void DataGridMods_OnDrop(object sender, DragEventArgs e)
    {
        // Get the position of the pointer relative to the DataGrid
        var pointerPosition = e.GetPosition(DataGridMods);

        // Get the item under the pointer. This is where we dropping boys
        var item = FindItemUnderCursor(pointerPosition);

        // ABORT!!! FUCK!!!!
        if (item == null)
            return;

        // Determine the original and the drop indices
        var oldIndex = ModsTableList.IndexOf(_draggedItem);
        var dropIndex = ModsTableList.IndexOf(item);

        Console.WriteLine(_draggedItem.Name);
        Console.WriteLine(item.Name);

        Console.WriteLine(oldIndex + " " + dropIndex);

        if (oldIndex == dropIndex)
            return;

        // Move the dragged item to the new position
        ModsTableList.Move(oldIndex, dropIndex);

        // Update the Order property of each item based on their new positions
        //ReorderList();
    }

    private ModsTableItem? FindItemUnderCursor(Point relativePointerPosition)
    {
        // Convert the relative coordinates to absolute coordinates
        var transform = DataGridMods.TransformToVisual(App.MainWindow.Content);
        var absolutePosition = transform.TransformPoint(relativePointerPosition);

        // Perform a hit-test to find elements at the pointer position
        var hits = VisualTreeHelper.FindElementsInHostCoordinates(absolutePosition, DataGridMods, true);

        // Find the DataGridRow by traversing the visual tree
        var dataGridRow = hits.OfType<DataGridRow>().FirstOrDefault();

        // FUCK!!!!!!!!!!
        if (dataGridRow == null)
            return null;

        return (ModsTableItem) dataGridRow.DataContext;
    }

    private void DataGridMods_OnDragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Move;
    }


    // ----------------------------------------------
    // SAVE AND PLAY BUTTONS
    // ----------------------------------------------

    private void Save()
    {
        // Update the active games list
        App.GameConfig.ActiveMods = ModsTableList.Where(x => x.Enabled).Select(x => x.Mod.Info.Id).ToList();
        foreach (var tableItem in ModsTableList)
        {
            tableItem.Mod.Enabled = tableItem.Enabled;
        }
        App.SaveGameConfig();
        ReorderList();
    }
    private void Play()
    {
        // Start the game
        App.CurrentGame.Run();
    }
    private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
    {
        Save();
    }
    private void ButtonPlay_OnClick(object sender, RoutedEventArgs e)
    {
        Play();
    }
    private void ButtonSaveAndPlay_OnClick(object sender, RoutedEventArgs e)
    {
        Save();
        Play();
    }


    // ----------------------------------------------
    // OTHER BUTTONS
    // ----------------------------------------------

    private void ButtonRefresh_OnClick(object sender, RoutedEventArgs e)
    {
        //DataGridMods.CancelEdit();
        //ModsTableList.Clear();
        //App.UnloadConfig();
        //App.LoadConfig();
        //LoadList();

        App.Restart();
    }

    private void ItemOpenFolder_OnClick(object sender, RoutedEventArgs e)
    {
        if (SelectedMod != null)
            Process.Start("explorer", SelectedMod.Mod.Directory.FullName);
    }

    private async void RemoveMod_OnClick(object sender, RoutedEventArgs e)
    {
        var confirmation = await ModernMessageBox.Show("Are you sure you want to remove this mod? There is no going back!", "Are you sure?", "Yes", "Well, on second thought...");

        if (confirmation == ContentDialogResult.Primary)
        {
            var selection = SelectedMod;

            // Actually delete it
            selection.Mod.Directory.Delete(true);

            // Remove it from the list
            ModsTableList.Remove(selection);
            DataGridMods.SelectedIndex = 0;
            var id = selection.Mod.Info.Id;
            Mods.List.Remove(id);
            App.GameConfig.ActiveMods.Remove(id);
            App.GameConfig.ModConfigs.Remove(id);

            // Force save
            Save();

            // Say we are done
            await ModernMessageBox.Show("Successfully removed the mod.", "RIP");
        }
    }

    private void ButtonReset_OnClick(object sender, RoutedEventArgs e)
    {
        // Replace existing config with a reloaded one
        var mod = SelectedMod.Mod;

        foreach (var item in mod.Config)
        {
            // Make that we arent reassigning reference values from the default config to the regular one
            mod.Config[item.Key].Value = mod.DefaultConfig[item.Key].Value;
        }

        // Reload the sidebar
        LoadSidebar();
    }
}
