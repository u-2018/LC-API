using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LC_API.Comp;
using LC_API.ManualPatches;
using LC_API.ServerAPI;
using System.Reflection;
using UnityEngine;

namespace LC_API
{
//.____    _________           _____  __________ .___  
//|    |   \_   ___ \         /  _  \ \______   \|   | 
//|    |   /    \  \/        /  /_\  \ |     ___/|   | 
//|    |___\     \____      /    |    \|    |    |   | 
//|_______ \\______  /______\____|__  /|____|    |___| 
//        \/       \//_____/        \/                 
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource Log;
        public static bool Initialized = false;
        private ConfigEntry<bool> configOverrideModServer;
        private ConfigEntry<bool> configLegacyAssetLoading;
        private ConfigEntry<bool> configDisableBundleLoader;
        private void Awake()
        {
            configOverrideModServer = Config.Bind("General","Force modded server browser",false,"Should the API force you into the modded server browser?");
            configLegacyAssetLoading = Config.Bind("General", "Legacy asset bundle loading", false, "Should the BundleLoader use legacy asset loading? Turning this on may help with loading assets from older plugins.");
            configDisableBundleLoader = Config.Bind("General", "Disable BundleLoader", false, "Should the BundleLoader be turned off? Enable this if you are having problems with mods that load assets using a different method from LC_API's BundleLoader.");


            Log = Logger;
            // Plugin startup logic
            Logger.LogWarning("\n.____    _________           _____  __________ .___  \r\n|    |   \\_   ___ \\         /  _  \\ \\______   \\|   | \r\n|    |   /    \\  \\/        /  /_\\  \\ |     ___/|   | \r\n|    |___\\     \\____      /    |    \\|    |    |   | \r\n|_______ \\\\______  /______\\____|__  /|____|    |___| \r\n        \\/       \\//_____/        \\/                 \r\n                                                     ");
            Logger.LogInfo($"LC_API Starting up..");
            if (configOverrideModServer.Value)
            {
                ModdedServer.SetServerModdedOnly();
            }

            Harmony harmony = new Harmony("ModAPI");
            MethodInfo original = AccessTools.Method(typeof(GameNetworkManager), "SteamMatchmaking_OnLobbyCreated");
            MethodInfo original2 = AccessTools.Method(typeof(GameNetworkManager), "LobbyDataIsJoinable");

            MethodInfo patch = AccessTools.Method(typeof(ManualPatches.ServerPatch), "OnLobbyCreate");

            MethodInfo original3 = AccessTools.Method(typeof(MenuManager), "Awake");

            MethodInfo patch3 = AccessTools.Method(typeof(ServerPatch), "Vers");

            MethodInfo original4 = AccessTools.Method(typeof(HUDManager), "AddChatMessage");

            MethodInfo patch4 = AccessTools.Method(typeof(ServerPatch), "ChatInterpreter");

            harmony.Patch(original3, new HarmonyMethod(patch3));
            harmony.Patch(original4, new HarmonyMethod(patch4));
            harmony.Patch(original, new HarmonyMethod(patch));
            
            Networking.GetString += CheatDatabase.RequestModList;
        }

        public void Start()
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
                gameObject.AddComponent<SVAPI>();
                Logger.LogInfo($"LC_API Started!");
                CheatDatabase.RunLocalCheatDetector();
            }
        }

        private void OnDestroy()
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
                gameObject.AddComponent<SVAPI>();
                Logger.LogInfo($"LC_API Started!");
                CheatDatabase.RunLocalCheatDetector();
            }
        }

        private static void PatchMethodManual(MethodInfo method, MethodInfo patch, Harmony harmony)
        {
            harmony.Patch(method, new HarmonyMethod(patch));
        }
    }
}