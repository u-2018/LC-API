using LC_API.Comp;
using LC_API.Data;
using LC_API.Extensions;
using LC_API.ServerAPI;
using Steamworks;
using Steamworks.Data;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LC_API.ManualPatches
{
    internal static class ServerPatch
    {
        internal static bool OnLobbyCreate(GameNetworkManager __instance, Result result, Lobby lobby)
        {
            if (result != Result.OK)
            {
                Debug.LogError(string.Format("Lobby could not be created! {0}", result), __instance);
            }
            __instance.lobbyHostSettings.lobbyName = "[MODDED]" + __instance.lobbyHostSettings.lobbyName.ToString();
            Plugin.Log.LogMessage("server pre-setup success");
            return (true);
        }

        internal static bool CacheMenuManager(MenuManager __instance)
        {
            LC_APIManager.MenuManager = __instance;
            return true;
        }

        internal static bool ChatCommands(HUDManager __instance, InputAction.CallbackContext context)
        {
            if (__instance.chatTextField.text.ToLower().Contains("/modcheck"))
            {
                CheatDatabase.OtherPlayerCheatDetector();
                return false;
            }
            return true;
        }

        internal static void GameNetworkManagerAwake(GameNetworkManager __instance)
        {
            if (GameNetworkManager.Instance == null) ModdedServer.GameVersion = __instance.gameVersionNum;
        }
    }
}
