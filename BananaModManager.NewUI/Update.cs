using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using BananaModManager.Shared;
using Microsoft.UI.Xaml.Controls;

namespace BananaModManager.NewUI;

public class Update
{
    public static void DoUpdate()
    {
        try
        {
            // Set the directory values for later use
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var parentDirectory = Directory.GetParent(currentDirectory).ToString();

            // The zip file path values
            var zipFile = Path.Combine(currentDirectory, "Download.zip");

            // Unzip the zip file
            using (var archive = ZipFile.OpenRead(zipFile))
            {
                // Go through each file
                foreach (var entry in archive.Entries)
                {
                    // If it's a folder
                    if (entry.FullName.EndsWith("/") || entry.FullName.EndsWith("\\"))
                    {
                        var entryFullName = entry.FullName.Replace('/', '\\');
                        var entryFullPath = Path.Combine(parentDirectory, entryFullName);
                        if (!Directory.Exists(entryFullPath))
                            Directory.CreateDirectory(entryFullPath);
                    }
                    // If it's a file
                    else
                    {
                        var entryFullName = entry.FullName.Replace('/', '\\');
                        var entryFullPath = Path.Combine(parentDirectory, entryFullName);
                        entry.ExtractToFile(entryFullPath, true);
                    }
                }
            }

            // Without this BMM gets called a Trojan for remote executing another program soooo
            var yes = true;
            if (yes)
            {
                // Run the new version
                var updatedStartInfo = new ProcessStartInfo(Path.Combine(parentDirectory, "BananaModManager.exe"));
                Process.Start(updatedStartInfo);

                // Bye bye!
                Environment.Exit(0);
            }

        }
        catch (Exception e)
        {
            // Old school message box on purpose, as we want this error to show before the UI has loaded
            MessageBox.Show(e.ToString(), "Error!");
        }
    }
    public static async Task DownloadAndRun()
    {
        // Commonly used paths
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var tempDir = Path.Combine(baseDir, "UpdateFiles");
        var zipFile = Path.Combine(tempDir, "Download.zip");

        try
        {
            // We need a temp folder to store the new version of BMM
            // If it exists, delete its contents
            if (Directory.Exists(tempDir))
            {
                foreach (var file in Directory.GetFiles(tempDir))
                {
                    File.Delete(file);
                }
                foreach (var subfolder in Directory.GetDirectories(tempDir))
                {
                    Directory.Delete(subfolder, true);
                }
            }
            // Otherwise, create it
            else
            {
                Directory.CreateDirectory(tempDir);
            }

            // This is where we download the zip file for the update
            using (var client = new WebClient())
            {
                // Get the latest release info from GitHub's API
                client.Headers.Add("user-agent", "request");
                var jsonData = client.DownloadString(new Uri("https://api.github.com/repos/MorsGames/BananaModManager/releases/latest"));
                var parsedJson = jsonData.Deserialize<Release>();

                // Download the zip file
                client.DownloadFile(parsedJson.assets[0].browser_download_url, zipFile);
            }

            // Extract it!
            ZipFile.ExtractToDirectory(zipFile, tempDir);

            // Run the executable so it can do the update stuff
            var exePath = Path.Combine(tempDir, "BananaModManager.exe");
            var startInfo = new ProcessStartInfo(exePath)
            {
                Arguments = "--update"
            };
            Process.Start(startInfo);
        }
        catch (Exception e)
        {
            await ModernMessageBox.Show(e.ToString(), "Error!");
        }
    }

