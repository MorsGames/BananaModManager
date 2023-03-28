using System;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace BananaModManager
{
    partial class OneClickConfirmation
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        /// 
            public static string GetPageTitle(string link)
        {
            try
            {
                WebClient wc = new WebClient();
                string html = wc.DownloadString(link);

                Regex x = new Regex("<title>(.*)</title>");
                MatchCollection m = x.Matches(html);

                if (m.Count > 0)
                {
                    return m[0].Value.Replace("<title>", "").Replace("</title>", "");
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not connect. Error:" + ex.Message);
                return "";
            }
        }
        private void InitializeComponent()
        {
            this.Icon = Properties.Resources.ProgramIcon;
            this.ModLink = new System.Windows.Forms.LinkLabel();
            this.InstallText = new System.Windows.Forms.Label();
            this.ConfirmInstall = new System.Windows.Forms.Button();
            this.DeconfirmInstall = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ModLink
            // 
            this.ModLink.AutoSize = false;
            this.ModLink.Location = new System.Drawing.Point(191, 40);
            this.ModLink.Name = "ModLink";
            this.ModLink.Size = new System.Drawing.Size(91, 13);
            this.ModLink.TabIndex = 0;
            this.ModLink.TabStop = true;
            this.ModLink.Text = GetPageTitle("https://gamebanana.com/mods/" + passedID).Remove(GetPageTitle("https://gamebanana.com/mods/" + passedID).Length - 40, 40);
            this.ModLink.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ModLink.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ModLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ModLink_LinkClicked);
            // 
            // InstallText
            // 
            this.InstallText.AutoSize = true;
            this.InstallText.Location = new System.Drawing.Point(130, 20);
            this.InstallText.Name = "InstallText";
            this.InstallText.Size = new System.Drawing.Size(91, 13);
            this.InstallText.TabIndex = 1;
            this.InstallText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.InstallText.Text = "Would you like to install this Mod?";
            // 
            // ConfirmInstall
            // 
            this.ConfirmInstall.Location = new System.Drawing.Point(97, 100);
            this.ConfirmInstall.Name = "ConfirmInstall";
            this.ConfirmInstall.Size = new System.Drawing.Size(75, 23);
            this.ConfirmInstall.TabIndex = 2;
            this.ConfirmInstall.Text = "Install";
            this.ConfirmInstall.UseVisualStyleBackColor = true;
            this.ConfirmInstall.Click += new System.EventHandler(this.ConfirmInstall_Click);
            // 
            // DeconfirmInstall
            // 
            this.DeconfirmInstall.Location = new System.Drawing.Point(257, 100);
            this.DeconfirmInstall.Name = "DeconfirmInstall";
            this.DeconfirmInstall.Size = new System.Drawing.Size(75, 23);
            this.DeconfirmInstall.TabIndex = 3;
            this.DeconfirmInstall.Text = "Close";
            this.DeconfirmInstall.UseVisualStyleBackColor = true;
            this.DeconfirmInstall.Click += new System.EventHandler(this.DeconfirmInstall_Click);
            // 
            // OneClickConfirmation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 148);
            this.Controls.Add(this.DeconfirmInstall);
            this.Controls.Add(this.ConfirmInstall);
            this.Controls.Add(this.InstallText);
            this.Controls.Add(this.ModLink);
            this.Name = "OneClickConfirmation";
            this.Text = "One-Click Mod Install";
            this.Load += new System.EventHandler(this.OneClickConfirmation_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel ModLink;
        private System.Windows.Forms.Label InstallText;
        private System.Windows.Forms.Button ConfirmInstall;
        private System.Windows.Forms.Button DeconfirmInstall;
    }
}