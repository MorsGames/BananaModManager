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
        public OneClickConfirmation(string modURL)
        {
            passedUrl = modURL;
            InitializeComponent();
        }

        private void OneClickConfirmation_Load(object sender, EventArgs e)
        {

        }

        private void ModLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try 
            {
                VisitLink(passedUrl);
            }
            catch (Exception ex)
            {

            }
        }
        private void VisitLink(string modURL)
        {
            ModLink.LinkVisited = true;
            System.Diagnostics.Process.Start(modURL);  
        }

        private void ConfirmInstall_Click(object sender, EventArgs e)
        {
            GameBanana.ModDownload(passedUrl);
        }

        private void DeconfirmInstall_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

}
