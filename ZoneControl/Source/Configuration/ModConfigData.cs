using Newtonsoft.Json;

namespace ZoneControl.Configuration;

internal sealed class ModConfigData
{
    #region Schematic
    [JsonProperty("metaDescription")]
    public string MetaDescription { get; set; } = string.Empty;

    [JsonProperty("version")]
    public string Version { get; set; } = string.Empty;
    #endregion

    #region Zone
    #endregion

    #region Zone
    [JsonProperty("zoneControlSize")]
    public int ZoneControlSize { get; set; } = ModConfig.DEFAULT_ZONE_CONTROL_SIZE;
    #endregion

    #region Land Claim
    [JsonProperty("landClaimCount")]
    public int LandClaimCount { get; set; } = ModConfig.DEFAULT_LANDCLAIM_COUNT;

    [JsonProperty("landClaimSize")]
    public int LandClaimSize { get; set; } = ModConfig.DEFAULT_LANDCLAIM_SIZE;
    #endregion

    #region Map
    [JsonProperty("hideLandClaimsFromCompassOnStart")]
    public bool HideLandClaimsFromCompassOnStart { get; set; } = true;

    [JsonProperty("hideSleepingBagsFromCompassOnStart")]
    public bool HideSleepingBagsFromCompassOnStart { get; set; } = true;
    #endregion

    #region General
    [JsonProperty("isDebug")]
    public bool IsDebug { get; set; } = false;
    #endregion
}