using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LC_API.ClientAPI;
using LC_API.Comp;
using LC_API.GameInterfaceAPI.Events;
using LC_API.ManualPatches;
using LC_API.Networking;
using LC_API.Networking.Serializers;
using LC_API.ServerAPI;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace LC_API
{
    // .____    _________           _____  __________ .___  
    // |    |   \_   ___ \         /  _  \ \______   \|   | 
    // |    |   /    \  \/        /  /_\  \ |     ___/|   | 
    // |    |___\     \____      /    |    \|    |    |   | 
    // |_______ \\______  /______\____|__  /|____|    |___| 
    //         \/       \//_____/        \/                 
    /// <summary>
    /// The Lethal Company modding API plugin!
    /// </summary>
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public sealed class Plugin : BaseUnityPlugin
    {
        internal static Plugin Instance { get; private set; }
        /// <summary>
        /// Runs after the LC API plugin's "Awake" method is finished.
        /// </summary>
        public static bool Initialized { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        internal static ManualLogSource Log;

        private ConfigEntry<bool> configOverrideModServer;
        private ConfigEntry<bool> configLegacyAssetLoading;
        private ConfigEntry<bool> configDisableBundleLoader;
        internal static ConfigEntry<bool> configVanillaSupport;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


        internal static Harmony Harmony;

        private void Awake()
        {
            Instance = this;
            configOverrideModServer = Config.Bind("General", "Force modded server browser", false, "Should the API force you into the modded server browser?");
            configLegacyAssetLoading = Config.Bind("General", "Legacy asset bundle loading", false, "Should the BundleLoader use legacy asset loading? Turning this on may help with loading assets from older plugins.");
            configDisableBundleLoader = Config.Bind("General", "Disable BundleLoader", false, "Should the BundleLoader be turned off? Enable this if you are having problems with mods that load assets using a different method from LC_API's BundleLoader.");
            configVanillaSupport = Config.Bind("Compatibility", "Vanilla Compatibility", false, "Allows you to join vanilla servers, but disables many networking-related things and could cause mods to not work properly.");
            CommandHandler.commandPrefix = Config.Bind("General", "Prefix", "/", "Command prefix");

            Log = Logger;
            // Plugin startup logic
            Logger.LogWarning("\n.____    _________           _____  __________ .___  \r\n|    |   \\_   ___ \\         /  _  \\ \\______   \\|   | \r\n|    |   /    \\  \\/        /  /_\\  \\ |     ___/|   | \r\n|    |___\\     \\____      /    |    \\|    |    |   | \r\n|_______ \\\\______  /______\\____|__  /|____|    |___| \r\n        \\/       \\//_____/        \\/                 \r\n                                                     ");
            Logger.LogInfo($"LC_API Starting up..");
            if (configOverrideModServer.Value)
            {
                ModdedServer.SetServerModdedOnly();
            }

            if (configVanillaSupport.Value) Logger.LogInfo("LC_API is starting with VANILLA SUPPORT ENABLED.");

            Harmony = new Harmony("ModAPI");
            MethodInfo originalLobbyCreated = AccessTools.Method(typeof(GameNetworkManager), "SteamMatchmaking_OnLobbyCreated");
            MethodInfo originalLobbyJoinable = AccessTools.Method(typeof(GameNetworkManager), "LobbyDataIsJoinable");

            MethodInfo patchLobbyCreate = AccessTools.Method(typeof(ServerPatch), nameof(ServerPatch.OnLobbyCreate));

            MethodInfo originalMenuAwake = AccessTools.Method(typeof(MenuManager), "Awake");

            MethodInfo patchCacheMenuMgr = AccessTools.Method(typeof(ServerPatch), nameof(ServerPatch.CacheMenuManager));

            MethodInfo originalAddChatMsg = AccessTools.Method(typeof(HUDManager), "AddChatMessage");

            MethodInfo originalSubmitChat = AccessTools.Method(typeof(HUDManager), "SubmitChat_performed");

            MethodInfo patchSubmitChat = AccessTools.Method(typeof(CommandHandler.SubmitChatPatch), nameof(CommandHandler.SubmitChatPatch.Transpiler));

            MethodInfo originalGameManagerAwake = AccessTools.Method(typeof(GameNetworkManager), nameof(GameNetworkManager.Awake));

            MethodInfo patchGameManagerAwake = AccessTools.Method(typeof(ServerPatch), nameof(ServerPatch.GameNetworkManagerAwake));

            MethodInfo originalStartClient = AccessTools.Method(typeof(NetworkManager), nameof(NetworkManager.StartClient));
            MethodInfo originalStartHost = AccessTools.Method(typeof(NetworkManager), nameof(NetworkManager.StartHost));
            MethodInfo originalShutdown = AccessTools.Method(typeof(NetworkManager), nameof(NetworkManager.Shutdown));

            MethodInfo registerPatch = AccessTools.Method(typeof(RegisterPatch), nameof(RegisterPatch.Postfix));
            MethodInfo unregisterPatch = AccessTools.Method(typeof(UnregisterPatch), nameof(UnregisterPatch.Postfix));

            Harmony.Patch(originalMenuAwake, new HarmonyMethod(patchCacheMenuMgr));
            Harmony.Patch(originalLobbyCreated, new HarmonyMethod(patchLobbyCreate));
            Harmony.Patch(originalSubmitChat, null, null, new HarmonyMethod(patchSubmitChat));
            Harmony.Patch(originalGameManagerAwake, new HarmonyMethod(patchGameManagerAwake));

            Harmony.Patch(originalStartClient, null, new HarmonyMethod(registerPatch));
            Harmony.Patch(originalStartHost, null, new HarmonyMethod(registerPatch));

            Harmony.Patch(originalShutdown, null, new HarmonyMethod(unregisterPatch));

            Network.Init();
            Events.Patch(Harmony);
        }

        internal void Start()
        {
            Initialize();
        }

        internal void OnDestroy()
        {
            Initialize();
        }

        internal void Initialize()
        {
            if (!Initialized)
            {
                Initialized = true;
                if (!configDisableBundleLoader.Value)
                {
                    BundleAPI.BundleLoader.Load(configLegacyAssetLoading.Value);
                }
                GameObject gameObject = new GameObject("API");
                DontDestroyOnLoad(gameObject);
                gameObject.AddComponent<LC_APIManager>();
                Logger.LogInfo($"LC_API Started!");
                CheatDatabase.RunLocalCheatDetector();
            }
        }

        internal static void PatchMethodManual(MethodInfo method, MethodInfo patch, Harmony harmony)
        {
            harmony.Patch(method, new HarmonyMethod(patch));
        }
    }
}