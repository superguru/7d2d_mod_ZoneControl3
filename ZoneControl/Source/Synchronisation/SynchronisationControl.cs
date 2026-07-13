using ZoneControl.Infrastructure;
using static ModEvents;

namespace ZoneControl.Synchronisation;

internal class SynchronisationControl
{
    public static void PlayerSpawnedInWorld(ref SPlayerSpawnedInWorldData data)
    {
        string spawnDescription = $"client {data.ClientInfo}; isLocalPlayer {data.IsLocalPlayer}; entityId {data.EntityId}; respawn type {data.RespawnType}; pos {data.Position}";

        if (IsSinglePlayer())
        {
            ModLogger.DebugLog($"Single player spawned into the world: {spawnDescription}");

            // Handle tasks for single player joining game
            // At this point, the spawn is not yet compelete
            // It's only complete when EntityPlayerLocal_onNewBiomeEntered is called for the first time
        }

        if (ShouldProcessPlayerSpawn(data))
        {
            ModLogger.DebugLog($"Handling player spwan: {spawnDescription}");

            // Handle the synchronisation tasks
        }
    }

    private static bool IsSinglePlayer()
    {
        var connectionManager = SingletonMonoBehaviour<ConnectionManager>.Instance;
        if (connectionManager == null)
        {
            ModLogger.Warning($"IsSinglePlayer: ConnectionManager is null, so there's a problem with this system");
            return true;  // Safest default?
        }

        return connectionManager.IsSinglePlayer;
    }

    private static bool ShouldProcessPlayerSpawn(SPlayerSpawnedInWorldData data)
    {
        var connectionManager = SingletonMonoBehaviour<ConnectionManager>.Instance;
        if (connectionManager == null)
        {
            return false;
        }

        return connectionManager.IsServer && !connectionManager.IsSinglePlayer && data.ClientInfo != null;
    }
}