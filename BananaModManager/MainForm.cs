using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using BananaModManager.Shared;

namespace BananaModManager
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            // Load the games
            foreach (var game in Games.List) ComboGames.Items.Add(game);

            // Set the currently selected game
            ComboGames.Text = ComboGames.Items[0].ToString();

            // Load the mods as well as the mod order
            LoadMods();

            // Collapse the panels, only showing the mod list
            ContainerMain.Panel2Collapsed = true;
            ContainerList.Panel2Collapsed = true;

            // Set the text stuff
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var versionString = "v" + version.Major + "." + version.Minor + "." + version.Revision;
            LabelAboutVersion.Text = "Version " + versionString;
            Text = "BananaModManager " + versionString;
        }

        private Game CurrentGame => (Game) ComboGames.SelectedItem;

        public Mod SelectedMod => Mods.List[ListMods.SelectedItems[0].Name];

        private void LoadMods()
        {
            // Load the mods
            Mods.Load(out var modOrder);

            // First add the mods based on their order
            foreach (var mod in modOrder.Select(_ => Mods.List[_]))
            {
                mod.Enabled = true;
                AddMod(mod);
            }

            // Then the rest
            foreach (var mod in Mods.List.Select(_ => _.Value)) AddMod(mod);
        }

        private void AddMod(Mod mod)
        {
            // Ignore if the mod is already added
            if (ListMods.Items.ContainsKey(mod.ToString()))
                return;

            // Add the mod
            ListMods.Items.Add(new ListViewItem
            {
                Name = mod.ToString(),
                Text = mod.Info.Title,
                SubItems = {mod.Info.Version, mod.Info.Author},
                Checked = mod.Enabled
            });
        }

        private void BtnPlay_Click(object sender, EventArgs e)
        {
            // Start the game
            CurrentGame.Run();
        }

        private void BtnSaveAndPlay_Click(object sender, EventArgs e)
        {
            // Simulate presses for those 2 other buttons in a row
            BtnSave_Click(sender, e);
            BtnPlay_Click(sender, e);
        }

        private void ListMods_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Change what's shown on the PropertyGrid
            if (ListMods.SelectedItems.Count > 0)
            {
                ContainerMain.Panel2Collapsed = false;
                ContainerList.Panel2Collapsed = false;
                var mod = SelectedMod;
                GridConfig.SelectedObject = new ConfigPropertyGridAdapter(mod.Config, mod.DefaultConfig);
                LabelTitle.Text = mod.Info.Title + " by " + mod.Info.Author + "    ";
                LabelTitle.LinkArea = new LinkArea(mod.Info.Title.Length + 4, mod.Info.Author.Length);
                TextDescription.Text = Environment.NewLine + mod.Info.Description;
            }
            else
            {
                ContainerMain.Panel2Collapsed = true;
                ContainerList.Panel2Collapsed = true;
            }
        }

        private void BoolOnCheckedChanged(object sender, EventArgs e)
        {
            var checkBox = (CheckBox) sender;
            checkBox.Font = new Font(Font, FontStyle.Bold);
            checkBox.Text = checkBox.Checked ? "Enabled" : "Disabled";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Get the mod order
            var modOrder = new List<string>();
            foreach (ListViewItem item in ListMods.Items)
                if (item.Checked)
                    modOrder.Add(item.Name);
            Mods.Save(modOrder);

            // We wanna reload everything now
            Mods.List.Clear();
            ListMods.Items.Clear();
            LoadMods();
        }

        private void ListMods_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            Mods.List[e.Item.Name].Enabled = e.Item.Checked;
        }

        private void ToolStripMenuItemReset_Click(object sender, EventArgs e)
        {
            GridConfig.ResetSelectedProperty();
        }

        private void LabelTitle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(SelectedMod.Info.AuthorURL);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var g = CreateGraphics();
            try
            {
                TabControl.ItemSize = new Size((int) (TabControl.ItemSize.Width * g.DpiX / 96f),
                    (int) (TabControl.ItemSize.Height * g.DpiY / 96f));
            }
            finally
            {
                g.Dispose();
            }
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(SelectedMod.Directory.FullName);
        }

        private void deleteModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will delete the selected mod. Are you sure?\n\nThere will be no way to recover it.",
                "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                SelectedMod.Directory.Delete(true);
            }
        }
    }
}