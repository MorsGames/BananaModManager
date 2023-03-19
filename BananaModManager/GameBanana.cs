using System;
using System.IO.Compression;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Text;
using System.Net;

namespace BananaModManager
{
    public static class GameBanana
    {
        public static Dictionary<string, string> parsedJson;
        // Upon launching the Mod Manager, this enables one-click capability.
        public static bool InstallOneClick()
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
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void ParseData(string gbID)
        {
            // Use Mod ID to find files 
            string urlRequest = $"https://api.gamebanana.com/Core/Item/Data?itemtype=Mod&itemid={gbID}&fields=Files().aFiles()&format=json_min&flags=JSON_UNESCAPED_SLASHES";
            using (WebClient wc = new WebClient())
            {
                var jsondata = wc.DownloadString(urlRequest);
                //fix the formatting so it can be parsed
                jsondata = jsondata.Remove(0, 11);
                jsondata = jsondata.Remove(jsondata.Length - 2, 2);
                parsedJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsondata);
            }


            WebClient x = new WebClient();
        }

        public static void DownloadArchive(Dictionary<string, string> modInfo)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in modInfo)
            {
                sb.AppendFormat("{0} - {1}{2}", item.Key, item.Value, Environment.NewLine);
            }

            string downloadlink = modInfo["_sDownloadUrl"];
            string fileName = modInfo["_sFile"];
            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadFile(downloadlink, AppDomain.CurrentDomain.BaseDirectory + "\\mods\\" + fileName);
                    ZipFile.ExtractToDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\mods\\" + fileName, AppDomain.CurrentDomain.BaseDirectory + "\\mods\\");
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\mods\\" + fileName);
                    MessageBox.Show("Success!");
                }
                catch (IOException)
                {
                    MessageBox.Show("The mod is already installed! Updating Mods will come soon...");
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\mods\\" + fileName))
                    {
                        File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\mods\\" + fileName);
                    }
                }
            }
        }

        public static void ModDownload(string gamebananaUrl)
        {
            string modID = gamebananaUrl.Substring(gamebananaUrl.Length - 6);
            ParseData(modID);
            DownloadArchive(parsedJson);
        }
    }
}
