using System;
using System.IO;
using Newtonsoft.Json;
using ZoneControl.Infrastructure;

namespace ZoneControl.Configuration;

internal static class ModConfig
{
    #region Schematic
    private const string ConfigFileName = "modconfig.json";
    private const string DefaultsFileName = "modconfig.defaults.json";
    private const string LegacyConfigFileName = "config.json";

    private const string DefaultMetaDescription = "RELEASE Configuration file for Zone Control mod package";
    private const string UserMetaDescription = "USER Configuration file for Zone Control mod package";

    /// <summary>
    /// Maximum allowed config file size in bytes (1KB) to prevent abuse
    /// </summary>
    private const long MaxConfigFileSize = 1024;

    private static bool IsConfigLoaded { get; set; } = false;
    internal static ModConfigData Config { get; private set; } = new ModConfigData();

    /// <summary>
    /// Gets the full path to the configuration file
    /// </summary>
    private static string GetConfigFilePath()
    {
        return Path.Combine(ModPathManager.GetConfigPath(true), ConfigFileName);
    }

    internal static void LoadConfig()
    {
        var configDir = ModPathManager.GetConfigPath(true);
        var defaults = ReadConfigData(Path.Combine(configDir, DefaultsFileName));
        var configPath = Path.Combine(configDir, ConfigFileName);
        var legacyPath = Path.Combine(configDir, LegacyConfigFileName);

        var defaultsVersion = defaults?.version;

        ModConfigData result;
        bool shouldSave = false;

        var existing = ReadConfigData(configPath);
        if (existing != null)
        {
            if (defaults != null && IsOlderVersion(existing.version, defaultsVersion))
            {
                // Upgrade: start from defaults, overlay the user's existing values, and bump to the current version.
                result = defaults;
                PopulateFromFile(configPath, result);
                result.version = defaultsVersion;
                shouldSave = true;
            }
            else
            {
                result = existing;
            }
        }
        else
        {
            // No user config yet. Start from defaults and overlay a legacy config.json if present.
            result = defaults ?? new ModConfigData();
            if (File.Exists(legacyPath))
            {
                PopulateFromFile(legacyPath, result);
            }
            result.version = defaultsVersion ?? result.version;
            shouldSave = true;
        }

        Config = result;
        IsConfigLoaded = true;

        if (shouldSave)
        {
            SaveConfig();
        }

        ModLogger.Info($"Config loaded successfully (v{Config.version}, debug={Config.isDebug}).");
    }

    private static ModConfigData ReadConfigData(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            return JsonConvert.DeserializeObject<ModConfigData>(File.ReadAllText(path));
        }
        catch (Exception ex)
        {
            ModLogger.Error($"Failed to read config file '{path}'.", ex);
            return null;
        }
    }

    private static void PopulateFromFile(string path, ModConfigData target)
    {
        try
        {
            JsonConvert.PopulateObject(File.ReadAllText(path), target);
        }
        catch (Exception ex)
        {
            ModLogger.Error($"Failed to merge config file '{path}'.", ex);
        }
    }

    private static bool IsOlderVersion(string existingVersion, string defaultsVersion)
    {
        if (string.IsNullOrEmpty(existingVersion))
        {
            return true;
        }
        if (string.IsNullOrEmpty(defaultsVersion))
        {
            return false;
        }
        if (Version.TryParse(existingVersion, out var existing) && Version.TryParse(defaultsVersion, out var defaults))
        {
            return existing < defaults;
        }
        return string.CompareOrdinal(existingVersion, defaultsVersion) < 0;
    }

    /// <summary>
    /// Saves the current config to the default config file location
    /// </summary>
    public static void SaveConfig()
    {
        SaveConfig(GetConfigFilePath());
    }

    /// <summary>
    /// Saves the current config to file with size validation
    /// </summary>
    private static void SaveConfig(string path)
    {
        ApplyMetaDescription();

        try
        {
            string configJson;

            var serializer = JsonSerializer.CreateDefault();

            using (var sw = new StringWriter())
            using (var writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                writer.IndentChar = ' ';
                writer.Indentation = 4;

                serializer.Serialize(writer, Config);

                configJson = sw.ToString();
            }

            // Validate serialized config size before writing
            var configBytes = System.Text.Encoding.UTF8.GetByteCount(configJson);
            if (configBytes > MaxConfigFileSize)
            {
                ModLogger.Error($"Generated config is too large ({configBytes} bytes, max {MaxConfigFileSize} bytes). Not saving to prevent abuse.");
                return;
            }

            File.WriteAllText(path, configJson);

#if DEBUG
            ModLogger.DebugLog($"Config saved successfully ({configBytes} bytes)");
#endif
        }
        catch (Exception e)
        {
            ModLogger.Warning($"Failed to save config to {path}: {e.Message}");
        }
    }

    private static void ApplyMetaDescription()
    {
        if (string.IsNullOrEmpty(Config.metaDescription) || Config.metaDescription == DefaultMetaDescription)
        {
            Config.metaDescription = UserMetaDescription;
        }
    }
    #endregion

    #region Zone
    internal const int DEFAULT_ZONE_CONTROL_SIZE = 60;
    internal const int MIN_ZONE_CONTROL_SIZE = 20;
    internal const int MAX_ZONE_CONTROL_SIZE = 100;

    internal static int ZoneControlSize()
    {
        if (IsConfigLoaded)
        {
            return Config.zoneControlSize;
        }
        return DEFAULT_ZONE_CONTROL_SIZE;
    }
    #endregion

    #region Land Claim
    internal const int DEFAULT_LANDCLAIM_COUNT = 3;
    internal const int MIN_LAND_CLAIM_COUNT = 1;
    internal const int MAX_LAND_CLAIM_COUNT = 15;  // 3 per biome

    internal const int DEFAULT_LANDCLAIM_SIZE = 41;
    internal const int MIN_LAND_CLAIM_SIZE = 30;
    internal const int MAX_LAND_CLAIM_SIZE = 60;

    internal static int LandClaimCount()
    {
        if (IsConfigLoaded)
        {
            return Config.landClaimCount;
        }
        return DEFAULT_LANDCLAIM_COUNT;
    }

    internal static int LandClaimSize()
    {
        if (IsConfigLoaded)
        {
            return Config.landClaimSize;
        }
        return DEFAULT_LANDCLAIM_SIZE;
    }
    #endregion

    #region Map
    internal static bool HideLandClaimsFromCompassOnStart()
    {
        return IsConfigLoaded && Config.hideLandClaimsFromCompassOnStart;
    }

    internal static bool HideSleepingBagsFromCompassOnStart()
    {
        return IsConfigLoaded && Config.hideSleepingBagsFromCompassOnStart;
    }
    #endregion

    #region General
    internal static bool IsDebug()
    {
        return IsConfigLoaded && Config.isDebug;
    }
    #endregion
}
