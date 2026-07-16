using HarmonyLib;
using ZoneControl.Infrastructure;
using ZoneControl.Synchronisation;
using static ZoneControl.Harmony.Extends.GameManager_Ext;

[HarmonyPatch(typeof(EntityPlayerLocal))]
#if DEBUG
[HarmonyDebug]
#endif
internal static class EntityPlayerLocal_Ext
{
    private static readonly object s_lock = new();
    private static bool IsFirstSpawnDone { get; set; } = false;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(EntityPlayerLocal.onNewBiomeEntered), [typeof(BiomeDefinition)])]
#if DEBUG
    [HarmonyDebug]
#endif
    private static void EntityPlayerLocal_onNewBiomeEntered(EntityPlayerLocal __instance, BiomeDefinition _biome)
    {
        const string d_MethodName = nameof(EntityPlayerLocal_onNewBiomeEntered);

        lock (s_lock)
        {
            if (IsFirstSpawnDone)
            {
                ModLogger.DebugLog($"{d_MethodName}: Local player has already spawned for the first time");
                return;
            }

            if (_biome == null)
            {
                // Various situations lead to this, but we can't count it as the right time to invoke game start events for single players
                return;
            }

            SinglePlayerTasks.HandleGameStartTasks();
            IsFirstSpawnDone = true;

#if DEBUG
            ModLogger.DebugLog($"{d_MethodName}: Player '{__instance.GetDebugName()}' game start events invoked");
#endif
        }
    }

    static EntityPlayerLocal_Ext()
    {
        OnSaveAndCleanupWorld += ResetFirstSpawnState;
    }

    private static void ResetFirstSpawnState(GameManager instance)
    {
        const string d_MethodName = nameof(ResetFirstSpawnState);

        lock (s_lock)
        {
            IsFirstSpawnDone = false;
#if DEBUG
            ModLogger.DebugLog($"{d_MethodName}: First spawn state reset for game '{instance}'");
#endif
        }
    }
}
