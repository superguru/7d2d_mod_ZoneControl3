using HarmonyLib;
using ZoneControl.Infrastructure;

namespace ZoneControl.Harmony.Extends;

[HarmonyPatch(typeof(GameManager))]
#if DEBUG
[HarmonyDebug]
#endif
internal static class GameManager_Ext
{
    private static readonly object s_lock = new();
    private static bool IsFirstSpawnDone { get; set; } = false;

    public delegate void OnSaveAndCleanupWorldDelegate(GameManager instance);
    public static event OnSaveAndCleanupWorldDelegate? OnSaveAndCleanupWorld;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(GameManager.SaveAndCleanupWorld))]
#if DEBUG
    [HarmonyDebug]
#endif
    private static void GameManager_SaveAndCleanupWorld(GameManager __instance)
    {
        const string d_MethodName = nameof(GameManager_SaveAndCleanupWorld);

        lock (s_lock)
        {
            OnSaveAndCleanupWorld?.Invoke(__instance);
#if DEBUG
            ModLogger.DebugLog($"{d_MethodName}: Game '{__instance}' game end events invoked");
#endif
        }
    }
}
