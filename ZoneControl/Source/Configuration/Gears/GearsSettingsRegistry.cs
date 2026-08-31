using System.Collections.Generic;

namespace ZoneControl.Configuration.Gears;

internal static class GearsSettingsRegistry
{
    private const string GeneralTab = "General";
    private const string MapTab = "Map";
    private const string LandClaimTab = "LandClaim";
    private const string ZoneTab = "Zone";

    private const string GeneralCategory = "General";
    private const string MapCategory = "Map";
    private const string LandClaimCategory = "LandClaim";
    private const string ZoneCategory = "Zone";

    public static readonly (string Tab, string Category, IReadOnlyList<IGearsSetting> Settings)[] Entries =
    [
        (GeneralTab, GeneralCategory,
        [
            GearsSettingFactory.Bool(
                nameof(ModConfig.IsDebug),
                c => c.isDebug,
                (c, v) => c.isDebug = v),
        ]),

        (MapTab, MapCategory,
        [
            GearsSettingFactory.Bool(
                nameof(ModConfig.HideLandClaimsFromCompassOnStart),
                c => c.hideLandClaimsFromCompassOnStart,
                (c, v) => c.hideLandClaimsFromCompassOnStart = v),
            GearsSettingFactory.Bool(
                nameof(ModConfig.HideSleepingBagsFromCompassOnStart),
                c => c.hideSleepingBagsFromCompassOnStart,
                (c, v) => c.hideSleepingBagsFromCompassOnStart = v),
        ]),

        (LandClaimTab, LandClaimCategory,
        [
            GearsSettingFactory.Int(
                nameof(ModConfig.LandClaimCount),
                c => c.landClaimCount,
                (c, v) => c.landClaimCount = v,
                ModConfig.MIN_LAND_CLAIM_COUNT, ModConfig.MAX_LAND_CLAIM_COUNT, ModConfig.DEFAULT_LANDCLAIM_COUNT),
            GearsSettingFactory.Int(
                nameof(ModConfig.LandClaimSize),
                c => c.landClaimSize,
                (c, v) => c.landClaimSize = v,
                ModConfig.MIN_LAND_CLAIM_SIZE, ModConfig.MAX_LAND_CLAIM_SIZE, ModConfig.DEFAULT_LANDCLAIM_SIZE),
        ]),

        (ZoneTab, ZoneCategory,
        [
            GearsSettingFactory.Int(
                nameof(ModConfig.ZoneControlSize),
                c => c.zoneControlSize,
                (c, v) => c.zoneControlSize = v,
                ModConfig.MIN_ZONE_CONTROL_SIZE, ModConfig.MAX_ZONE_CONTROL_SIZE, ModConfig.DEFAULT_ZONE_CONTROL_SIZE),
        ]),
    ];
}
