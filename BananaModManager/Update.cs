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

        public static void UpdateSpeedrunLegalMods()
        {
            bool installAll = false;
            if (MessageBox.Show("Would you like to install all other Speedrun-legal mods in addition to updating the mods you already have installed?", "Install New Mods?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                installAll = true;
            }
            using (WebClient wc = new WebClient())
            {
                try
                {
                    // Get the latest release info from GitHub's API
                    wc.Headers.Add("user-agent", "request");
                    var jsondata = wc.DownloadString(new System.Uri("https://api.github.com/repos/MorsGames/BananaModManager/releases/latest"));
                    Release parsedJson = JsonConvert.DeserializeObject<Release>(jsondata);
                    // Delete leftover if it's there for some reason
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\LegalMods.zip"))
                    {
                        File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\LegalMods.zip");
                    }
                    wc.DownloadFile(parsedJson.assets[1].browser_download_url, AppDomain.CurrentDomain.BaseDirectory + "\\LegalMods.zip");

                    // Get path of mods folder and whitelisted mods list
                    string pathModsFolder = AppDomain.CurrentDomain.BaseDirectory + "mods\\";
                    List<string> whitelistedMods = Games.BananaMania.WhitelistNames;

                    // Prep objects for handling the zip file
                    ZipArchive zipArchive = ZipFile.OpenRead(AppDomain.CurrentDomain.BaseDirectory + "\\LegalMods.zip");

                    foreach (string modDLL in whitelistedMods)
                    {
                        string[] pathInstalledModLocation = Directory.GetFiles(pathModsFolder, modDLL, SearchOption.AllDirectories);
                        // If the mod was already installed
                        if (pathInstalledModLocation.Length > 0)
                        {
                            // Cut down the full path to the DLL to just be the path to the mod folder
                            string pathInstalledModFolder = pathInstalledModLocation[0].Substring(0, (pathInstalledModLocation[0].Length - modDLL.Length));
                            DirectoryInfo directoryInfo = new DirectoryInfo(pathInstalledModFolder);
                            // Clear out the old directory
                            foreach (FileInfo file in directoryInfo.GetFiles())
                            {
                                // file.Delete();
                            }
                            // Find the equivalent mod folder in the downloaded zip
                            string path = null;
                            foreach (ZipArchiveEntry entry in zipArchive.Entries)
                            {
                                if (entry.Name.Contains(modDLL))
                                {
                                   path = entry.FullName.Substring(0, entry.FullName.Length - modDLL.Length);
                                }
                            }
                            // Iterate over every file in the found folder, overwrite if it wasn't deleted.
                            foreach (ZipArchiveEntry entry in zipArchive.Entries)
                            {
                                if (entry.FullName.Contains(path) && entry.FullName.Substring(path.Length - 1 ) == path && !entry.FullName.EndsWith("/"))
                                {
                                    string fixedpath = (pathInstalledModFolder.Substring(0, pathInstalledModFolder.Length - 1) + entry.FullName.Substring(path.Length - 1).Replace('/', '\\'));
                                    entry.ExtractToFile(fixedpath, true);
                                }
                            }

                        }
                        // If the mod was not already installed
                        else
                        {
                            if (installAll)
                            {
                                string path = null;
                                foreach (ZipArchiveEntry entry in zipArchive.Entries)
                                {
                                    if (entry.Name.Contains(modDLL))
                                    {
                                        path = entry.FullName.Substring(0, entry.FullName.Length - modDLL.Length);
                                    }
                                }
                                foreach (ZipArchiveEntry entry in zipArchive.Entries)
                                {
                                    if (entry.FullName.Contains(path) && entry.FullName.EndsWith("/"))
                                    {
                                        Directory.CreateDirectory((pathModsFolder + entry.FullName).Replace('/', '\\'));
                                    }
                                    if (entry.FullName.Contains(path) && !entry.FullName.EndsWith("/"))
                                    {
                                        entry.ExtractToFile((pathModsFolder + entry.FullName).Replace('/', '\\'), true);
                                    }
                                }
                            }
                        }
                    }
                    MessageBox.Show("Successfully updated all mods!");
                    zipArchive.Dispose();
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\LegalMods.zip");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
        }
    }
}
