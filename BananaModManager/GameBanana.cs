using System;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


using BananaModManager;

namespace BananaModManager
{

    public static class GameBanana
    {

        public static string GetModTitle(string link)
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
                Console.WriteLine("Could not connect. Error:" + ex.Message);
                return "";
            }
        }

        // Upon launching the Mod Manager, this enables one-click capability.
        public static void InstallOneClick()
        {
            string ExeDirectory = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".exe");
            string protocol = $"bananamodmanager";
            try
            {
                var reg = Registry.CurrentUser.CreateSubKey(@"Software\Classes\BananaModManager");
                reg.SetValue("", $"URL:{protocol}");
                reg.SetValue("URL Protocol", "");
                reg = reg.CreateSubKey(@"shell\open\command");
                reg.SetValue("", $"\"{ExeDirectory}\" -download \"%1\"");
                reg.Close();
                return;            
            }
            catch
            {
                return;
            }
        }
        public static void DisableOneClick()
        {
            try
            {
               Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\BananaModManager");
            }
            catch
            {
                return;
            }
        }

        public static void InstallMod(string downloadUrl, string modID)
        {
            
            using (var client = new WebClient())
            {
                
                // Isolate the File ID for API usage
                string fileID = downloadUrl.Remove(0, downloadUrl.Length - 6);

                // GameBanana API requests
                string fileName = client.DownloadString($"https://api.gamebanana.com/Core/Item/Data?itemtype=File&itemid={fileID}&fields=file&format=json_min");
                string fileContents = client.DownloadString($"https://api.gamebanana.com/Core/Item/Data?itemtype=File&itemid={fileID}&fields=Metadata%28%29.aArchiveFilesList%28%29&format=json_min&flags=JSON_UNESCAPED_SLASHES");
                

                // Remove the extra json junk
                char[] brackets = { '[', ']', '"'};
                fileContents = fileContents.Trim('[', ']');

                // Sort through the files and find the DLL
                string[] files = fileContents.Split(',');
                string DLL = "";
                foreach (string i in files)
                {
                    string file = "";
                    // If it has a directory, remove it
                    if (i.Contains("/"))
                    {
                        file = i.Substring(i.IndexOf('/') + 1);
                    }
                    // Check the extension
                    if (i.Contains(".dll"))
                    {
                        DLL = file.Remove(file.Length - 1);
                    }

                }
                fileName = fileName.Trim(brackets);

                // Grab the mod name from the title of the main page
                string modName = GetModTitle("https://gamebanana.com/mods/" + modID).Remove(GetModTitle("https://gamebanana.com/mods/" + modID).Length - 40, 40);
                try
                {
                    // Download the zip
                    client.DownloadFile(downloadUrl, AppDomain.CurrentDomain.BaseDirectory + "\\mods\\" + fileName);

                    // Check if the mod is installed and prompt to overwrite
                    if (Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\mods\\", DLL, SearchOption.AllDirectories) != null)
                    {
                        if (MessageBox.Show("It appears you have a version of this mod installed! Would you like to overwrite/update it?", "Overwrite?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            try
                            {
                                string[] path = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\mods\\", DLL, SearchOption.AllDirectories);
                                // Find directory of the DLL regardless of name and delete the contents. 
                                Directory.Delete(path[0].Remove(path[0].Length - DLL.Length, DLL.Length), true);

                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.Message);
                            }
                        }
                    }

                    // If the zip contains a folder with the mod inside, just extract to "smbbm\mods"
                    if (fileContents.Contains("/"))
                    {
                        ZipFile.ExtractToDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\mods\\" + fileName, AppDomain.CurrentDomain.BaseDirectory + "\\mods\\");
                    }
                    // If the zip doesn't contain a folder with the mod inside, make one with the mod name we pulled earlier and extract
                    else
                    {
                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + $"\\mods\\{modName}\\");
                        ZipFile.ExtractToDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\mods\\" + fileName, AppDomain.CurrentDomain.BaseDirectory + $"\\mods\\{modName}\\");
                    }
                    // Remove the zip
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\mods\\" + fileName);
                    // WE DID IT!
                    MessageBox.Show($"\"{modName}\" has been installed! It is recommended you check the newly created directory for a \"README.txt\".\nSometimes mods have additional files that need to be placed elsewhere!", "Success!");
                }
                catch (Exception e)
                {
                    MessageBox.Show($"An error has occured: {e.Message}");
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\mods\\" + fileName))
                    {
                        File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\mods\\" + fileName);
                    }
                }
            }
        }
    }
}
