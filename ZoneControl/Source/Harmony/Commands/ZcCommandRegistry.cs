using System.Collections.Generic;
using System.Linq;
using ZoneControl.Infrastructure;

namespace ZoneControl.Harmony.Commands;

internal class ZcCommandRegistry
{
    internal static Dictionary<string, CommandInfo> RegisteredCommands { get; } = [];

    public static void RegisterCommand(string commandName, string description, string usage = null)
    {
        if (string.IsNullOrEmpty(commandName))
        {
            return;
        }

        var commandInfo = new CommandInfo
        {
            Name = commandName.ToLowerInvariant(),
            Description = description ?? "No description available",
            Usage = usage ?? commandName
        };

        RegisteredCommands[commandInfo.Name] = commandInfo;
        ModLogger.Info($"Registered command: {commandName}");
    }

    public static List<CommandInfo> GetAllCommands()
    {
        return [.. RegisteredCommands.Values.OrderBy(x => x.Name)];
    }

    public static CommandInfo GetCommand(string commandName)
    {
        if (string.IsNullOrEmpty(commandName))
        {
            return null;
        }

        RegisteredCommands.TryGetValue(commandName.ToLowerInvariant(), out var commandInfo);
        return commandInfo;
    }

    public static bool IsCommandRegistered(string commandName)
    {
        return !string.IsNullOrEmpty(commandName) && RegisteredCommands.ContainsKey(commandName.ToLowerInvariant());
    }

    public static void UnregisterCommand(string commandName)
    {
        if (!string.IsNullOrEmpty(commandName) && RegisteredCommands.Remove(commandName.ToLowerInvariant()))
        {
            ModLogger.DebugLog($"Unregistered command: {commandName}");
        }
    }

    public static void ClearAllCommands()
    {
        RegisteredCommands.Clear();
        ModLogger.DebugLog(" All registered commands cleared");
    }

    public class CommandInfo
    {
        public string Name
        {
            get; set;
        }
        public string Description
        {
            get; set;
        }
        public string Usage
        {
            get; set;
        }
    }
}