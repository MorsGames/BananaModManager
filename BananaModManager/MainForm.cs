using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using BananaModManager.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BananaModManager
{
    public partial class MainForm : Form
    {
        private bool darkModeEnabled;
        
        public MainForm()
        {
            // Check for updates to Banana Mod Manager
            using (WebClient wc = new WebClient())
            {
                try
                {
                    wc.Headers.Add("user-agent", "request");
                    var jsondata = wc.DownloadString(new System.Uri("https://api.github.com/repos/MorsGames/BananaModManager/releases/latest"));
                    Release parsedJson = JsonConvert.DeserializeObject<Release>(jsondata);
                    string bmmversion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
                    if (parsedJson.tag_name != "v" + bmmversion)
                    {
                        if (MessageBox.Show("Your version of BananaModManager is out of date! Would you like to download the latest version?" + "\n \n Patch Notes: \n" + parsedJson.body, "Update Available!", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            // Run before updating to make sure that the folder is clean for updating
                            ClearLeftovers();
                            BananaModManager.Update.Download();
                            ProcessStartInfo startInfo = new ProcessStartInfo(AppDomain.CurrentDomain.BaseDirectory + "\\New\\BananaModManager.exe");
                            startInfo.Arguments = "--update";
                            Process.Start(startInfo);
                            Process.GetCurrentProcess().Kill();
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }

            // Run to check for leftovers in the update process
            ClearLeftovers();

            try
            {
                InitializeComponent();
                // Load the games
                foreach (var game in Games.List)
                {
                    if (File.Exists(Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), game.ExecutableName + ".exe")))
                    {
                        CurrentGame = game;
                    }
                    ComboGames.Items.Add(game);
                    
                }
                if (CurrentGame == null)
                {
                    MessageBox.Show("BananaModManager could not find a game executable! Please place all of the contents of the BananaModManager zip in the EXACT SAME directory as your Super Monkey Ball game's executable!", "Could not find game!");
                    Process.GetCurrentProcess().Kill();
                }

                // Set the currently selected game
                ComboGames.SelectedItem = CurrentGame;
                SetUpGame(CurrentGame);
                ComboGames.SelectedIndexChanged += SetupCurrentGame;
                
                // Load the mods as well as the mod order
                LoadMods();
                DarkMode();
                // Collapse the panels, only showing the mod list
                ContainerMain.Panel2Collapsed = true;
                ContainerList.Panel2Collapsed = true;

                // Set the text stuff
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                var versionString = "v" + version.Major + "." + version.Minor + "." + version.Build + "." + version.Revision;
                LabelAboutVersion.Text = "Version " + versionString;
                Text = "BananaModManager " + versionString;
            }
            catch
            {
                MessageBox.Show("An error has occurred! Something is wrong with your mods folder. Please check that ONLY mod directories are in your mods folder, then try launching again.", "Error!");
                Process.GetCurrentProcess().Kill();
            }
            

            
        }
        private void SetupCurrentGame(object sender, EventArgs e)
        {
            var game = (Game) ComboGames.SelectedItem;
            if (CurrentGame == game)
                return;

            CurrentGame = game;
            SetUpGame(CurrentGame);
        }

        // a private function to clear the leftover files from a previous update (attempt)
        private void ClearLeftovers()
        {
            try
            {
                if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\New\\"))
                {
                    Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\New\\", true);
                }
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Download.zip"))
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\Download.zip");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                
            }
        }

        private Game CurrentGame { get; set; }
        private ConfigPropertyGridAdapter configPropertyGridAdapter;
        public Mod SelectedMod => Mods.List[ListMods.SelectedItems[0].Name];

        private void LoadMods()
        {
            // Load the mods
            Mods.Load(out var modOrder, out var consoleWindow, out var speedrunMode, out var oneClick, out var fastRestart, out var saveMode, out var discordRPC, out var legacyMode, out var darkMode);

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
            CheckOneClick.Checked = oneClick;
            CheckFastRestart.Checked = fastRestart;
            SaveModeCheckbox.Checked = saveMode;
            DiscordRPC.Checked = discordRPC;
            legacyModeCheckbox.Checked = legacyMode;
            darkModeEnabled = darkMode;
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
                configPropertyGridAdapter = new ConfigPropertyGridAdapter(mod.Config, mod.DefaultConfig);
                ContainerMain.Panel2Collapsed = mod.DefaultConfig.Count == 0;
                ContainerList.Panel2Collapsed = false;
                GridConfig.SelectedObject = configPropertyGridAdapter;
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
            Mods.Save(modOrder, CheckConsole.Checked, CheckSpeedrun.Checked, CheckOneClick.Checked, CheckFastRestart.Checked, SaveModeCheckbox.Checked, DiscordRPC.Checked, legacyModeCheckbox.Checked, darkModeEnabled);

            // We wanna reload everything now
            Mods.List.Clear();
            ListMods.Items.Clear();
            LoadMods();

            // Check One-Click
            if (CheckOneClick.Checked)
            {
                // Try to create registry entry for One-Click install
                GameBanana.InstallOneClick();
            }
            if (!CheckOneClick.Checked)
            {
                GameBanana.DisableOneClick();
            }
            
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

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CheckSpeedrun_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void GridConfig_PropertyValueChanged_1(object s, PropertyValueChangedEventArgs e)
        {
            try
            {
                foreach (KeyValuePair<string, ConfigItem> kvp in SelectedMod.Config)
                {
                    if (kvp.Key == e.ChangedItem.PropertyDescriptor.Name.Replace(" ", ""))
                    {
                        kvp.Value.Value = e.ChangedItem.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured saving your mods! Please re-select the mod you're saving, then try again.");
            }
        }
        private void SetColors(Control control, string color)
        {
            
            if (color == "Black")
            {
                this.BackColor = Color.DarkSlateGray;
                if (control is PropertyGrid)
                {
                    SetColorsPropGrid((PropertyGrid)control, color);
                }
                this.BackColor = Color.DarkSlateGray;
                this.ForeColor = Color.White;
                if (control.ForeColor != Color.White)
                {
                    control.ForeColor = Color.White;
                }
                if (control.BackColor != Color.DarkSlateGray)
                {
                    control.BackColor = Color.DarkSlateGray;
                }
                if (control.HasChildren)
                {
                    foreach (Control controlChild in control.Controls)
                    {
                        SetColors(controlChild, color);
                    }
                }
            }
            if (color == "White")
            {
                if (control is PropertyGrid)
                {
                    SetColorsPropGrid((PropertyGrid)control, color);
                }
                this.BackColor = SystemColors.Window;
                this.ForeColor = SystemColors.WindowText;
                if (control.ForeColor != SystemColors.ControlText)
                {
                    control.ForeColor = SystemColors.ControlText;
                }
                if (control.BackColor != SystemColors.Window)
                {
                    control.BackColor = SystemColors.Window;
                }
                if (control.HasChildren)
                {
                    foreach (Control controlChild in control.Controls)
                    {
                        SetColors(controlChild, color);
                    }
                }
            }
        }
        private void SetColorsPropGrid(PropertyGrid propgrid, string color)
        {
            if (color == "Black")
            {
                propgrid.ViewForeColor = Color.DarkSlateGray;
                propgrid.CategoryForeColor = Color.White;
                propgrid.CategorySplitterColor = Color.White;
                propgrid.CommandsBackColor = Color.DarkSlateGray;
                propgrid.HelpBackColor = Color.DarkSlateGray;
                propgrid.LineColor = Color.DarkSlateGray;
                propgrid.HelpForeColor = Color.White;
                propgrid.CommandsBorderColor = Color.White;
                propgrid.CommandsForeColor = Color.White;
            }
            else
            {
                propgrid.ViewForeColor = SystemColors.HighlightText;
                propgrid.CategoryForeColor = SystemColors.Control;
                propgrid.CategorySplitterColor = SystemColors.Control;
                propgrid.CommandsBackColor = SystemColors.Control;
                propgrid.HelpBackColor = SystemColors.Control;
                propgrid.LineColor = SystemColors.InactiveBorder;
                propgrid.HelpForeColor = SystemColors.ControlText;
                propgrid.CommandsBorderColor = SystemColors.ControlDark;
                propgrid.CommandsForeColor = SystemColors.Control;
            }
        }

        private void DarkMode()
        {
            if (darkModeEnabled)
            {
                foreach (Control control in this.Controls)
                {
                    SetColors(control, "Black");
                }
            }
            else
            {
                foreach (Control control in this.Controls)
                {
                    SetColors(control, "White");
                }
            }
        }

        private void LabelAboutTitle_Click(object sender, EventArgs e)
        {
            darkModeEnabled = !darkModeEnabled;
            if (darkModeEnabled)
            {
                DarkMode();
                MessageBox.Show("Dark Mode enabled!");
            }
            else
            {
                DarkMode();
                MessageBox.Show("Returning to the light...");
            }

            var modOrder = new List<string>();
            foreach (ListViewItem item in ListMods.Items)
                if (item.Checked)
                    modOrder.Add(item.Name);
            Mods.Save(modOrder, CheckConsole.Checked, CheckSpeedrun.Checked, CheckOneClick.Checked, CheckFastRestart.Checked, SaveModeCheckbox.Checked, DiscordRPC.Checked, legacyModeCheckbox.Checked, darkModeEnabled);

            // We wanna reload everything now
            Mods.List.Clear();
            ListMods.Items.Clear();
            LoadMods();
        }
    }
}