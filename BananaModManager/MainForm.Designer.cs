
using BananaModManager.Controls;
using ItemCheckedEventHandler = System.Windows.Forms.ItemCheckedEventHandler;

namespace BananaModManager
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MenuConfig = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItemReset = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuMods = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteModToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TabControl = new BananaModManager.Controls.TabControl(this.components);
            this.PageMods = new System.Windows.Forms.TabPage();
            this.ContainerMain = new System.Windows.Forms.SplitContainer();
            this.ContainerList = new System.Windows.Forms.SplitContainer();
            this.ListMods = new BananaModManager.Controls.ListView();
            this.ColName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColVersion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColAuthor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LabelTitle = new System.Windows.Forms.LinkLabel();
            this.TextDescription = new System.Windows.Forms.TextBox();
            this.GridConfig = new BananaModManager.Controls.PropertyGrid(this.components);
            this.BtnRemove = new System.Windows.Forms.Button();
            this.BtnDiscord = new System.Windows.Forms.Button();
            this.BtnSaveAndPlay = new System.Windows.Forms.Button();
            this.BtnSave = new System.Windows.Forms.Button();
            this.BtnPlay = new System.Windows.Forms.Button();
            this.PageSettings = new System.Windows.Forms.TabPage();
            this.BtnSave2 = new System.Windows.Forms.Button();
            this.PanelSettings = new System.Windows.Forms.FlowLayoutPanel();
            this.LabelGame = new System.Windows.Forms.Label();
            this.ComboGames = new System.Windows.Forms.ComboBox();
            this.CheckConsole = new System.Windows.Forms.CheckBox();
            this.CheckSpeedrun = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CheckOneClick = new System.Windows.Forms.CheckBox();
            this.OneClickLabel = new System.Windows.Forms.Label();
            this.CheckFastRestart = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.PageAbout = new System.Windows.Forms.TabPage();
            this.TextInfo = new System.Windows.Forms.TextBox();
            this.LabelAboutVersion = new System.Windows.Forms.Label();
            this.LabelAboutTitle = new System.Windows.Forms.Label();
            this.SaveModeCheckbox = new System.Windows.Forms.CheckBox();
            this.SaveModeLabel = new System.Windows.Forms.Label();
            this.MenuConfig.SuspendLayout();
            this.MenuMods.SuspendLayout();
            this.TabControl.SuspendLayout();
            this.PageMods.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ContainerMain)).BeginInit();
            this.ContainerMain.Panel1.SuspendLayout();
            this.ContainerMain.Panel2.SuspendLayout();
            this.ContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ContainerList)).BeginInit();
            this.ContainerList.Panel1.SuspendLayout();
            this.ContainerList.Panel2.SuspendLayout();
            this.ContainerList.SuspendLayout();
            this.PageSettings.SuspendLayout();
            this.PanelSettings.SuspendLayout();
            this.PageAbout.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuConfig
            // 
            this.MenuConfig.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.MenuConfig.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemReset});
            this.MenuConfig.Name = "menuConfig";
            this.MenuConfig.Size = new System.Drawing.Size(131, 26);
            // 
            // ToolStripMenuItemReset
            // 
            this.ToolStripMenuItemReset.Name = "ToolStripMenuItemReset";
            this.ToolStripMenuItemReset.Size = new System.Drawing.Size(130, 22);
            this.ToolStripMenuItemReset.Text = "Reset Field";
            this.ToolStripMenuItemReset.Click += new System.EventHandler(this.ToolStripMenuItemReset_Click);
            // 
            // MenuMods
            // 
            this.MenuMods.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.MenuMods.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFolderToolStripMenuItem,
            this.deleteModToolStripMenuItem});
            this.MenuMods.Name = "MenuMods";
            this.MenuMods.Size = new System.Drawing.Size(140, 48);
            // 
            // openFolderToolStripMenuItem
            // 
            this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            this.openFolderToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.openFolderToolStripMenuItem.Text = "Open Folder";
            this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.openFolderToolStripMenuItem_Click);
            // 
            // deleteModToolStripMenuItem
            // 
            this.deleteModToolStripMenuItem.Name = "deleteModToolStripMenuItem";
            this.deleteModToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.deleteModToolStripMenuItem.Text = "Delete Mod";
            this.deleteModToolStripMenuItem.Click += new System.EventHandler(this.deleteModToolStripMenuItem_Click);
            // 
            // TabControl
            // 
            this.TabControl.Controls.Add(this.PageMods);
            this.TabControl.Controls.Add(this.PageSettings);
            this.TabControl.Controls.Add(this.PageAbout);
            this.TabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl.ItemSize = new System.Drawing.Size(64, 24);
            this.TabControl.Location = new System.Drawing.Point(0, 0);
            this.TabControl.Margin = new System.Windows.Forms.Padding(0);
            this.TabControl.Name = "TabControl";
            this.TabControl.Offset = new System.Drawing.Point(4, 4);
            this.TabControl.Padding = new System.Drawing.Point(2, 2);
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(627, 449);
            this.TabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.TabControl.TabBackColor = System.Drawing.SystemColors.Control;
            this.TabControl.TabColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.TabControl.TabIndex = 6;
            this.TabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);
            // 
            // PageMods
            // 
            this.PageMods.Controls.Add(this.ContainerMain);
            this.PageMods.Controls.Add(this.BtnRemove);
            this.PageMods.Controls.Add(this.BtnDiscord);
            this.PageMods.Controls.Add(this.BtnSaveAndPlay);
            this.PageMods.Controls.Add(this.BtnSave);
            this.PageMods.Controls.Add(this.BtnPlay);
            this.PageMods.Location = new System.Drawing.Point(4, 28);
            this.PageMods.Margin = new System.Windows.Forms.Padding(2);
            this.PageMods.Name = "PageMods";
            this.PageMods.Padding = new System.Windows.Forms.Padding(2);
            this.PageMods.Size = new System.Drawing.Size(619, 417);
            this.PageMods.TabIndex = 0;
            this.PageMods.Text = "Mods";
            // 
            // ContainerMain
            // 
            this.ContainerMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ContainerMain.Location = new System.Drawing.Point(7, 6);
            this.ContainerMain.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.ContainerMain.Name = "ContainerMain";
            // 
            // ContainerMain.Panel1
            // 
            this.ContainerMain.Panel1.Controls.Add(this.ContainerList);
            this.ContainerMain.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ContainerMain.Panel1MinSize = 128;
            // 
            // ContainerMain.Panel2
            // 
            this.ContainerMain.Panel2.AutoScroll = true;
            this.ContainerMain.Panel2.Controls.Add(this.GridConfig);
            this.ContainerMain.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ContainerMain.Panel2MinSize = 128;
            this.ContainerMain.Size = new System.Drawing.Size(606, 339);
            this.ContainerMain.SplitterDistance = 409;
            this.ContainerMain.SplitterIncrement = 8;
            this.ContainerMain.SplitterWidth = 3;
            this.ContainerMain.TabIndex = 0;
            // 
            // ContainerList
            // 
            this.ContainerList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ContainerList.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.ContainerList.Location = new System.Drawing.Point(0, 0);
            this.ContainerList.Margin = new System.Windows.Forms.Padding(2);
            this.ContainerList.Name = "ContainerList";
            this.ContainerList.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // ContainerList.Panel1
            // 
            this.ContainerList.Panel1.Controls.Add(this.ListMods);
            // 
            // ContainerList.Panel2
            // 
            this.ContainerList.Panel2.Controls.Add(this.LabelTitle);
            this.ContainerList.Panel2.Controls.Add(this.TextDescription);
            this.ContainerList.Size = new System.Drawing.Size(409, 339);
            this.ContainerList.SplitterDistance = 265;
            this.ContainerList.SplitterWidth = 3;
            this.ContainerList.TabIndex = 0;
            // 
            // ListMods
            // 
            this.ListMods.CheckBoxes = true;
            this.ListMods.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColName,
            this.ColVersion,
            this.ColAuthor});
            this.ListMods.ContextMenuStrip = this.MenuMods;
            this.ListMods.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListMods.FullRowSelect = true;
            this.ListMods.GridLines = true;
            this.ListMods.HideSelection = false;
            this.ListMods.Location = new System.Drawing.Point(0, 0);
            this.ListMods.Margin = new System.Windows.Forms.Padding(2);
            this.ListMods.Name = "ListMods";
            this.ListMods.Size = new System.Drawing.Size(409, 265);
            this.ListMods.TabIndex = 9;
            this.ListMods.UseCompatibleStateImageBehavior = false;
            this.ListMods.View = System.Windows.Forms.View.Details;
            this.ListMods.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.ListMods_ItemChecked);
            this.ListMods.SelectedIndexChanged += new System.EventHandler(this.ListMods_SelectedIndexChanged);
            // 
            // ColName
            // 
            this.ColName.Text = "Name";
            this.ColName.Width = 316;
            // 
            // ColVersion
            // 
            this.ColVersion.Text = "Version";
            this.ColVersion.Width = 64;
            // 
            // ColAuthor
            // 
            this.ColAuthor.Text = "Author";
            this.ColAuthor.Width = 128;
            // 
            // LabelTitle
            // 
            this.LabelTitle.AutoSize = true;
            this.LabelTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelTitle.Location = new System.Drawing.Point(4, 2);
            this.LabelTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelTitle.Name = "LabelTitle";
            this.LabelTitle.Size = new System.Drawing.Size(90, 15);
            this.LabelTitle.TabIndex = 1;
            this.LabelTitle.TabStop = true;
            this.LabelTitle.Text = "Mod by Author";
            this.LabelTitle.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LabelTitle_LinkClicked);
            // 
            // TextDescription
            // 
            this.TextDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextDescription.Location = new System.Drawing.Point(0, 0);
            this.TextDescription.Margin = new System.Windows.Forms.Padding(2);
            this.TextDescription.Multiline = true;
            this.TextDescription.Name = "TextDescription";
            this.TextDescription.ReadOnly = true;
            this.TextDescription.Size = new System.Drawing.Size(409, 71);
            this.TextDescription.TabIndex = 0;
            // 
            // GridConfig
            // 
            this.GridConfig.ContextMenuStrip = this.MenuConfig;
            this.GridConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridConfig.Location = new System.Drawing.Point(0, 0);
            this.GridConfig.Margin = new System.Windows.Forms.Padding(2);
            this.GridConfig.Name = "GridConfig";
            this.GridConfig.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.GridConfig.Size = new System.Drawing.Size(194, 339);
            this.GridConfig.TabIndex = 0;
            // 
            // BtnRemove
            // 
            this.BtnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnRemove.Location = new System.Drawing.Point(8, 382);
            this.BtnRemove.Name = "BtnRemove";
            this.BtnRemove.Size = new System.Drawing.Size(102, 26);
            this.BtnRemove.TabIndex = 11;
            this.BtnRemove.Text = "Remove Mod";
            this.BtnRemove.Click += new System.EventHandler(this.BtnRemove_Click);
            // 
            // BtnDiscord
            // 
            this.BtnDiscord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnDiscord.Location = new System.Drawing.Point(7, 351);
            this.BtnDiscord.Name = "BtnDiscord";
            this.BtnDiscord.Size = new System.Drawing.Size(102, 26);
            this.BtnDiscord.TabIndex = 10;
            this.BtnDiscord.Text = "Join Discord";
            this.BtnDiscord.Click += new System.EventHandler(this.BtnDiscord_Click);
            // 
            // BtnSaveAndPlay
            // 
            this.BtnSaveAndPlay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSaveAndPlay.Location = new System.Drawing.Point(116, 351);
            this.BtnSaveAndPlay.Name = "BtnSaveAndPlay";
            this.BtnSaveAndPlay.Size = new System.Drawing.Size(388, 58);
            this.BtnSaveAndPlay.TabIndex = 9;
            this.BtnSaveAndPlay.Text = "Save and Play";
            this.BtnSaveAndPlay.Click += new System.EventHandler(this.BtnSaveAndPlay_Click);
            // 
            // BtnSave
            // 
            this.BtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSave.Location = new System.Drawing.Point(511, 351);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(102, 26);
            this.BtnSave.TabIndex = 8;
            this.BtnSave.Text = "Save";
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // BtnPlay
            // 
            this.BtnPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnPlay.Location = new System.Drawing.Point(511, 383);
            this.BtnPlay.Name = "BtnPlay";
            this.BtnPlay.Size = new System.Drawing.Size(102, 26);
            this.BtnPlay.TabIndex = 6;
            this.BtnPlay.Text = "Play";
            this.BtnPlay.Click += new System.EventHandler(this.BtnPlay_Click);
            // 
            // PageSettings
            // 
            this.PageSettings.Controls.Add(this.BtnSave2);
            this.PageSettings.Controls.Add(this.PanelSettings);
            this.PageSettings.Location = new System.Drawing.Point(4, 28);
            this.PageSettings.Margin = new System.Windows.Forms.Padding(2);
            this.PageSettings.Name = "PageSettings";
            this.PageSettings.Padding = new System.Windows.Forms.Padding(2);
            this.PageSettings.Size = new System.Drawing.Size(619, 417);
            this.PageSettings.TabIndex = 1;
            this.PageSettings.Text = "Settings";
            // 
            // BtnSave2
            // 
            this.BtnSave2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSave2.Location = new System.Drawing.Point(510, 383);
            this.BtnSave2.Name = "BtnSave2";
            this.BtnSave2.Size = new System.Drawing.Size(103, 26);
            this.BtnSave2.TabIndex = 9;
            this.BtnSave2.Text = "Save";
            this.BtnSave2.Click += new System.EventHandler(this.BtnSave2_Click);
            // 
            // PanelSettings
            // 
            this.PanelSettings.Controls.Add(this.LabelGame);
            this.PanelSettings.Controls.Add(this.ComboGames);
            this.PanelSettings.Controls.Add(this.CheckConsole);
            this.PanelSettings.Controls.Add(this.CheckSpeedrun);
            this.PanelSettings.Controls.Add(this.label1);
            this.PanelSettings.Controls.Add(this.CheckOneClick);
            this.PanelSettings.Controls.Add(this.OneClickLabel);
            this.PanelSettings.Controls.Add(this.CheckFastRestart);
            this.PanelSettings.Controls.Add(this.label2);
            this.PanelSettings.Controls.Add(this.SaveModeCheckbox);
            this.PanelSettings.Controls.Add(this.SaveModeLabel);
            this.PanelSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelSettings.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.PanelSettings.Location = new System.Drawing.Point(2, 2);
            this.PanelSettings.Margin = new System.Windows.Forms.Padding(6);
            this.PanelSettings.Name = "PanelSettings";
            this.PanelSettings.Size = new System.Drawing.Size(615, 413);
            this.PanelSettings.TabIndex = 0;
            // 
            // LabelGame
            // 
            this.LabelGame.AutoSize = true;
            this.LabelGame.Location = new System.Drawing.Point(0, 6);
            this.LabelGame.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.LabelGame.Name = "LabelGame";
            this.LabelGame.Size = new System.Drawing.Size(84, 15);
            this.LabelGame.TabIndex = 1;
            this.LabelGame.Text = "Current Game:";
            // 
            // ComboGames
            // 
            this.ComboGames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboGames.FormattingEnabled = true;
            this.ComboGames.Location = new System.Drawing.Point(3, 27);
            this.ComboGames.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.ComboGames.Name = "ComboGames";
            this.ComboGames.Size = new System.Drawing.Size(257, 23);
            this.ComboGames.TabIndex = 0;
            // 
            // CheckConsole
            // 
            this.CheckConsole.AutoSize = true;
            this.CheckConsole.Checked = true;
            this.CheckConsole.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckConsole.Location = new System.Drawing.Point(3, 53);
            this.CheckConsole.Name = "CheckConsole";
            this.CheckConsole.Size = new System.Drawing.Size(164, 19);
            this.CheckConsole.TabIndex = 3;
            this.CheckConsole.Text = "Show the console window";
            this.CheckConsole.UseVisualStyleBackColor = true;
            // 
            // CheckSpeedrun
            // 
            this.CheckSpeedrun.AutoSize = true;
            this.CheckSpeedrun.Location = new System.Drawing.Point(3, 78);
            this.CheckSpeedrun.Name = "CheckSpeedrun";
            this.CheckSpeedrun.Size = new System.Drawing.Size(110, 19);
            this.CheckSpeedrun.TabIndex = 2;
            this.CheckSpeedrun.Text = "Speedrun Mode";
            this.CheckSpeedrun.UseVisualStyleBackColor = true;
            this.CheckSpeedrun.CheckedChanged += new System.EventHandler(this.CheckSpeedrun_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 100);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(540, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "(Enables the leaderboards, but only allows whitelisted mods, and displays active " +
    "mods on the screen.)";
            // 
            // CheckOneClick
            // 
            this.CheckOneClick.AutoSize = true;
            this.CheckOneClick.CausesValidation = false;
            this.CheckOneClick.Location = new System.Drawing.Point(3, 118);
            this.CheckOneClick.Name = "CheckOneClick";
            this.CheckOneClick.Size = new System.Drawing.Size(117, 19);
            this.CheckOneClick.TabIndex = 7;
            this.CheckOneClick.Text = "Enable One-Click";
            this.CheckOneClick.UseVisualStyleBackColor = true;
            // 
            // OneClickLabel
            // 
            this.OneClickLabel.Location = new System.Drawing.Point(3, 140);
            this.OneClickLabel.Name = "OneClickLabel";
            this.OneClickLabel.Size = new System.Drawing.Size(257, 23);
            this.OneClickLabel.TabIndex = 8;
            this.OneClickLabel.Text = "Enables One-Click support on GameBanana!";
            // 
            // CheckFastRestart
            // 
            this.CheckFastRestart.AutoSize = true;
            this.CheckFastRestart.Location = new System.Drawing.Point(3, 166);
            this.CheckFastRestart.Name = "CheckFastRestart";
            this.CheckFastRestart.Size = new System.Drawing.Size(262, 19);
            this.CheckFastRestart.TabIndex = 4;
            this.CheckFastRestart.Text = "Add Fast Restart Keybind for Speedrun Mode";
            this.CheckFastRestart.UseVisualStyleBackColor = true;
            this.CheckFastRestart.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(3, 188);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(574, 23);
            this.label2.TabIndex = 5;
            this.label2.Text = "Pressing F11 while the game is open will fast restart the game, toggling Speedrun" +
    " Mode in the process.";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // PageAbout
            // 
            this.PageAbout.BackColor = System.Drawing.SystemColors.Control;
            this.PageAbout.Controls.Add(this.TextInfo);
            this.PageAbout.Controls.Add(this.LabelAboutVersion);
            this.PageAbout.Controls.Add(this.LabelAboutTitle);
            this.PageAbout.Location = new System.Drawing.Point(4, 28);
            this.PageAbout.Margin = new System.Windows.Forms.Padding(2);
            this.PageAbout.Name = "PageAbout";
            this.PageAbout.Size = new System.Drawing.Size(619, 417);
            this.PageAbout.TabIndex = 2;
            this.PageAbout.Text = "About";
            // 
            // TextInfo
            // 
            this.TextInfo.Location = new System.Drawing.Point(7, 124);
            this.TextInfo.Multiline = true;
            this.TextInfo.Name = "TextInfo";
            this.TextInfo.ReadOnly = true;
            this.TextInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TextInfo.Size = new System.Drawing.Size(606, 283);
            this.TextInfo.TabIndex = 5;
            this.TextInfo.Text = resources.GetString("TextInfo.Text");
            // 
            // LabelAboutVersion
            // 
            this.LabelAboutVersion.AutoSize = true;
            this.LabelAboutVersion.Font = new System.Drawing.Font("Segoe UI Light", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelAboutVersion.Location = new System.Drawing.Point(9, 71);
            this.LabelAboutVersion.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.LabelAboutVersion.Name = "LabelAboutVersion";
            this.LabelAboutVersion.Size = new System.Drawing.Size(117, 45);
            this.LabelAboutVersion.TabIndex = 4;
            this.LabelAboutVersion.Text = "Version";
            // 
            // LabelAboutTitle
            // 
            this.LabelAboutTitle.AutoSize = true;
            this.LabelAboutTitle.Font = new System.Drawing.Font("Segoe UI Black", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelAboutTitle.Location = new System.Drawing.Point(0, 6);
            this.LabelAboutTitle.Margin = new System.Windows.Forms.Padding(0);
            this.LabelAboutTitle.Name = "LabelAboutTitle";
            this.LabelAboutTitle.Size = new System.Drawing.Size(520, 65);
            this.LabelAboutTitle.TabIndex = 3;
            this.LabelAboutTitle.Text = "BananaModManager";
            // 
            // SaveModeCheckbox
            // 
            this.SaveModeCheckbox.AutoSize = true;
            this.SaveModeCheckbox.Location = new System.Drawing.Point(3, 214);
            this.SaveModeCheckbox.Name = "SaveModeCheckbox";
            this.SaveModeCheckbox.Size = new System.Drawing.Size(84, 19);
            this.SaveModeCheckbox.TabIndex = 9;
            this.SaveModeCheckbox.Text = "Save Mode";
            this.SaveModeCheckbox.UseVisualStyleBackColor = true;
            // 
            // SaveModeLabel
            // 
            this.SaveModeLabel.AutoSize = true;
            this.SaveModeLabel.Location = new System.Drawing.Point(3, 236);
            this.SaveModeLabel.Name = "SaveModeLabel";
            this.SaveModeLabel.Size = new System.Drawing.Size(591, 15);
            this.SaveModeLabel.TabIndex = 10;
            this.SaveModeLabel.Text = "When enabled the game will save your best clears of each stage. When disabled it " +
    "will not save your best clears.";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(627, 449);
            this.Controls.Add(this.TabControl);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(635, 461);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BananaModManager";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MenuConfig.ResumeLayout(false);
            this.MenuMods.ResumeLayout(false);
            this.TabControl.ResumeLayout(false);
            this.PageMods.ResumeLayout(false);
            this.ContainerMain.Panel1.ResumeLayout(false);
            this.ContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ContainerMain)).EndInit();
            this.ContainerMain.ResumeLayout(false);
            this.ContainerList.Panel1.ResumeLayout(false);
            this.ContainerList.Panel2.ResumeLayout(false);
            this.ContainerList.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ContainerList)).EndInit();
            this.ContainerList.ResumeLayout(false);
            this.PageSettings.ResumeLayout(false);
            this.PanelSettings.ResumeLayout(false);
            this.PanelSettings.PerformLayout();
            this.PageAbout.ResumeLayout(false);
            this.PageAbout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.TabControl TabControl;
        private System.Windows.Forms.TabPage PageMods;
        private System.Windows.Forms.Button BtnRemove;
        private System.Windows.Forms.Button BtnDiscord;
        private System.Windows.Forms.Button BtnSaveAndPlay;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.Button BtnPlay;
        private System.Windows.Forms.TabPage PageSettings;
        private System.Windows.Forms.FlowLayoutPanel PanelSettings;
        private System.Windows.Forms.ComboBox ComboGames;
        private System.Windows.Forms.Label LabelGame;
        private System.Windows.Forms.SplitContainer ContainerMain;
        private PropertyGrid GridConfig;
        private System.Windows.Forms.ContextMenuStrip MenuConfig;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemReset;
        private System.Windows.Forms.SplitContainer ContainerList;
        private ListView ListMods;
        private System.Windows.Forms.ColumnHeader ColName;
        private System.Windows.Forms.ColumnHeader ColVersion;
        private System.Windows.Forms.ColumnHeader ColAuthor;
        private System.Windows.Forms.LinkLabel LabelTitle;
        private System.Windows.Forms.TextBox TextDescription;
        private System.Windows.Forms.TabPage PageAbout;
        private System.Windows.Forms.TextBox TextInfo;
        private System.Windows.Forms.Label LabelAboutVersion;
        private System.Windows.Forms.Label LabelAboutTitle;
        private System.Windows.Forms.ContextMenuStrip MenuMods;
        private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteModToolStripMenuItem;
        private System.Windows.Forms.CheckBox CheckConsole;
        private System.Windows.Forms.CheckBox CheckSpeedrun;
        private System.Windows.Forms.CheckBox CheckOneClick;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label OneClickLabel;
        private System.Windows.Forms.Button BtnSave2;
        private System.Windows.Forms.CheckBox CheckFastRestart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox SaveModeCheckbox;
        private System.Windows.Forms.Label SaveModeLabel;
    }
}

