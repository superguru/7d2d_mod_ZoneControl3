using System;
using System.Collections.Generic;
using ZoneControl.Game.Navigation;
using ZoneControl.Infrastructure;

namespace ZoneControl.Harmony.Commands;

public class ConsoleCmdZcHideSleepingBagNavObject : ConsoleCmdAbstract
{
    static ConsoleCmdZcHideSleepingBagNavObject()
    {
        RegisterCommand();
    }

    public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
    {
        const string d_methodName = nameof(ConsoleCmdZcHideSleepingBagNavObject);

        try
        {
#if DEBUG
            ModLogger.Info($"Executing {d_methodName}");
#endif
            NavObjectControl.HideSleepingBagFromCompass();
        }
        catch (Exception e)
        {
            ModLogger.Error($"Error in {d_methodName}: {e.Message}", e);
        }
    }

    public override string[] getCommands()
    {
        //TODO: Change this to be more elegant and shorter
        return
        [
            "zchidesleepingbagnav",
        ];
    }

    public override string getDescription()
    {
        return CmdDescription;
    }

    public static string CmdDescription { get; } = "Hides the sleeping bag nav object on the compass";

    private static void RegisterCommand()
    {
        ZcCommandRegistry.RegisterCommand("zchidesleepingbagnav", CmdDescription);
    }
}