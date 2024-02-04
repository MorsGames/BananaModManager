using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using BananaModManager.Shared;
using Microsoft.UI.Xaml.Controls;

namespace BananaModManager;

public class Update
{
    public static void DoUpdate()
    {
        try
        {
            // Set the directory values for later use
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var parentDirectory = Directory.GetParent(Directory.GetParent(currentDirectory).ToString()).ToString();

            // Define the zip file name and path
            const string zipFileName = "Download.zip";
            var zipFilePath = Path.Combine(currentDirectory, zipFileName);

            // Check if the zip file exists
            if (File.Exists(zipFilePath))
            {
                // Open the zip file and extract it to the parent directory
                using var zipFile = ZipFile.OpenRead(zipFilePath);
                zipFile.ExtractToDirectory(parentDirectory, true);
            }
            else
            {
                // Print an error message
                MessageBox.Show($"The zip file {zipFileName} is not found in {currentDirectory}.", "Error!", MessageBoxButtons.Ok, MessageBoxIcon.Error);
                return;
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
            MessageBox.Show(e.ToString(), "Error!", MessageBoxButtons.Ok, MessageBoxIcon.Error);
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

            // Say we are done
            await ModernMessageBox.Show("After the installation is complete, remember to manually update the mod loader for each of your games by clicking the \"Update Mod Loader\" button in the settings menu.", "The update has been successfully downloaded!", "Sure thing, boss!");

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

    public static void UpdateModLoader()
    {
        var game = App.CurrentGame;

        // No game, no setup
        if (game == Games.Default)
            return;

        // The folder where all the loader files are located before copying
        const string loaderDir = "Loader";

        // Copy the ini files for doorstop
        var sourceDoorstopFile = game.Managed ? "doorstop_config_mono.ini" : "doorstop_config_il2cpp.ini";
        const string targetDoorstopFile = "doorstop_config.ini";
        File.Copy(
            Path.Combine(loaderDir, sourceDoorstopFile),
            App.PathConvert(targetDoorstopFile), true);

        // Copy the library shared between all
        const string sharedFile = "BananaModManager.Shared.dll";
        File.Copy(
            Path.Combine(loaderDir, sharedFile),
            App.PathConvert(sharedFile), true);

        // The architecture specific stuff
        var architectureFolder = game.X64 ? "x64" : "x86";

        // Copy the doorstop entry point file
        const string doorstopEntryFile = "winhttp.dll";
        File.Copy(
            Path.Combine(loaderDir, architectureFolder, doorstopEntryFile),
            App.PathConvert(doorstopEntryFile), true);

        // Copy the detours library
        const string detoursFile = "BananaModManager.Detours.dll";
        File.Copy(
            Path.Combine(loaderDir, architectureFolder, detoursFile),
            App.PathConvert(detoursFile), true);

        // Copy the Mono / IL2Cpp specific loader library
        var loaderFile = game.Managed ? "BananaModManager.Loader.Mono.dll" : "BananaModManager.Loader.IL2Cpp.dll";
        File.Copy(
            Path.Combine(loaderDir, loaderFile),
            App.PathConvert(loaderFile), true);

        // Following is only for IL2Cpp games
        if (!game.Managed)
        {
            // Copy the Discord RPC library
            const string discordRPCFile = "DiscordRPC.dll";
            File.Copy(
                Path.Combine(loaderDir, discordRPCFile),
                App.PathConvert(discordRPCFile), true);

            // Copy the mono files
            const string monoDir = "mono";
            var sourceDirectory = Path.Combine(loaderDir, monoDir);
            var destinationDirectory = App.PathConvert(monoDir);

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

        // We will need these in a bit
        var dumperDir = App.PathConvert("Il2CppDumper");
        var unhollowerDir = App.PathConvert("Il2CppAssemblyUnhollower");

        // Download the necessary tools
        if (!Directory.Exists(dumperDir))
        {
            var zipPath = Path.Combine(tempPath, "Il2CppDumper-v6.6.5.zip");
            using var client = new WebClient();
            client.DownloadFile("https://github.com/Perfare/Il2CppDumper/releases/download/v6.6.5/Il2CppDumper-v6.6.5.zip", zipPath);
            ZipFile.ExtractToDirectory(zipPath, dumperDir);
        }
        if (!Directory.Exists(unhollowerDir))
        {
            var zipPath = Path.Combine(tempPath, "Il2CppAssemblyUnhollower.0.4.15.4.zip");
            using var client = new WebClient();
            client.DownloadFile("https://github.com/knah/Il2CppAssemblyUnhollower/releases/download/v0.4.15.4/Il2CppAssemblyUnhollower.0.4.15.4.zip", zipPath);
            ZipFile.ExtractToDirectory(zipPath, unhollowerDir);
        }

        // Run the tools if needed
        if (!Directory.Exists(Path.Combine(dumperDir, "DummyDll")))
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(dumperDir, "Il2CppDumper.exe"),
                Arguments = $"\"GameAssembly.dll\" {game.ExecutableName}\"_Data\\il2cpp_data\\Metadata\\global-metadata.dat\"",
                WorkingDirectory = App.ManagerConfig.GetGameDirectory()
            };
            var process = Process.Start(processStartInfo);
            process?.WaitForExit();
        }

        var process2StartInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(unhollowerDir, "AssemblyUnhollower.exe"),
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

        // Delete the stuff we don't need
        Directory.Delete(dumperDir, true);
        Directory.Delete(unhollowerDir, true);
        Directory.Delete(tempPath, true);

        // We are done!
    }
}
