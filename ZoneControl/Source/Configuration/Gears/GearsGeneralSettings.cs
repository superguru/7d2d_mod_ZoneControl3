using GearsAPI.Settings.Global;
using ZoneControl.Infrastructure;

namespace ZoneControl.Configuration.Gears;

internal static class GearsGeneralSettings
{
    internal static void ConfigureGeneralCategorySettings(IGlobalModSettingsCategory generalCategory)
    {
        ConfigureIsDebugSetting(generalCategory);
    }

    private static bool TryGetGlobalValueSetting(IGlobalModSettingsCategory category, string key, out IGlobalValueSetting setting)
    {
        setting = (category.GetSetting(key) as IGlobalValueSetting);
        if (setting == null)
        {
            ModLogger.DebugLog($"Global settings loaded, but setting `{key}` is null");
            return false;
        }

        return true;
    }

    private static void ConfigureIsDebugSetting(IGlobalModSettingsCategory generalCategory)
    {
        if (!TryGetGlobalValueSetting(generalCategory, "IsDebug", out IGlobalValueSetting setting))
        {
            return;
        }

        setting.OnSettingChanged += SetIsDebug;
        SyncIsDebugSetting(setting);
    }

    private static void SetIsDebug(IGlobalModSetting setting, string newValue)
    {
        var value = GearsConversions.ToBool(newValue, false);
        var oldValue = ModConfig.Config.IsDebug;

        if (oldValue != value)
        {
            ModConfig.Config.IsDebug = value;
            ModConfig.SaveConfig();
        }
    }
    private static void SyncIsDebugSetting(IGlobalValueSetting setting)
    {
        var modConfigValue = ModConfig.Config.IsDebug;

        if (!GearsConversions.IsEqualValue(setting.CurrentValue, modConfigValue))
        {
            setting.CurrentValue = GearsConversions.FromBool(modConfigValue);
            GearsModAPI.SaveGlobalSettings();
        }
    }

    internal static void ConfigureMapCategorySettings(IGlobalModSettingsCategory mapCategory)
    {
        ConfigureHideLandClaimFromCompassOnStartSetting(mapCategory);
        ConfigureHideSleepingBagFromCompassOnStartSetting(mapCategory);
    }

    private static void ConfigureHideLandClaimFromCompassOnStartSetting(IGlobalModSettingsCategory mapCategory)
    {
        if (!TryGetGlobalValueSetting(mapCategory, "HideLandClaimsFromCompassOnStart", out IGlobalValueSetting setting))
        {
            return;
        }

        setting.OnSettingChanged += SetHideLandClaimsFromCompassOnStart;
        SyncHideLandClaimsFromCompassOnStartSetting(setting);
    }

    private static void SetHideLandClaimsFromCompassOnStart(IGlobalModSetting setting, string newValue)
    {
        var value = GearsConversions.ToBool(newValue, false);
        var oldValue = ModConfig.Config.HideLandClaimsFromCompassOnStart;

        if (oldValue != value)
        {
            ModConfig.Config.HideLandClaimsFromCompassOnStart = value;
            ModConfig.SaveConfig();
        }
    }
    private static void SyncHideLandClaimsFromCompassOnStartSetting(IGlobalValueSetting setting)
    {
        var modConfigValue = ModConfig.Config.HideLandClaimsFromCompassOnStart;

        if (!GearsConversions.IsEqualValue(setting.CurrentValue, modConfigValue))
        {
            setting.CurrentValue = GearsConversions.FromBool(modConfigValue);
            GearsModAPI.SaveGlobalSettings();
        }
    }

    private static void ConfigureHideSleepingBagFromCompassOnStartSetting(IGlobalModSettingsCategory mapCategory)
    {
        if (!TryGetGlobalValueSetting(mapCategory, "HideSleepingBagsFromCompassOnStart", out IGlobalValueSetting setting))
        {
            return;
        }

        setting.OnSettingChanged += SetHideSleepingBagsFromCompassOnStart;
        SyncHideSleepingBagsFromCompassOnStartSetting(setting);
    }

    private static void SetHideSleepingBagsFromCompassOnStart(IGlobalModSetting setting, string newValue)
    {
        var value = GearsConversions.ToBool(newValue, false);
        var oldValue = ModConfig.Config.HideSleepingBagsFromCompassOnStart;

        if (oldValue != value)
        {
            ModConfig.Config.HideSleepingBagsFromCompassOnStart = value;
            ModConfig.SaveConfig();
        }
    }
    private static void SyncHideSleepingBagsFromCompassOnStartSetting(IGlobalValueSetting setting)
    {
        var modConfigValue = ModConfig.Config.HideSleepingBagsFromCompassOnStart;

        if (!GearsConversions.IsEqualValue(setting.CurrentValue, modConfigValue))
        {
            setting.CurrentValue = GearsConversions.FromBool(modConfigValue);
            GearsModAPI.SaveGlobalSettings();
        }
    }
}
