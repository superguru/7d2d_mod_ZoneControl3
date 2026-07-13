using HarmonyLib;
using ZoneControl.Game.Navigation;
using ZoneControl.Infrastructure;

namespace ZoneControl.Harmony.Extends;

[HarmonyPatch(typeof(XUiC_MapArea))]
#if DEBUG
[HarmonyDebug]
#endif
internal static class XUiC_MapArea_Ext
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(XUiC_MapArea.onMapPressedLeft), [typeof(XUiController), typeof(int)])]
#if DEBUG
    [HarmonyDebug]
#endif
    private static void XUiC_MapArea_onMapPressedLeft(XUiC_MapArea __instance, XUiController _sender, int _mouseButton)
    {
        const string d_MethodName = nameof(XUiC_MapArea_onMapPressedLeft);

        NavObject navObject = __instance?.closestMouseOverNavObject;
        string name = NavObjectControl.GetNavObjectName(navObject);

        var isHidden = navObject?.hiddenOnCompass ?? false;
        var newCompassState = isHidden ? "hidden" : "showing";

        ModLogger.DebugLog($"{d_MethodName}: Nav Object '{name}' compass state set to {newCompassState}");
    }
}
