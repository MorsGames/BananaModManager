using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BananaModManager.Shared;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Win32;

namespace BananaModManager.NewUI;

public static class GameBanana
{
    private static string GetModTitle(string link)
    {
        try
        {
            var wc = new WebClient();
            var html = wc.DownloadString(link);

            var x = new Regex("<title>(.*)</title>");
            var m = x.Matches(html);

            if (m.Count > 0)
            {
                return m[0].Value.Replace("<title>", "").Replace("</title>", "");
            }
            return "";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not connect. Error:{ex.Message}");
            return "";
        }
    }

    // This enables one-click capability by writing registry entries to redirect "bananamodmanager:" links to BMM.
    public static async void InstallOneClick()
    {
        var exeDirectory = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".exe");
        var protocol = $"bananamodmanager";
        try
        {
            var reg = Registry.CurrentUser.CreateSubKey(@"Software\Classes\BananaModManager");
            reg.SetValue("", $"URL:{protocol}");
            reg.SetValue("URL Protocol", "");
            reg = reg.CreateSubKey(@"shell\open\command");
            reg.SetValue("", $"\"{exeDirectory}\" -download \"%1\"");
            reg.Close();

            await ModernMessageBox.Show("GameBanana 1-Click support has been installed!", "Done!");
            return;
        }
        catch
        {
            return;
        }
    }
    // This deletes the registry entries if the user unchecks 1-Click.
    public static async void DisableOneClick()
    {
        try
        {
            Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\BananaModManager");
            await ModernMessageBox.Show("GameBanana 1-Click support has been removed.", "Done!");
        }
        catch
        {
            return;
        }
    }

    public static async Task InstallMod(string downloadUrl, string modID, WebClient client)
    {
        var modsDirectory = App.PathConvert("mods\\");
        using (client)
        {
            var moreFiles = false;

            // Isolate the File ID for API usage
            var fileID = downloadUrl.Split(',');
            fileID[0] = fileID[0].Replace("https://gamebanana.com/mmdl/", "");

            // GameBanana API requests
            // Get the name of the archive
            var fileName = client.DownloadString($"https://api.gamebanana.com/Core/Item/Data?itemtype=File&itemid={fileID[0]}&fields=file&format=json_min");

            // Get the entire archive contents
            var fileContents = client.DownloadString($"https://api.gamebanana.com/Core/Item/Data?itemtype=File&itemid={fileID[0]}&fields=Metadata%28%29.aArchiveFilesList%28%29&format=json_min&flags=JSON_UNESCAPED_SLASHES");

            // Remove the extra json junk
            char[] stuff = {'[', ']', '"', '"', '`', '\'', '"'};
            fileContents = fileContents.Trim(stuff);

            // Sort through the files and find the DLL
            var files = fileContents.Split(',');
            var dll = "";
            var folder = "";
            var dllFolder = "";
            foreach (var i in files)
            {
                var file = i;
                foreach (var character in stuff)
                {
                    file = file.Replace(character.ToString(), "");
                }

                // If it has a directory, remove it
                if (file.Contains('/') || file.Contains ('\\'))
                {
                    foreach (var character in file)
                    {
                        switch (character)
                        {
                            case '\\':
                                folder = file.Substring(0, file.IndexOf('\\'));
                                file = file.Substring(file.IndexOf('\\') + 1);
                                break;
                            case '/':
                                folder = file.Substring(0, file.IndexOf('/'));
                                file = file.Substring(file.IndexOf('/') + 1);
                                break;
                        }
                    }
                }
                // Check the extension
                if (file.Contains(".dll") || file.Contains(".DLL"))
                {
                    dllFolder = folder;
                    dll = file;
                }
                if (!file.Contains(".dll") && !file.Contains(".DLL") && !file.Contains(".json") && !file.Contains(".JSON"))
                {
                    moreFiles = true;
                }
            }
            foreach(var character in stuff)
            {
                fileName = fileName.Replace(character.ToString(), "");
            }
            var modName = GetModName(modID, client);

            try
            {
                var fullPath = Path.Combine(modsDirectory, fileName);

                // Check that the zip hasn't already been downloaded
                if (Directory.GetFiles(modsDirectory, fileName).Length > 0)
                {
                    File.Delete(fullPath);
                }
                // Download the zip
                client.DownloadFile(downloadUrl, fullPath);
                // Check if the mod is installed and prompt to overwrite
                var existingVersions = Directory.GetFiles(modsDirectory, dll, SearchOption.AllDirectories);

                if (existingVersions.Length != 0)
                {
                    if (await ModernMessageBox.Show("It appears you have a version of this mod installed! Would you like to overwrite or update it?", "Overwrite?", "Yes", "No") == ContentDialogResult.Primary)
                    {
                        try
                        {
                            // Find directory of the DLL regardless of folder name and delete the contents.
                            var path = Directory.GetFiles(modsDirectory, dll, SearchOption.AllDirectories);
                            Directory.Delete(path[0].Remove(path[0].Length - dll.Length, dll.Length), true);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    }
                    // If the user says no, just delete the downloaded zip.
                    else
                    {
                        if (File.Exists(fullPath))
                        {
                            File.Delete(fullPath);
                        }
                        return;
                    }
                }
                // Create the directory for the mod
                else
                {
                    Directory.CreateDirectory(Path.Combine(modsDirectory, modName));
                }
                // Check for only DLL files and json files
                var zip = ZipFile.OpenRead(fullPath);
                foreach (var entry in zip.Entries)
                {
                    // Extract from the zip
                    if (!File.Exists($"{modsDirectory}{modName}\\{entry.Name}") && !entry.FullName.EndsWith("/"))
                    {
                        // Check if there's an extra folder in the file's path
                        if (dllFolder != "" && entry.FullName.Contains(dllFolder))
                        {
                            // Check if the directory needs to be created
                            if (entry.FullName.Contains('/') || entry.FullName.Contains('\\') && !entry.FullName.EndsWith("/"))
                            {
                                if (!Directory.Exists($"{modsDirectory}{modName}\\{Path.GetDirectoryName(entry.FullName.Substring(dllFolder.Length + 1))}"))
                                {
                                    Directory.CreateDirectory($"{modsDirectory}{modName}\\{Path.GetDirectoryName(entry.FullName.Substring(dllFolder.Length + 1))}");
                                }
                            }
                            // If it's not a folder, extract it
                            if (!entry.FullName.EndsWith("/"))
                            {
                                entry.ExtractToFile($"{modsDirectory}{modName}\\{entry.FullName.Substring(dllFolder.Length + 1)}");
                            }
                        }
                        // If there isn't an extra folder, just extract
                        else
                        {
                            if (!Directory.Exists($"{modsDirectory}{modName}\\{Path.GetDirectoryName(entry.FullName)}"))
                            {
                                Directory.CreateDirectory($"{modsDirectory}{modName}\\{Path.GetDirectoryName(entry.FullName)}");
                            }
                            if (!entry.FullName.EndsWith("/"))
                            {
                                entry.ExtractToFile($"{modsDirectory}{modName}\\{entry.FullName.Substring(dllFolder.Length)}");
                            }
                        }
                    }
                }
                zip.Dispose();
                if (moreFiles)
                {
                    // WE DID IT!
                    await ModernMessageBox.Show($"\"{modName}\" has been installed!\n\nFollowing this message box, the directory for \"{modName}\\\" will be opened. Please install the additional files to their appropriate directory. If you're stuck, try checking the GameBanana page for the mod or looking for a \"readme.txt\" file.", "Additional Files Detected!");
                    GC.WaitForPendingFinalizers();
                    File.Delete($"{AppDomain.CurrentDomain.BaseDirectory}\\mods\\{fileName}");
                    Process.Start($"{AppDomain.CurrentDomain.BaseDirectory}\\mods\\{modName}");
                }
                else
                {
                    // Remove the zip
                    File.Delete($"{AppDomain.CurrentDomain.BaseDirectory}\\mods\\{fileName}");
                    // WE DID IT!
                    await ModernMessageBox.Show($"\"{modName}\" has been installed!", "Success!");
                }

            }
            catch (Exception e)
            {
                MessageBox.Show($"An error has occured: {e.Message}");
                if (File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}\\mods\\{fileName}"))
                {
                    File.Delete($"{AppDomain.CurrentDomain.BaseDirectory}\\mods\\{fileName}");
                }
            }
        }
    }
    public static string GetModName(string modID, WebClient client)
    {

        string modName;
        var title = GetModTitle($"https://gamebanana.com/mods/{modID}");
        // Grab the mod name from the title of the main page
        if (client.DownloadString($"https://api.gamebanana.com/Core/Item/Data?itemtype=Mod&itemid={modID}&fields=Game%28%29.name&format=json_min").Contains("Mania"))
        {
            // Banana Mania Mod Names
            modName = title.Remove(title.Length - 40, 40);
        }
        else
        {
            // BBHD Mod Names
            modName = title.Remove(title.Length - 44, 44);
        }
        return modName;
    }
}
