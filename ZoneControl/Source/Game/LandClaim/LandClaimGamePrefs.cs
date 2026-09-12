using ZoneControl.Configuration;
using ZoneControl.Infrastructure;

namespace ZoneControl.Game.LandClaim;

internal static class LandClaimGamePrefs
{
    internal static void OnGameStarting(ref ModEvents.SGameStartingData data)
    {
        if (!data.AsServer)
        {
            return;
        }

        ApplyGamePrefs();
    }

    private static void ApplyGamePrefs()
    {
        var landClaimCount = ModConfig.LandClaimCount();
        var landClaimSize = ModConfig.LandClaimSize();

        GamePrefs.Set(EnumGamePrefs.LandClaimCount, landClaimCount);
        GamePrefs.Set(EnumGamePrefs.LandClaimSize, landClaimSize);

        ModLogger.Info($"Applied land claim game prefs: count={landClaimCount}, size={landClaimSize}");
    }
}
