using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using BananaModManager.Loader;

namespace BananaModManager.Shared
{
    public static class Mods
    {
        public const string ConfigFile = "BananaModManager.json";
        public static string Folder = "mods\\";

        /// <summary>
        ///     A list of all mods.
        /// </summary>
        public static Dictionary<string, Mod> List { get; set; } = new Dictionary<string, Mod>();

        /// <summary>
        ///     Loads all necessary data about all the mods
        /// </summary>
        /// <param name="activeMods">A list of all mods that are currently enabled, in a specific order.</param>
        public static void Load(out List<string> activeMods)
        {
            // Load the config file
            var userConfig = LoadUserConfig();

            // Set the active mods
            activeMods = userConfig.ActiveMods ?? new List<string>();

            // Get the mods folder
            var modFolder = new DirectoryInfo(Folder);
            if (!modFolder.Exists)
                modFolder.Create();

            // Load individual mods
            foreach (var directory in modFolder.GetDirectories())
            {
                // Load the mod info
                var modInfo = LoadModInfo(directory);

                // Load the default config
                var defaultConfig = LoadDefaultModConfig(directory);

                // Load the mod config, either from the loader config file or the default configs file inside the mod folder
                var modConfig = LoadModConfig(modInfo, userConfig, defaultConfig);

                // Mod object
                var mod = new Mod
                {
                    Info = modInfo,
                    DefaultConfig = defaultConfig,
                    Config = modConfig,
                    Directory = directory
                };

                // Add it to the mods list
                List.Add(mod.ToString(), mod);
            }
        }

        /// <summary>
        ///     Loads the information of a specific mod.
        /// </summary>
        /// <param name="directory">Directory of the mod.</param>
        /// <returns>Mod information.</returns>
        public static ModInfo LoadModInfo(DirectoryInfo directory)
        {
            var modInfo = JsonSerializer.Deserialize<ModInfo>(File.ReadAllText(directory.FullName + "\\mod.json"));
            return modInfo;
        }

        /// <summary>
        ///     Loads the current config values of a specific mod.
        /// </summary>
        /// <param name="modInfo">Information about the mod.</param>
        /// <param name="userConfig">The config values of the mod manager.</param>
        /// <param name="defaultConfig">Default values to fall back to if the values don't exist.</param>
        /// <returns>Config values of the mod.</returns>
        public static Dictionary<string, ConfigItem> LoadModConfig(ModInfo modInfo, UserConfig userConfig,
            Dictionary<string, ConfigItem> defaultConfig)
        {
            // If that config item is modified, change the value
            if (userConfig.ModConfigs.ContainsKey(modInfo.ToString()))
            {
                var newConfig = new Dictionary<string, ConfigItem>();
                for (var i = 0; i < defaultConfig.Count; i++)
                {
                    var defaultElement = defaultConfig.ElementAt(i);
                    var modConfig = userConfig.ModConfigs[modInfo.ToString()];

                    ConfigItem item;
                    // If the config exists, load
                    if (modConfig.ContainsKey(defaultElement.Key))
                    {
                        item = new ConfigItem
                        {
                            Value = ConvertJsonValue(modConfig[defaultElement.Key]),
                            Description = defaultElement.Value.Description,
                            Category = defaultElement.Value.Category
                        };
                    }
                    // Otherwise resort to the default
                    else
                    {
                        item = defaultElement.Value;
                    }

                    newConfig.Add(defaultElement.Key, item);
                }

                return newConfig;
            }

            // Return the default one otherwise
            return defaultConfig;
        }

        /// <summary>
        ///     Loads the default config values of a specific mod.
        /// </summary>
        /// <param name="directory">Directory of the mod.</param>
        /// <returns>Default config values of the mod.</returns>
        public static Dictionary<string, ConfigItem> LoadDefaultModConfig(DirectoryInfo directory)
        {
            var defaultConfig =
                JsonSerializer.Deserialize<Dictionary<string, ConfigItem>>(
                    File.ReadAllText(directory.FullName + "\\config.json"));
            foreach (var element in defaultConfig)
            {
                var configItem = element.Value;
                configItem.Value = ConvertJsonValue(configItem.Value);
            }

            return defaultConfig;
        }

        /// <summary>
        ///     Loads all the user config of the mod manager.
        /// </summary>
        /// <returns>Settings of the mod manager.</returns>
        public static UserConfig LoadUserConfig()
        {
            UserConfig userConfig;
            // Load and deserialize the mod loader config file if it exists
            // If it doesn't just create an empty one
            try
            {
                if (File.Exists(Folder + ConfigFile))
                    userConfig = JsonSerializer.Deserialize<UserConfig>(File.ReadAllText(Folder + ConfigFile)) ??
                                 new UserConfig();
                else
                    userConfig = new UserConfig();
            }
            // At any error it's better to start fresh then to silently crash.
            catch
            {
                MessageBox.Show("An error occured when reading your settings.", "Error!", MessageBoxButtons.Ok, MessageBoxIcon.Error);
                userConfig = new UserConfig();
            }
            return userConfig;
        }

        /// <summary>
        ///     Saves the user config of the mod manager.
        /// </summary>
        /// <param name="activeMods">A list of enabled mods, in a specific order.</param>
        public static void Save(List<string> activeMods)
        {
            // Config object used for the user data
            var loaderConfig = new UserConfig {ActiveMods = activeMods};

            // Add the configs into it
            foreach (var mod in List.Select(_ => _.Value))
            {
                // Convert the config before adding it to the list so it doesn't contain unnecessary information
                var config = ConvertConfig(mod.Config);
                loaderConfig.ModConfigs.Add(mod.ToString(), config);
            }

            // Serialize and save it
            var configJson = JsonSerializer.Serialize(loaderConfig);
            File.WriteAllText(Folder + ConfigFile, configJson);
        }

        /// <summary>
        ///     Converts a regular config dictionary into a format that's easier to parse by mod developers.
        /// </summary>
        /// <param name="config">The base config dictionary.</param>
        /// <returns>The new dictionary</returns>
        public static Dictionary<string, object> ConvertConfig(Dictionary<string, ConfigItem> config)
        {
            var output = new Dictionary<string, object>();

            // Convert
            foreach (var element in config)
            {
                var value = element.Value.Value;
                output.Add(element.Key, ConvertJsonValue(value));
            }

            return output;
        }

        /// <summary>
        ///     Convert JsonElement to what it's supposed to represent.
        /// </summary>
        /// <param name="value">The object.</param>
        /// <returns>Converted result.</returns>
        public static object ConvertJsonValue(object value)
        {
            if (value is JsonElement jsonElement)
            {
                var kind = jsonElement.ValueKind;
                value = kind switch
                {
                    JsonValueKind.Number => jsonElement.GetSingle(),
                    JsonValueKind.String => jsonElement.GetString(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    _ => null
                };
            }

            return value;
        }
    }
}