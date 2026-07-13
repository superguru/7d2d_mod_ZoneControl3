using HarmonyLib;
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

        var navObject = __instance?.closestMouseOverNavObject;
        if (navObject == null)
        {
            ModLogger.DebugLog($"{d_MethodName}: Closest NavObject is empty");
            return;
        }

        var name = navObject.name;
        if (string.IsNullOrEmpty(name))
        {
            name = navObject.localizedName;
            if (!string.IsNullOrEmpty(name))
            {
                name = $"[LN] {name}";
            }
            else
            {
                name = navObject.HiddenDisplayName;
                if (!string.IsNullOrEmpty(name))
                {
                    name = $"[HN] {name}";
                }
                else
                {
                    name = navObject.NavObjectClass?.NavObjectClassName;
                    if (!string.IsNullOrEmpty(name))
                    {
                        name = $"[NC] {name}";
                    }
                    else
                    {
                        name = $"Unknown NavObject/Class type {navObject.GetType()} for |{navObject}|";
                    }
                }
            }
        }

        var isHidden = navObject.hiddenOnCompass;
        var newCompassState = isHidden ? "hidden" : "showing";

        ModLogger.DebugLog($"{d_MethodName}: Nav Object {name} compass state set to {newCompassState}");
    }
}
