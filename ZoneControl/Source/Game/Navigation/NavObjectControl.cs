using System.Collections.Generic;
using ZoneControl.Infrastructure;

namespace ZoneControl.Game.Navigation;

internal static class NavObjectControl
{
    internal const string LAND_CLAIM_NAV_OBJECT = "land_claim";
    internal const string SLEEPING_BAG_NAV_OBJECT = "sleeping_bag";

    internal static void HideLandClaimFromCompass()
    {
        HideNavObjectOnCompass(LAND_CLAIM_NAV_OBJECT);
    }

    internal static void HideSleepingBagFromCompass()
    {
        HideNavObjectOnCompass(SLEEPING_BAG_NAV_OBJECT);
    }

    private static void HideNavObjectOnCompass(string className)
    {
        const string d_MethodName = nameof(HideNavObjectOnCompass);

        var navObject = GetNavObjectByClassName(className);
        if (navObject == null)
        {
            ModLogger.DebugLog($"{d_MethodName}: Could not find {className} on compass");
            return;
        }

        var prevState = navObject.hiddenOnCompass;
        navObject.hiddenOnCompass = true;
        var newState = navObject.hiddenOnCompass;

        ModLogger.DebugLog($"{d_MethodName}: {className} compass visibility changed from {prevState} to {newState}");
    }

    internal static NavObject GetLandClaimNavObject()
    {
        return GetNavObjectByClassName(LAND_CLAIM_NAV_OBJECT);
    }

    internal static NavObject GetSleepingBagNavObject()
    {
        return GetNavObjectByClassName(SLEEPING_BAG_NAV_OBJECT);
    }

    internal static IReadOnlyList<NavObject> GetNavObjectList()
    {
        return NavObjectManager.Instance?.NavObjectList;
    }

    internal static NavObject GetNavObjectByClassName(string className)
    {
        if (string.IsNullOrEmpty(className) || string.IsNullOrWhiteSpace(className))
        {
            return null;
        }

        IReadOnlyList<NavObject> navObjects = GetNavObjectList();
        int maxNavObjects = navObjects?.Count ?? 0;
        for (int i = 0; i < maxNavObjects; i++)
        {
            var navObject = navObjects[i];
            if (navObject == null)
            {
                continue;
            }

            string navObjectClassName = GetNavObjectClassName(navObject);

            if (className.Equals(navObjectClassName, System.StringComparison.InvariantCulture))
            {
                return navObject;
            }
        }

        ModLogger.DebugLog($"Could not find NavObject({className}) in all {maxNavObjects} known NavObjects");
        return null;  // Not found
    }

    internal static string GetNavObjectClassName(NavObject navObject)
    {
        return navObject?.NavObjectClass?.NavObjectClassName;
    }

    internal static string GetNavObjectName(NavObject navObject)
    {
        if (navObject == null)
        {
            return "null";
        }

        string name = navObject.name;
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

        return name;
    }
}