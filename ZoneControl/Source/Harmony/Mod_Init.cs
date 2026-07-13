using System.Reflection;
using ZoneControl.Configuration;
using ZoneControl.Synchronisation;

#if DEBUG
using HarmonyLib.Tools;
#endif

namespace ZoneControl;

public class ZoneControl3_Mod : IModApi
{
    public static ZoneControl3_Mod Context
    {
        get; set;
    }

    internal static Mod s_modInstance;
    private static string s_mod_assembly_path = "";

    internal static string GetModAssemblyPath()
    {
        return s_mod_assembly_path;
    }

    public void InitMod(Mod modInstance)
    {
        Context = this;
        s_mod_assembly_path = modInstance.Path;
        ModConfig.LoadConfig();
        s_modInstance = modInstance;
        var harmony = new HarmonyLib.Harmony(GetType().ToString());
#if DEBUG
        HarmonyFileLog.Enabled = true;
#endif
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        HookSynchronisationEvents();
    }

    private void HookSynchronisationEvents()
    {
        ModEvents.PlayerSpawnedInWorld.RegisterHandler(SynchronisationControl.PlayerSpawnedInWorld);
    }
}
