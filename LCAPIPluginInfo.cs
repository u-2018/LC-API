namespace LC_API
{
    /// <summary>
    /// Contains information about LC_API.
    /// <para>If you use these values, note that they are <see langword="const"/> and will not change at runtime (unless you use reflection).</para>
    /// </summary>
    public static class LCAPIPluginInfo
    {
        /// <summary>
        /// Plugin's GUID as given to BepInEx
        /// </summary>
        public const string PLUGIN_GUID = "LC_API";
        /// <summary>
        /// Plugin's name as given to BepInEx
        /// </summary>
        public const string PLUGIN_NAME = "LC_API";
        /// <summary>
        /// Plugin's version as given to BepInEx
        /// </summary>
        public const string PLUGIN_VERSION = "1.0.0";
    }
}
