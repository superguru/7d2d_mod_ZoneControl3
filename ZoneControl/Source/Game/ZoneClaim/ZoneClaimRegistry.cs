using System.Collections.Generic;
using ZoneControl.Infrastructure;

namespace ZoneControl.Game.ZoneClaim;

internal static class ZoneClaimRegistry
{
    internal sealed class ZoneClaim
    {
        public Vector3i Min;
        public Vector3i Max;
    }

    private static readonly object Lock = new();
    private static readonly Dictionary<Vector3i, ZoneClaim> Zones = [];

    internal static void Register(Vector3i position, Vector3i min, Vector3i max)
    {
        lock (Lock)
        {
            Zones[position] = new ZoneClaim
            {
                Min = min,
                Max = max
            };

            ModLogger.DebugLog($"Zone claim registered at {position}");
        }
    }

    internal static void Unregister(Vector3i position)
    {
        lock (Lock)
        {
            if (Zones.Remove(position))
            {
                ModLogger.DebugLog($"Zone claim unregistered at {position}");
            }
        }
    }

    internal static bool Contains(Vector3i position)
    {
        lock (Lock)
        {
            return Zones.ContainsKey(position);
        }
    }

    internal static IReadOnlyList<ZoneClaim> GetAllZones()
    {
        lock (Lock)
        {
            return new List<ZoneClaim>(Zones.Values);
        }
    }

    internal static bool DoesAnyZoneOverlap(Vector3i min, Vector3i max)
    {
        lock (Lock)
        {
            foreach (var zone in Zones.Values)
            {
                if (zone.Min.x <= max.x && zone.Max.x >= min.x && zone.Min.z <= max.z && zone.Max.z >= min.z)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
