using System;
using System.Collections.Generic;
using System.Linq;
using ZoneControl.Infrastructure;

namespace ZoneControl.Harmony.Commands;

public class ConsoleCmdZcHelp : ConsoleCmdAbstract
{
    static ConsoleCmdZcHelp()
    {
        RegisterCommand();
    }

    public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
    {
        const string d_methodName = nameof(ConsoleCmdZcHelp);

        try
        {
#if DEBUG
            ModLogger.Info($"Executing {d_methodName}");
#endif
            ShowHelp();
        }
        catch (Exception e)
        {
            ModLogger.Error($"Error in {d_methodName}: {e.Message}", e);
        }
    }

    private void ShowHelp()
    {
        var allCommands = ZcCommandRegistry.GetAllCommands();

        if (allCommands.Count == 0)
        {
            ModLogger.Info("No console commands are currently registered.");
            return;
        }

        ModLogger.Info($"Commands ({allCommands.Count} available):");
        ModLogger.Info("==========================================");

        // Find the longest command name for formatting
        int maxCommandLength = allCommands.Max(cmd => cmd.Name.Length);

        foreach (var commandInfo in allCommands)
        {
            // Format: "command    - description"
            var paddedCommand = commandInfo.Name.PadRight(maxCommandLength);
            ModLogger.Info($"  {paddedCommand} - {commandInfo.Description}");
        }

        ModLogger.Info("==========================================");
    }

    public override string[] getCommands()
    {
        return
        [
            "zchelp",
        ];
    }

    public override string getDescription()
    {
        return CmdDescription;
    }

    public static string CmdDescription { get; } = "Lists all available commands and their descriptions";

    private static void RegisterCommand()
    {
        ZcCommandRegistry.RegisterCommand("zchelp", CmdDescription);
    }
}