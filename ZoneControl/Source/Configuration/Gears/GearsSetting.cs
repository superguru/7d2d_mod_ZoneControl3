using System;
using System.Collections.Generic;
using GearsAPI.Settings.Global;
using ZoneControl.Infrastructure;

namespace ZoneControl.Configuration.Gears;

internal interface IGearsSetting
{
    void Bind(IGlobalModSettingsCategory category);
}

internal sealed class GearsSetting<T> : IGearsSetting
{
    private readonly string _key;
    private readonly Func<ModConfigData, T> _getConfig;
    private readonly Action<ModConfigData, T> _setConfig;
    private readonly Func<string, T> _parse;
    private readonly Func<T, string> _format;

    public GearsSetting(string key, Func<ModConfigData, T> getConfig, Action<ModConfigData, T> setConfig, Func<string, T> parse, Func<T, string> format)
    {
        _key = key;
        _getConfig = getConfig;
        _setConfig = setConfig;
        _parse = parse;
        _format = format;
    }

    public void Bind(IGlobalModSettingsCategory category)
    {
        if (category.GetSetting(_key) is not IGlobalValueSetting setting)
        {
            ModLogger.DebugLog($"Global settings loaded, but setting `{_key}` is null");
            return;
        }

        setting.OnSettingChanged += (_, newValue) =>
        {
            var value = _parse(newValue);

            if (!EqualityComparer<T>.Default.Equals(_getConfig(ModConfig.Config), value))
            {
                _setConfig(ModConfig.Config, value);
                ModConfig.SaveConfig();
            }
        };

        Sync(setting);
    }

    private void Sync(IGlobalValueSetting setting)
    {
        var configValue = _getConfig(ModConfig.Config);

        if (!EqualityComparer<T>.Default.Equals(_parse(setting.CurrentValue), configValue))
        {
            setting.CurrentValue = _format(configValue);
            GearsModAPI.SaveGlobalSettings();
        }
    }
}

internal static class GearsSettingFactory
{
    internal static GearsSetting<bool> Bool(string key, Func<ModConfigData, bool> getConfig, Action<ModConfigData, bool> setConfig)
    {
        return new GearsSetting<bool>(
            key,
            getConfig,
            setConfig,
            value => GearsConversions.ToBool(value, false),
            GearsConversions.FromBool);
    }

    internal static GearsSetting<int> Int(string key, Func<ModConfigData, int> getConfig, Action<ModConfigData, int> setConfig, int min, int max, int defaultValue)
    {
        return new GearsSetting<int>(
            key,
            getConfig,
            setConfig,
            value => GearsConversions.ToInt(value, defaultValue),
            value => GearsConversions.FromInt(value, min, max));
    }
}
