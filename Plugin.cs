using BepInEx;
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
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource Log;

        private ConfigEntry<bool> configOverrideModServer;
        private void Awake()
        {
            configOverrideModServer = Config.Bind("General",
                                                "Force modded server browser",
                                                false,
                                                "Should the API force you into the modded server browser?");


            Log = Logger;
            // Plugin startup logic
            Logger.LogInfo($"LC-API Starting up..");
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
        }

        private void OnDestroy()
        {
            BundleAPI.BundleLoader.Load();
            GameObject gameObject = new GameObject("API");
            DontDestroyOnLoad(gameObject);
            gameObject.AddComponent<SVAPI>();
        }
    }
}