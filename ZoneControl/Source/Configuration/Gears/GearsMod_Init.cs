using GearsAPI.Settings;
using GearsAPI.Settings.Global;
using GearsAPI.Settings.World;
using ZoneControl.Infrastructure;

namespace ZoneControl.Configuration.Gears;

public class GearsModAPI : IGearsModApi
{
    private static IGearsMod s_gearsMod;

    private static IModGlobalSettings GearsGlobalSettings
    {
        get; set;
    }

    void IGearsModApi.InitMod(IGearsMod modInstance)
    {
        s_gearsMod = modInstance;
    }

    public static void SaveGlobalSettings()
    {
        GearsGlobalSettings?.SaveSettings();
    }

    void IGearsModApi.OnGlobalSettingsLoaded(IModGlobalSettings modSettings)
    {
        GearsGlobalSettings = modSettings;
        if (GearsGlobalSettings == null)
        {
            ModLogger.DebugLog($"Global settings loaded, but modSettings is null, and gears mod is {s_gearsMod}");
            return;
        }

        ConfigureGeneralCategory();
        ConfigureMapCategory();
        ConfigureLandClaimCategory();
        ConfigureZoneCategory();
    }

    private void ConfigureGeneralCategory()
    {
        var generalTab = GearsGlobalSettings.GetTab("General");
        if (generalTab == null)
        {
            ModLogger.DebugLog($"Global settings loaded, but generalTab is null");
            return;
        }

        var generalCategory = generalTab.GetCategory("General");
        if (generalCategory == null)
        {
            ModLogger.DebugLog($"Global settings loaded, but generalCategory is null");
            return;
        }

        GearsGeneralSettings.ConfigureGeneralCategorySettings(generalCategory);
    }

    private void ConfigureMapCategory()
    {
        var mapTab = GearsGlobalSettings.GetTab("Map");
        if (mapTab == null)
        {
            ModLogger.DebugLog($"Global settings loaded, but mapTab is null");
            return;
        }

        var mapCategory = mapTab.GetCategory("Map");
        if (mapCategory == null)
        {
            ModLogger.DebugLog($"Global settings loaded, but mapCategory is null");
            return;
        }

        GearsGeneralSettings.ConfigureMapCategorySettings(mapCategory);
    }

    private void ConfigureLandClaimCategory()
    {
        var landClaimTab = GearsGlobalSettings.GetTab("LandClaim");
        if (landClaimTab == null)
        {
            ModLogger.DebugLog($"Global settings loaded, but landClaimTab is null");
            return;
        }

        var landClaimCategory = landClaimTab.GetCategory("LandClaim");
        if (landClaimCategory == null)
        {
            ModLogger.DebugLog($"Global settings loaded, but landClaimCategory is null");
            return;
        }

        GearsGeneralSettings.ConfigureLandClaimCategorySettings(landClaimCategory);
    }

    private void ConfigureZoneCategory()
    {
        var zoneTab = GearsGlobalSettings.GetTab("Zone");
        if (zoneTab == null)
        {
            ModLogger.DebugLog($"Global settings loaded, but zoneTab is null");
            return;
        }

        var zoneCategory = zoneTab.GetCategory("Zone");
        if (zoneCategory == null)
        {
            ModLogger.DebugLog($"Global settings loaded, but zoneCategory is null");
            return;
        }

        GearsGeneralSettings.ConfigureZoneCategorySettings(zoneCategory);
    }

    void IGearsModApi.OnWorldSettingsLoaded(IModWorldSettings worldSettings)
    {
        // NOP
    }
}
