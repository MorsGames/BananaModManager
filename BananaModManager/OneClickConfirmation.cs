using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BananaModManager
{
    public partial class OneClickConfirmation : Form
    {
        public static string passedUrl = "";
        public static string passedID = "";
        public OneClickConfirmation(string downloadURL, string modID)
        {
            passedUrl = downloadURL;
            passedID = modID;
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
        }

        private void OneClickConfirmation_Load(object sender, EventArgs e)
        {

        }

        private void ModLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try 
            {
                VisitLink("https://gamebanana.com/mods/" + passedID);
            }
            catch
            {
                MessageBox.Show("Invalid URL! Please try again with a valid GameBanana link.");
            }
        }
        private void VisitLink(string GameBananaURL)
        {
            ModLink.LinkVisited = true;
            System.Diagnostics.Process.Start(GameBananaURL);  
        }

        private void ConfirmInstall_Click(object sender, EventArgs e)
        {
            GameBanana.InstallMod(passedUrl, passedID);
        }

        private void DeconfirmInstall_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

}
