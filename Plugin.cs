namespace LC_API;

using System.Reflection;

// ReSharper disable RedundantNameQualifier
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LC_API.Comp;
using LC_API.ManualPatches;
using LC_API.ServerAPI;
using UnityEngine;

/// <inheritdoc />
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    /// <summary>
    /// Gets the <see cref="ManualLogSource"/> file for the plugin.
    /// </summary>
#pragma warning disable SA1401
    public static ManualLogSource Log;
#pragma warning restore SA1401

    // ReSharper disable once InconsistentNaming
    private ConfigEntry<bool> configOverrideModServer;

    /// <summary>
    /// Called when the plugin awakes.
    /// </summary>
    private void Awake()
    {
        this.configOverrideModServer = this.Config.Bind(
            "General",
            "Force modded server browser",
            false,
            "Should the API force you into the modded server browser?");

        Log = this.Logger;

        // Plugin startup logic
        this.Logger.LogInfo($"LC-API Starting up..");
        if (this.configOverrideModServer.Value)
        {
            ModdedServer.SetServerModdedOnly();
        }

        // ReSharper disable StringLiteralTypo
        Harmony harmony = new Harmony("ModAPI");
        MethodInfo original = AccessTools.Method(typeof(GameNetworkManager), "SteamMatchmaking_OnLobbyCreated");
        MethodInfo original2 = AccessTools.Method(typeof(GameNetworkManager), "LobbyDataIsJoinable");

        MethodInfo patch = AccessTools.Method(typeof(ManualPatches.ServerPatch), "OnLobbyCreate");
        MethodInfo patch2 = AccessTools.Method(typeof(ManualPatches.ServerPatch), "LobbyIsJoinable");

        MethodInfo original3 = AccessTools.Method(typeof(MenuManager), "Awake");

        MethodInfo patch3 = AccessTools.Method(typeof(ServerPatch), "Vers");

        // ReSharper disable once UnusedVariable
        MethodInfo original4 = AccessTools.Method(typeof(HUDManager), "AddChatMessage");

        // ReSharper disable once UnusedVariable
        MethodInfo patch4 = AccessTools.Method(typeof(ServerPatch), "ChatInterpreter");

        harmony.Patch(original3, new HarmonyMethod(patch3));

        harmony.Patch(original, new HarmonyMethod(patch));
        harmony.Patch(original2, new HarmonyMethod(patch2));
    }

    private void OnDestroy()
    {
        BundleAPI.BundleLoader.Load();
        GameObject apiGameObject = new GameObject("API");
        DontDestroyOnLoad(apiGameObject);
        apiGameObject.AddComponent<SVAPI>();
    }
}