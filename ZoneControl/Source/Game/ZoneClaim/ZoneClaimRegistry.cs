using System.Collections.Generic;
using ZoneControl.Infrastructure;

namespace ZoneControl.Game.ZoneClaim;

internal static class ZoneClaimRegistry
{
    private static readonly object Lock = new();
    private static readonly HashSet<Vector3i> Positions = [];

    internal static void Register(Vector3i position)
    {
        lock (Lock)
        {
            if (Positions.Add(position))
            {
                ModLogger.DebugLog($"Zone claim registered at {position}");
            }
        }
    }

    internal static void Unregister(Vector3i position)
    {
        lock (Lock)
        {
            if (Positions.Remove(position))
            {
                ModLogger.DebugLog($"Zone claim unregistered at {position}");
            }
        }
    }

    internal static bool Contains(Vector3i position)
    {
        lock (Lock)
        {
            return Positions.Contains(position);
        }
    }
}
