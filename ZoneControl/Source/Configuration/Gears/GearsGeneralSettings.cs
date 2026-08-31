using GearsAPI.Settings.Global;
using ZoneControl.Infrastructure;

namespace ZoneControl.Configuration.Gears;

internal static class GearsGeneralSettings
{
    internal static void ConfigureGeneralCategorySettings(IGlobalModSettingsCategory category)
    {
        ConfigureIsDebugSetting(category);
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

    private static void ConfigureIsDebugSetting(IGlobalModSettingsCategory category)
    {
        if (!TryGetGlobalValueSetting(category, nameof(ModConfig.IsDebug), out IGlobalValueSetting setting))
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

    internal static void ConfigureMapCategorySettings(IGlobalModSettingsCategory category)
    {
        ConfigureHideLandClaimFromCompassOnStartSetting(category);
        ConfigureHideSleepingBagFromCompassOnStartSetting(category);
    }

    private static void ConfigureHideLandClaimFromCompassOnStartSetting(IGlobalModSettingsCategory category)
    {
        if (!TryGetGlobalValueSetting(category, nameof(ModConfig.HideLandClaimsFromCompassOnStart), out IGlobalValueSetting setting))
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

    private static void ConfigureHideSleepingBagFromCompassOnStartSetting(IGlobalModSettingsCategory category)
    {
        if (!TryGetGlobalValueSetting(category, nameof(ModConfig.HideSleepingBagsFromCompassOnStart), out IGlobalValueSetting setting))
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

    internal static void ConfigureLandClaimCategorySettings(IGlobalModSettingsCategory category)
    {
        ConfigureLandClaimCountSetting(category);
        ConfigureLandClaimSizeSetting(category);
    }

    private static void ConfigureLandClaimCountSetting(IGlobalModSettingsCategory category)
    {
        if (!TryGetGlobalValueSetting(category, nameof(ModConfig.LandClaimCount), out IGlobalValueSetting setting))
        {
            return;
        }

        setting.OnSettingChanged += SetLandClaimCount;
        SyncLandClaimCountSetting(setting);
    }

    private static void SetLandClaimCount(IGlobalModSetting setting, string newValue)
    {
        var value = GearsConversions.ToInt(newValue, ModConfig.DEFAULT_LANDCLAIM_COUNT);
        var oldValue = ModConfig.Config.LandClaimCount;

        if (oldValue != value)
        {
            ModConfig.Config.LandClaimCount = value;
            ModConfig.SaveConfig();
        }
    }

    private static void SyncLandClaimCountSetting(IGlobalValueSetting setting)
    {
        var modConfigValue = ModConfig.Config.LandClaimCount;

        if (!GearsConversions.IsEqualValue(setting.CurrentValue, modConfigValue))
        {
            setting.CurrentValue = GearsConversions.FromInt(modConfigValue, ModConfig.MIN_LAND_CLAIM_COUNT, ModConfig.MAX_LAND_CLAIM_COUNT);
            GearsModAPI.SaveGlobalSettings();
        }
    }

    private static void ConfigureLandClaimSizeSetting(IGlobalModSettingsCategory category)
    {
        if (!TryGetGlobalValueSetting(category, nameof(ModConfig.LandClaimSize), out IGlobalValueSetting setting))
        {
            return;
        }

        setting.OnSettingChanged += SetLandClaimSize;
        SyncLandClaimSizeSetting(setting);
    }

    private static void SetLandClaimSize(IGlobalModSetting setting, string newValue)
    {
        var value = GearsConversions.ToInt(newValue, ModConfig.DEFAULT_LANDCLAIM_SIZE);
        var oldValue = ModConfig.Config.LandClaimSize;

        if (oldValue != value)
        {
            ModConfig.Config.LandClaimSize = value;
            ModConfig.SaveConfig();
        }
    }

    private static void SyncLandClaimSizeSetting(IGlobalValueSetting setting)
    {
        var modConfigValue = ModConfig.Config.LandClaimSize;

        if (!GearsConversions.IsEqualValue(setting.CurrentValue, modConfigValue))
        {
            setting.CurrentValue = GearsConversions.FromInt(modConfigValue, ModConfig.MIN_LAND_CLAIM_SIZE, ModConfig.MAX_LAND_CLAIM_SIZE);
            GearsModAPI.SaveGlobalSettings();
        }
    }

    internal static void ConfigureZoneCategorySettings(IGlobalModSettingsCategory category)
    {
        ConfigureZoneControlSizeSetting(category);
    }

    private static void ConfigureZoneControlSizeSetting(IGlobalModSettingsCategory category)
    {
        if (!TryGetGlobalValueSetting(category, nameof(ModConfig.ZoneControlSize), out IGlobalValueSetting setting))
        {
            return;
        }

        setting.OnSettingChanged += SetZoneControlSize;
        SyncZoneControlSizeSetting(setting);
    }

    private static void SetZoneControlSize(IGlobalModSetting setting, string newValue)
    {
        var value = GearsConversions.ToInt(newValue, ModConfig.DEFAULT_ZONE_CONTROL_SIZE);
        var oldValue = ModConfig.Config.ZoneControlSize;

        if (oldValue != value)
        {
            ModConfig.Config.ZoneControlSize = value;
            ModConfig.SaveConfig();
        }
    }

    private static void SyncZoneControlSizeSetting(IGlobalValueSetting setting)
    {
        var modConfigValue = ModConfig.Config.ZoneControlSize;

        if (!GearsConversions.IsEqualValue(setting.CurrentValue, modConfigValue))
        {
            setting.CurrentValue = GearsConversions.FromInt(modConfigValue, ModConfig.MIN_ZONE_CONTROL_SIZE, ModConfig.MAX_ZONE_CONTROL_SIZE);
            GearsModAPI.SaveGlobalSettings();
        }
    }
}