    public static async Task UpdateSpeedrunLegalMods()
    {
        var installAll = false;
        var message = await ModernMessageBox.Show("Would you like to install all other Speedrun-legal mods in addition to updating the mods you already have installed?", "Install New Mods?", "Yes!", "No thanks");
        if (message == ContentDialogResult.Primary)
        {
            installAll = true;
        }
        using (var wc = new WebClient())
        {
            try
            {
                // Get the latest release info from GitHub's API
                wc.Headers.Add("user-agent", "request");
                var jsonData = wc.DownloadString(new Uri("https://api.github.com/repos/MorsGames/BananaModManager/releases/latest"));
                var parsedJson = jsonData.Deserialize<Release>();

                // Delete leftover if it's there for some reason
                var legalModsPath = App.PathConvert("LegalMods.zip");
                if (File.Exists(legalModsPath))
                {
                    File.Delete(legalModsPath);
                }
                wc.DownloadFile(parsedJson.assets[1].browser_download_url, legalModsPath);

                // Get path of mods folder and whitelisted mods list
                var pathModsFolder = App.PathConvert("mods\\");
                var whitelistedMods = Games.BananaMania.WhitelistNames;

                // Prep objects for handling the zip file
                var zipArchive = ZipFile.OpenRead(legalModsPath);

                foreach (var modDLL in whitelistedMods)
                {
                    var pathInstalledModLocation = Directory.GetFiles(pathModsFolder, modDLL, SearchOption.AllDirectories);
                    // If the mod was already installed
                    if (pathInstalledModLocation.Length > 0)
                    {
                        // Cut down the full path to the DLL to just be the path to the mod folder
                        var pathInstalledModFolder = pathInstalledModLocation[0].Substring(0, (pathInstalledModLocation[0].Length - modDLL.Length));
                        var directoryInfo = new DirectoryInfo(pathInstalledModFolder);
                        // Clear out the old directory
                        foreach (var file in directoryInfo.GetFiles())
                        {
                            // file.Delete();
                        }
                        // Find the equivalent mod folder in the downloaded zip
                        string path = null;
                        foreach (var entry in zipArchive.Entries)
                        {
                            if (entry.Name.Contains(modDLL))
                            {
                                path = entry.FullName.Substring(0, entry.FullName.Length - modDLL.Length);
                            }
                        }
                        // Iterate over every file in the found folder, overwrite if it wasn't deleted.
                        foreach (var entry in zipArchive.Entries)
                        {
                            if (entry.FullName.Contains(path) && entry.FullName.Substring(path.Length - 1 ) == path && !entry.FullName.EndsWith("/"))
                            {
                                var fixedPath = (pathInstalledModFolder.Substring(0, pathInstalledModFolder.Length - 1) + entry.FullName.Substring(path.Length - 1).Replace('/', '\\'));
                                entry.ExtractToFile(fixedPath, true);
                            }
                        }

                    }
                    // If the mod was not already installed
                    else
                    {
                        if (installAll)
                        {
                            string path = null;
                            foreach (var entry in zipArchive.Entries)
                            {
                                if (entry.Name.Contains(modDLL))
                                {
                                    path = entry.FullName.Substring(0, entry.FullName.Length - modDLL.Length);
                                }
                            }
                            foreach (var entry in zipArchive.Entries)
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
                zipArchive.Dispose();
                File.Delete(legalModsPath);
            }
            catch (Exception e)
            {
                await ModernMessageBox.Show(e.ToString(), "Error!");
            }
        }
    }

    public static async void UpdateModLoader()
    {
        try
        {
            var game = App.CurrentGame;

            // No game, no setup.
            if (game == Games.Default)
                return;

            // TODO: Don't hardcode the names like this...
            // Copy the ini files for doorstop
            File.Copy(game.Managed ? "Loader\\doorstop_config_mono.ini" : "Loader\\doorstop_config_il2cpp.ini", App.PathConvert("doorstop_config.ini"), true);

            // Copy the mod loader DLL files
            File.Copy("Loader\\BananaModManager.Shared.dll", App.PathConvert("BananaModManager.Shared.dll"), true);
            if (game.X64)
            {
                File.Copy("Loader\\x64\\winhttp.dll", App.PathConvert("winhttp.dll"), true);
                File.Copy("Loader\\x64\\BananaModManager.Detours.dll", App.PathConvert("BananaModManager.Detours.dll"), true);
            }
            else
            {
                File.Copy("Loader\\x86\\winhttp.dll", App.PathConvert("winhttp.dll"), true);
                File.Copy("Loader\\x86\\BananaModManager.Detours.dll", App.PathConvert("BananaModManager.Detours.dll"), true);
            }
            if (game.Managed)
            {
                File.Copy("Loader\\BananaModManager.Loader.Mono.dll", App.PathConvert("BananaModManager.Loader.Mono.dll"), true);
            }
            else
            {
                File.Copy("Loader\\DiscordRPC.dll", App.PathConvert("DiscordRPC.dll"), true);
                File.Copy("Loader\\BananaModManager.Loader.IL2Cpp.dll", App.PathConvert("BananaModManager.Loader.IL2Cpp.dll"), true);

                // Copy the mono files
                var sourceDirectory = "Loader\\mono";
                var destinationDirectory = App.PathConvert("mono");

                // Get all files in the source directory and its subdirectories
                var files = Directory.GetFiles(sourceDirectory, "*.*", SearchOption.AllDirectories);

                // Copy each file to the destination directory
                foreach (var filePath in files)
                {
                    // Create the destination path by replacing the source directory with the destination directory
                    var destinationPath = Path.Combine(destinationDirectory, filePath.Substring(sourceDirectory.Length + 1));

                    // Ensure the destination directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                    // Perform the file copy
                    File.Copy(filePath, destinationPath, true);
                }
            }

            // Check if the game is managed or already decompiled.
            if (game.Managed || Directory.Exists(App.PathConvert("managed")))
            {
                // If so we can skip the rest
                return;
            }

            // Otherwise, we are gaming
            var tempPath = Path.Combine(Path.GetTempPath(), "BananaModManager");
            Directory.CreateDirectory(tempPath);

            // Download the necessary tools
            if (!Directory.Exists("Il2CppDumper"))
            {
                var zipPath = Path.Combine(tempPath, "Il2CppDumper-v6.6.5.zip");
                using var client = new WebClient();
                client.DownloadFile("https://github.com/Perfare/Il2CppDumper/releases/download/v6.6.5/Il2CppDumper-v6.6.5.zip", zipPath);
                ZipFile.ExtractToDirectory(zipPath, App.PathConvert("Il2CppDumper"));
            }
            if (!Directory.Exists("Il2CppAssemblyUnhollower"))
            {
                var zipPath = Path.Combine(tempPath, "Il2CppAssemblyUnhollower.0.4.15.4.zip");
                using var client = new WebClient();
                client.DownloadFile("https://github.com/knah/Il2CppAssemblyUnhollower/releases/download/v0.4.15.4/Il2CppAssemblyUnhollower.0.4.15.4.zip", zipPath);
                ZipFile.ExtractToDirectory(zipPath, App.PathConvert("Il2CppAssemblyUnhollower"));
            }

            // Run the tools if needed
            if (!Directory.Exists(App.PathConvert("Il2CppDumper\\DummyDll")))
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = App.PathConvert("Il2CppDumper\\Il2CppDumper.exe"),
                    Arguments = $"\"GameAssembly.dll\" {game.ExecutableName}\"_Data\\il2cpp_data\\Metadata\\global-metadata.dat\"",
                    WorkingDirectory = App.ManagerConfig.GetGameDirectory()
                };
                var process = Process.Start(processStartInfo);
                process?.WaitForExit();
            }

            var process2StartInfo = new ProcessStartInfo
            {
                FileName = App.PathConvert("Il2CppAssemblyUnhollower\\AssemblyUnhollower.exe"),
                Arguments = "--input=\"Il2CppDumper\\DummyDll\" --output=\"managed\" --mscorlib=\"mono\\Managed\\mscorlib.dll\"",
                WorkingDirectory = App.ManagerConfig.GetGameDirectory()
            };
            var process2 = Process.Start(process2StartInfo);
            process2?.WaitForExit();

            // Overwrite dummy mono files with actual ones
            foreach (var path in Directory.GetFiles(App.PathConvert("mono\\Managed"), "*.dll", SearchOption.AllDirectories))
            {
                var lastIndexOfManaged = path.LastIndexOf("mono\\Managed", StringComparison.Ordinal);
                var newPath = path.Substring(0, lastIndexOfManaged) + "managed" + path.Substring(lastIndexOfManaged + "mono\\Managed".Length);
                File.Copy(path, newPath, true);
            }
            /*foreach (var path in Directory.GetFiles("Il2CppAssemblyUnhollower", "*.dll", SearchOption.AllDirectories))
            {
                var newPath = path.Replace("Il2CppAssemblyUnhollower", "managed");
                File.Copy(path, newPath, true);
            }*/

            // Delete stuff we don't need
            Directory.Delete(App.PathConvert("Il2CppDumper"), true);
            Directory.Delete(App.PathConvert("Il2CppAssemblyUnhollower"), true);
            Directory.Delete(tempPath, true);
        }
        catch (Exception e)
        {
            await ModernMessageBox.Show(e.ToString(), "Error!");
        }
    }
}
