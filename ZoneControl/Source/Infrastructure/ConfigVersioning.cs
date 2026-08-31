namespace ZoneControl.Infrastructure;

internal class ConfigVersioning
{
    /// <summary>
    /// The first version to include versioning (3.2.0)
    /// </summary>
    public const string FirstVersionedConfig = "3.2.0";

    /// <summary>
    /// Current config schema version - always matches ModInfo.Version (lazy loaded)
    /// </summary>
    public static string CurrentVersion
    {
        get
        {
            if (string.IsNullOrEmpty(field))
            {
                field = ModInfo.Version;
            }
            return field;
        }
    } = null;
}