using Newtonsoft.Json;

namespace ZoneControl.Configuration;

internal sealed class ModConfigData
{
    #region Schematic
    [JsonProperty(nameof(metaDescription))]
    public string metaDescription { get; set; } = string.Empty;

    [JsonProperty(nameof(version))]
    public string version { get; set; } = string.Empty;
    #endregion

    #region Zone
    [JsonProperty(nameof(zoneControlSize))]
    public int zoneControlSize { get; set; } = ModConfig.DEFAULT_ZONE_CONTROL_SIZE;
    #endregion

    #region Land Claim
    [JsonProperty(nameof(landClaimCount))]
    public int landClaimCount { get; set; } = ModConfig.DEFAULT_LANDCLAIM_COUNT;

    [JsonProperty(nameof(landClaimSize))]
    public int landClaimSize { get; set; } = ModConfig.DEFAULT_LANDCLAIM_SIZE;
    #endregion

    #region Map
    [JsonProperty(nameof(hideLandClaimsFromCompassOnStart))]
    public bool hideLandClaimsFromCompassOnStart { get; set; } = true;

    [JsonProperty(nameof(hideSleepingBagsFromCompassOnStart))]
    public bool hideSleepingBagsFromCompassOnStart { get; set; } = true;
    #endregion

    #region General
    [JsonProperty(nameof(isDebug))]
    public bool isDebug { get; set; } = false;
    #endregion
}