using System;
using System.IO;
using Newtonsoft.Json;
using ZoneControl.Infrastructure;

namespace ZoneControl.Configuration;

internal static class ModConfig
{
    #region Schematic
    private const string ConfigFileName = "modconfig.json";
    private static bool IsConfigLoaded { get; set; } = false;
    internal static ModConfigData Config { get; private set; } = new ModConfigData();

    /// <summary>
    /// Maximum allowed config file size in bytes (1KB) to prevent abuse
    /// </summary>
    private const long MaxConfigFileSize = 1024;

    /// <summary>
    /// Gets the full path to the configuration file
    /// </summary>
    /// <returns>Full path to the config.json file</returns>
    private static string GetConfigFilePath()
    {
        return Path.Combine(ModPathManager.GetConfigPath(true), ConfigFileName);
    }

    internal static void LoadConfig()
    {
        var path = Path.Combine(ModPathManager.GetConfigPath(true), ConfigFileName);
        ModLogger.Info($"Loading config from {path}");

        try
        {
            var fileInfo = new FileInfo(path);

            if (!fileInfo.Exists)
            {
                ModLogger.Error($"Config file not found at {path}. Using defaults.");
                return;
            }

            if (fileInfo.Length > MaxConfigFileSize)
            {
                ModLogger.Error($"Config file exceeds maximum allowed size of {MaxConfigFileSize} bytes. Using defaults.");
                return;
            }

            var json = File.ReadAllText(path);
            var loaded = JsonConvert.DeserializeObject<ModConfigData>(json);

            if (loaded == null)
            {
                ModLogger.Error("Config file deserialized to null. Using defaults.");
                return;
            }

            Config = loaded;
            IsConfigLoaded = true;
            ModLogger.Info($"Config loaded successfully (v{Config.Version}, debug={Config.IsDebug}).");
        }
        catch (JsonException ex)
        {
            ModLogger.Error("Failed to parse config file. Using defaults.", ex);
        }
        catch (IOException ex)
        {
            ModLogger.Error("Failed to read config file. Using defaults.", ex);
        }
    }

    /// <summary>
    /// Saves the current config to the default config file location
    /// </summary>
    public static void SaveConfig()
    {
        ValidateConfig(saveAlways: true);
    }

    /// <summary>
    /// Validates and corrects configuration values. Saves config if any changes are made.
    /// </summary>
    private static void ValidateConfig(bool saveAlways = false)
    {
        bool configChanged = false;

        // Track if any validation methods make changes
        configChanged |= ValidateVersion();

        // Save config if any changes were made during validation
        if (configChanged || saveAlways)
        {
            try
            {
                var configPath = GetConfigFilePath();
                SaveConfig(configPath);
                ModLogger.DebugLog("Validated config saved to config file.");
            }
            catch (Exception ex)
            {
                ModLogger.Error($"Failed to save config after validation corrections: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// Saves the current config to file with size validation
    /// </summary>
    private static void SaveConfig(string path)
    {
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

    /// <summary>
    /// Validates and corrects the version field.
    /// </summary>
    /// <returns>True if the config was modified, false otherwise</returns>
    private static bool ValidateVersion()
    {
        if (string.IsNullOrEmpty(Config.Version))
        {
            ModLogger.Warning("Config missing version field, setting to current version");
            Config.Version = ConfigVersioning.CurrentVersion;
            return true; // Config was modified
        }
        return false; // No changes made
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
            return Config.ZoneControlSize;
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
            return Config.LandClaimCount;
        }

        return DEFAULT_LANDCLAIM_COUNT;
    }

    internal static int LandClaimSize()
    {
        if (IsConfigLoaded)
        {
            return Config.LandClaimSize;
        }

        return DEFAULT_LANDCLAIM_SIZE;
    }
    #endregion

    #region Map
    internal static bool HideLandClaimsFromCompassOnStart()
    {
        return IsConfigLoaded && Config.HideLandClaimsFromCompassOnStart;
    }

    internal static bool HideSleepingBagsFromCompassOnStart()
    {
        return IsConfigLoaded && Config.HideSleepingBagsFromCompassOnStart;
    }
    #endregion

    #region General
    internal static bool IsDebug()
    {
        return IsConfigLoaded && Config.IsDebug;
    }
    #endregion
}
