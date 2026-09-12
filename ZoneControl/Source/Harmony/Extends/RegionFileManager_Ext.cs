using HarmonyLib;
using ZoneControl.Game.ZoneClaim;

namespace ZoneControl.Harmony.Extends;

[HarmonyPatch(typeof(RegionFileManager))]
internal static class RegionFileManager_Ext
{
    private const ChunkProtectionLevel ZoneControlProtection = (ChunkProtectionLevel)0x20000;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(RegionFileManager.UpdateChunkProtectionLevels))]
    private static void AddZoneClaimProtection(RegionFileManager __instance)
    {
        var zones = ZoneClaimRegistry.GetAllZones();
        for (int i = 0; i < zones.Count; i++)
        {
            var zone = zones[i];

            int minChunkX = World.toChunkXZ(zone.Min.x);
            int maxChunkX = World.toChunkXZ(zone.Max.x);
            int minChunkZ = World.toChunkXZ(zone.Min.z);
            int maxChunkZ = World.toChunkXZ(zone.Max.z);

            for (int cx = minChunkX; cx <= maxChunkX; cx++)
            {
                for (int cz = minChunkZ; cz <= maxChunkZ; cz++)
                {
                    long key = WorldChunkCache.MakeChunkKey(cx, cz);
                    __instance.chunkProtectionLevels.TryGetValue(key, out var existing);
                    __instance.chunkProtectionLevels[key] = existing | ZoneControlProtection;
                }
            }
        }
    }
}
