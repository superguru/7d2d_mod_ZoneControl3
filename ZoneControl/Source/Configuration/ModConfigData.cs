using Newtonsoft.Json;

namespace ZoneControl.Configuration;

internal sealed class ModConfigData
{
    [JsonProperty("metaDescription")]
    public string MetaDescription { get; set; } = string.Empty;

    [JsonProperty("version")]
    public string Version { get; set; } = string.Empty;

    [JsonProperty("isDebug")]
    public bool IsDebug { get; set; } = false;

    [JsonProperty("hideLandClaimFromCompassOnStart")]
    public bool HideLandClaimFromCompassOnStart { get; set; } = true;

    [JsonProperty("hideSleepingBagFromCompassOnStart")]
    public bool HideSleepingBagFromCompassOnStart { get; set; } = true;
}