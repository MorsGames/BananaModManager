using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace BananaModManager.Shared
{
    // TODO: Why is this static? Make it an instance of App on the new UI once the old one is depreciated
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
        public static void Load(out UserConfig userConfig, string gameDirectory)
        {
            // Load the config file
            userConfig = LoadUserConfig(gameDirectory);
            userConfig.ActiveMods ??= new List<string>();

            // Get the mods folder
            var folder = Path.Combine(gameDirectory, Folder);
            var modFolder = new DirectoryInfo(folder);
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
        private static ModInfo LoadModInfo(DirectoryInfo directory)
        {
            // Load the file into a string
            var file = Path.Combine(directory.FullName, "mod.json");
            var text = File.ReadAllText(file);

            // Deserialize the string
            var modInfo = JsonSerializer.Deserialize<ModInfo>(text);
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
            var file = Path.Combine(directory.FullName, "config.json");

            // If the file doesn't exist, return an empty dictionary
            if (!File.Exists(file))
            {
                return new Dictionary<string, ConfigItem>();
            }

            var defaultConfig =
                JsonSerializer.Deserialize<Dictionary<string, ConfigItem>>(
                    File.ReadAllText(file));
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
        public static UserConfig LoadUserConfig(string gameDirectory)
        {
            UserConfig userConfig;
            // Load and deserialize the mod loader config file if it exists
            // If it doesn't just create an empty one
            try
            {
                var configFilePath = Path.Combine(gameDirectory, Folder, ConfigFile);
                if (File.Exists(configFilePath))
                    userConfig = JsonSerializer.Deserialize<UserConfig>(File.ReadAllText(configFilePath)) ??
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
        /// <param name="userConfig">Settings of the mod manager.</param>
        public static void Save(UserConfig userConfig, string gameDirectory)
        {
            // Add the configs into it
            foreach (var mod in List.Select(_ => _.Value))
            {
                // Convert the config before adding it to the list so it doesn't contain unnecessary information
                var config = ConvertConfig(mod.Config);
                userConfig.ModConfigs.Remove(mod.ToString());
                userConfig.ModConfigs.Add(mod.ToString(), config);
            }

            // Serialize and save it
            var configJson = JsonSerializer.Serialize(userConfig);
            File.WriteAllText(Path.Combine(gameDirectory, Folder, ConfigFile), configJson);
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
