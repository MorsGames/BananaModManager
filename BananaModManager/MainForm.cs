using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
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
            foreach (var game in Games.List)
            {
                if (File.Exists(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), game.ExecutableName + ".exe")))
                {
                    CurrentGame = game;
                }
                ComboGames.Items.Add(game);
            }

            // Set the currently selected game
            ComboGames.SelectedItem = CurrentGame;
            SetUpGame(CurrentGame);
            ComboGames.SelectedIndexChanged += SetupCurrentGame;

            // Load the mods as well as the mod order
            LoadMods();

            // Collapse the panels, only showing the mod list
            ContainerMain.Panel2Collapsed = true;
            ContainerList.Panel2Collapsed = true;

            // Set the text stuff
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var versionString = "v" + version.Major + "." + version.Minor + "." + version.Build;
            LabelAboutVersion.Text = "Version " + versionString;
            Text = "BananaModManager " + versionString;
        }

        private void SetupCurrentGame(object sender, EventArgs e)
        {
            var game = (Game) ComboGames.SelectedItem;
            if (CurrentGame == game)
                return;

            CurrentGame = game;
            SetUpGame(CurrentGame);
        }

        private Game CurrentGame { get; set; }

        public Mod SelectedMod => Mods.List[ListMods.SelectedItems[0].Name];

        private void LoadMods()
        {
            // Load the mods
            Mods.Load(out var modOrder, out var consoleWindow, out var speedrunMode);

            // First add the mods based on their order
            foreach (var mod in modOrder.Select(_ => Mods.List[_]))
            {
                mod.Enabled = true;
                AddMod(mod);
            }

            // Then the rest
            foreach (var mod in Mods.List.Select(_ => _.Value)) AddMod(mod);

            // And set all the other user settings
            CheckConsole.Checked = consoleWindow;
            CheckSpeedrun.Checked = speedrunMode;
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

        private void SetUpGame(Game game)
        {
            // No game, no setup.
            if (game == null)
                return;

            // TODO: Don't hardcode the names like this...
            File.Copy(game.Managed ? "doorstop_config_mono.ini" : "doorstop_config_il2cpp.ini", "doorstop_config.ini", true);

            // Check if the game is managed or already decompiled.
            if (game.Managed || Directory.Exists("managed")) return;

            // Ask if the user wants to do it
            if (MessageBox.Show("Game files need to be extracted to enable mod support. This might take a few minutes, and some extra files might get downloaded during the process. Do you want to continue?",
                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, 
                    MessageBoxDefaultButton.Button1) != DialogResult.Yes) 
                return;

            var tempPath = Path.Combine(Path.GetTempPath(), "BananaModManager");
            Directory.CreateDirectory(tempPath);

            // Download the necessary tools
            if (!Directory.Exists("Il2CppDumper"))
            {
                var zipPath = Path.Combine(tempPath + "Il2CppDumper-v6.6.5.zip");
                using var client = new WebClient();
                client.DownloadFile("https://github.com/Perfare/Il2CppDumper/releases/download/v6.6.5/Il2CppDumper-v6.6.5.zip", zipPath);
                ZipFile.ExtractToDirectory(zipPath, "Il2CppDumper");
            }
            if (!Directory.Exists("Il2CppAssemblyUnhollower"))
            {
                var zipPath = Path.Combine(tempPath + "Il2CppAssemblyUnhollower.0.4.15.4.zip");
                using var client = new WebClient();
                client.DownloadFile("https://github.com/knah/Il2CppAssemblyUnhollower/releases/download/v0.4.15.4/Il2CppAssemblyUnhollower.0.4.15.4.zip", zipPath);
                ZipFile.ExtractToDirectory(zipPath, "Il2CppAssemblyUnhollower");
            }

            // Run the tools if needed
            if (!Directory.Exists("Il2CppDumper\\DummyDll"))
            {
                var process = Process.Start("Il2CppDumper\\Il2CppDumper.exe",
                    "\"GameAssembly.dll\" " + game.ExecutableName
                                            + "\"_Data\\il2cpp_data\\Metadata\\global-metadata.dat\"");
                process.WaitForExit();
            }

            var process2 = Process.Start("Il2CppAssemblyUnhollower\\AssemblyUnhollower.exe",
                "--input=\"Il2CppDumper\\DummyDll\" --output=\"managed\" --mscorlib=\"mono\\Managed\\mscorlib.dll\"");
            process2.WaitForExit();

            // Overwrite dummy mono files with actual ones
            foreach (var path in Directory.GetFiles("mono\\Managed", "*.dll", SearchOption.AllDirectories))
            {
                var newPath = path.Replace("mono\\Managed", "managed");
                File.Copy(path, newPath, true);
            }
            /*foreach (var path in Directory.GetFiles("Il2CppAssemblyUnhollower", "*.dll", SearchOption.AllDirectories))
            {
                var newPath = path.Replace("Il2CppAssemblyUnhollower", "managed");
                File.Copy(path, newPath, true);
            }*/

            // Delete stuff we don't need
            Directory.Delete("Il2CppDumper", true);
            Directory.Delete("Il2CppAssemblyUnhollower", true);
            Directory.Delete(tempPath, true);
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
                var mod = SelectedMod;

                ContainerMain.Panel2Collapsed = mod.DefaultConfig.Count == 0;
                ContainerList.Panel2Collapsed = false;
                GridConfig.SelectedObject = new ConfigPropertyGridAdapter(mod.Config, mod.DefaultConfig);
                LabelTitle.Text = mod.Info.Title + " by " + mod.Info.Author;
                if (mod.Info.AuthorURL == "") 
                    LabelTitle.LinkArea = new LinkArea(0, 0);
                else
                {
                    LabelTitle.Text += "    ";
                    LabelTitle.LinkArea = new LinkArea(mod.Info.Title.Length + 4, mod.Info.Author.Length);
                }

                TextDescription.Text = Environment.NewLine + mod.Info.Description;
            }
            else
            {
                ContainerMain.Panel2Collapsed = true;
                ContainerList.Panel2Collapsed = true;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Get the mod order
            var modOrder = new List<string>();
            foreach (ListViewItem item in ListMods.Items)
                if (item.Checked)
                    modOrder.Add(item.Name);
            Mods.Save(modOrder, CheckConsole.Checked, CheckSpeedrun.Checked);

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

        private void DeleteMod()
        {
            if (MessageBox.Show("This will delete the selected mod. Are you sure?\n\nThere will be no way to recover it.",
                "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                SelectedMod.Directory.Delete(true);
            }
        }

        private void deleteModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteMod();
        }


        private void BtnRemove_Click(object sender, EventArgs e)
        {
            DeleteMod();
        }

        private void BtnDiscord_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/vuZWDMzzye");
        }

        private void BtnSave2_Click(object sender, EventArgs e)
        {
            BtnSave_Click(sender, e);
        }
    }
}