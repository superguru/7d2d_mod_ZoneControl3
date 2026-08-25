using ZoneControl.Configuration;
using ZoneControl.Game.Navigation;

namespace ZoneControl.Synchronisation;

internal static class SinglePlayerTasks
{
    internal static void HandleGameStartTasks()
    {
        HandleHideLandClaimFromCompassOnStart();
        HandleHideSleepingBagFromCompassOnStart();
    }

    private static void HandleHideLandClaimFromCompassOnStart()
    {
        if (ModConfig.HideLandClaimsFromCompassOnStart())
        {
            NavObjectControl.HideLandClaimsFromCompass();
        }
    }

    private static void HandleHideSleepingBagFromCompassOnStart()
    {
        if (ModConfig.HideSleepingBagsFromCompassOnStart())
        {
            NavObjectControl.HideSleepingBagsFromCompass();
        }
    }
}
