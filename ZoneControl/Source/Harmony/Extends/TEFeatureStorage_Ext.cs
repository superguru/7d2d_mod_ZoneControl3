using HarmonyLib;
using ZoneControl.Game.ZoneClaim;

namespace ZoneControl.Harmony.Extends;

[HarmonyPatch(typeof(TEFeatureStorage))]
internal static class TEFeatureStorage_Ext
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(TEFeatureStorage.UpdateTick))]
    private static bool SkipLootRespawnInZone(TEFeatureStorage __instance)
    {
        var position = __instance.ToWorldPos();
        return !ZoneClaimRegistry.DoesAnyZoneOverlap(position, position);
    }
}
