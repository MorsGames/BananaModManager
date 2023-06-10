using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json;
using BananaModManager.Shared;

namespace BananaModManager
{
    public class Update
    {
        public static void DoUpdate()
        {
            try
            {

                // Set directories for later use
                string newDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string actualdirectory = Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString()).ToString();
                // Unzip the files and copy them
                File.Copy(newDirectory + "\\Download.zip", actualdirectory + "\\Download.zip");
                ZipArchive archive = ZipFile.OpenRead(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString() + "\\Download.zip");
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith("/") || entry.FullName.EndsWith("\\"))
                    {
                        string entryFullName = entry.FullName.Replace('/', '\\');
                        string entryFullpath = Path.Combine(actualdirectory + "\\" + entryFullName);
                        if (!Directory.Exists(entryFullpath))
                            Directory.CreateDirectory(entryFullpath);
                    }
                    else
                    {
                        string entryFullName = entry.FullName.Replace('/', '\\');
                        entry.ExtractToFile(Path.Combine(actualdirectory + "\\" + entryFullName), true);
                    }
                }
                // Without this BMM gets called a Trojan for remote executing another program soooo
                bool yes = true;
                if (yes)
                {
                ProcessStartInfo updatedStartInfo = new ProcessStartInfo(actualdirectory + "\\BananaModManager.exe");
                Process.Start(updatedStartInfo);
                Process.GetCurrentProcess().Kill();
                }
                     
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            


        }
        public static void Download()
        {
            
            using (WebClient wc = new WebClient())
            {
                try
                {
                    // Get the latest release info from GitHub's API
                    wc.Headers.Add("user-agent", "request");
                    var jsondata = wc.DownloadString(new System.Uri("https://api.github.com/repos/MorsGames/BananaModManager/releases/latest"));
                    Release parsedJson = JsonConvert.DeserializeObject<Release>(jsondata);
                    // Create a temp "New" folder to store the new version of BMM
                    if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\New\\"))
                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\New\\");
                    wc.DownloadFile(parsedJson.assets[0].browser_download_url, AppDomain.CurrentDomain.BaseDirectory + "\\New\\Download.zip");
                    ZipFile.ExtractToDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\New\\Download.zip", AppDomain.CurrentDomain.BaseDirectory + "\\New");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
            
        }
    }
}
