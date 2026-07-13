using System.IO;
using Newtonsoft.Json;
using ZoneControl.Infrastructure;

namespace ZoneControl.Configuration;

internal static class ModConfig
{
    private const string ConfigFileName = "modconfig.json";
    private static bool IsConfigLoaded { get; set; } = false;
    private static ModConfigData Config { get; set; } = new ModConfigData();

    /// <summary>
    /// Maximum allowed config file size in bytes (1KB) to prevent abuse
    /// </summary>
    private const long MaxConfigFileSize = 1024;

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

    internal static bool IsDebug()
    {
        return IsConfigLoaded && Config.IsDebug;
    }

    internal static string GetVersion()
    {
        return IsConfigLoaded ? Config.Version : "0.0.0";
    }

    internal static bool HideLandClaimFromCompassOnStart()
    {
        return IsConfigLoaded && Config.HideLandClaimFromCompassOnStart;
    }

    internal static bool HideSleepingBagFromCompassOnStart()
    {
        return IsConfigLoaded && Config.HideSleepingBagFromCompassOnStart;
    }
}
