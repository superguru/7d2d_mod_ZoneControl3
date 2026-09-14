using HarmonyLib;
using ZoneControl.Game.ZoneClaim;

namespace ZoneControl.Harmony.Extends;

[HarmonyPatch(typeof(GameUtils))]
internal static class GameUtils_Ext
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(GameUtils.CheckForAnyPlayerHome))]
    private static bool CheckForZoneClaim(World world, Vector3i BoxMin, Vector3i BoxMax, ref GameUtils.EPlayerHomeType __result)
    {
        if (ZoneClaimRegistry.DoesAnyZoneOverlap(BoxMin, BoxMax))
        {
            __result = GameUtils.EPlayerHomeType.Landclaim;
            return false;
        }
        return true;
    }
}
