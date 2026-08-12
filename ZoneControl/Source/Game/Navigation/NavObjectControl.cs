using System.Collections.Generic;
using ZoneControl.Infrastructure;

namespace ZoneControl.Game.Navigation;

internal static class NavObjectControl
{
    internal const string LAND_CLAIM_NAV_OBJECT = "land_claim";
    internal const string SLEEPING_BAG_NAV_OBJECT = "sleeping_bag";

    internal static void HideLandClaimsFromCompass()
    {
        HideNavObjectsOnCompass(LAND_CLAIM_NAV_OBJECT);
    }

    internal static void HideSleepingBagsFromCompass()
    {
        HideNavObjectsOnCompass(SLEEPING_BAG_NAV_OBJECT);
    }

    private static void HideNavObjectsOnCompass(string className)
    {
        const string d_MethodName = nameof(HideNavObjectsOnCompass);

        var navObjects = GetNavObjectsByClassName(className);
        if (navObjects == null)
        {
            ModLogger.DebugLog($"{d_MethodName}: Could not find {className} on compass");
            return;
        }

        foreach (var navObject in navObjects)
        {

            var prevState = !navObject.hiddenOnCompass;
            navObject.hiddenOnCompass = true;
            var newState = !navObject.hiddenOnCompass;

            ModLogger.DebugLog($"{d_MethodName}: {className} compass visibility changed from {prevState} to {newState}");
        }
    }

    internal static IReadOnlyList<NavObject> GetLandClaimNavObjects()
    {
        return GetNavObjectsByClassName(LAND_CLAIM_NAV_OBJECT);
    }

    internal static IReadOnlyList<NavObject> GetSleepingBagNavObjects()
    {
        return GetNavObjectsByClassName(SLEEPING_BAG_NAV_OBJECT);
    }

    internal static IReadOnlyList<NavObject> GetNavObjectList()
    {
        return NavObjectManager.Instance?.NavObjectList;
    }

    internal static IReadOnlyList<NavObject> GetNavObjectsByClassName(string className)
    {
        if (string.IsNullOrEmpty(className) || string.IsNullOrWhiteSpace(className))
        {
            return []; // Return an empty list for invalid input
        }

        IReadOnlyList<NavObject> navObjects = GetNavObjectList();
        int maxNavObjects = navObjects?.Count ?? 0;

        var matchingObjects = new List<NavObject>(maxNavObjects);
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
                matchingObjects.Add(navObject);
            }
        }

        if (matchingObjects.Count == 0)
        {
            ModLogger.DebugLog($"Could not find NavObject({className}) in all {maxNavObjects} known NavObjects");
        }

        return matchingObjects;
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