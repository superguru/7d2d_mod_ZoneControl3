using System;
using System.Collections.Generic;
using ZoneControl.Game.Navigation;
using ZoneControl.Infrastructure;

namespace ZoneControl.Harmony.Commands;

public class ConsoleCmdZcHideLandClaimNavObject : ConsoleCmdAbstract
{
    static ConsoleCmdZcHideLandClaimNavObject()
    {
        RegisterCommand();
    }

    public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
    {
        const string d_methodName = nameof(ConsoleCmdZcHideLandClaimNavObject);

        try
        {
#if DEBUG
            ModLogger.Info($"Executing {d_methodName}");
#endif
            NavObjectControl.HideLandClaimFromCompass();
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
            "zchidelandclaimnav",
        ];
    }

    public override string getDescription()
    {
        return CmdDescription;
    }

    public static string CmdDescription { get; } = "Hides the landclaim nav object on the compass";

    private static void RegisterCommand()
    {
        ZcCommandRegistry.RegisterCommand("zchidelandclaimnav", CmdDescription);
    }
}