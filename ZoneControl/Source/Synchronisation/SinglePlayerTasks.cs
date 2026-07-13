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
        if (ModConfig.HideLandClaimFromCompassOnStart())
        {
            NavObjectControl.HideLandClaimFromCompass();
        }
    }

    private static void HandleHideSleepingBagFromCompassOnStart()
    {
        if (ModConfig.HideSleepingBagFromCompassOnStart())
        {
            NavObjectControl.HideSleepingBagFromCompass();
        }
    }
}
