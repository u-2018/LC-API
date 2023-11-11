namespace LC_API.ServerAPI;

// Field should be made private.
#pragma warning disable SA1401

/// <summary>
/// A class holding information about the modded server.
/// </summary>
// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
public sealed class ModdedServer
{
    /// <summary>
    /// Used to set if only modded players can join.
    /// </summary>
    public static bool SetModdedOnly;

    /// <summary>
    /// A backing field value for <see cref="ModdedOnly"/>, indicating whether only modded players join.
    /// </summary>
    private static bool moddedOnly;

    /// <summary>
    /// Gets a value indicating whether only modded players are able to join.
    /// </summary>
    public static bool ModdedOnly
    {
        get { return moddedOnly; }
    }

    /// <summary>
    /// Call this method to make your plugin place the user in modded only servers.
    /// </summary>
    public static void SetServerModdedOnly()
    {
        moddedOnly = true;
        Plugin.Log.LogMessage("A plugin has set your game to only allow you to play with other people who have mods!");
    }

    /// <summary>
    /// For internal use. Do not call this method.
    /// </summary>
    public static void OnSceneLoaded()
    {
        if (GameNetworkManager.Instance && ModdedOnly)
        {
            GameNetworkManager.Instance.gameVersionNum += 16440;
            SetModdedOnly = true;
        }
    }
}