using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;
using BepInEx;
using UnityEngine;
using Steamworks;
using Steamworks.Data;
using HarmonyLib;
using LC_API.ServerAPI;
using LC_API.Comp;

namespace LC_API.ManualPatches
{
    internal class ServerPatch
    {
        private static bool OnLobbyCreate(GameNetworkManager __instance, Result result, Lobby lobby)
        {
            if (result != Result.OK)
            {
                Debug.LogError(string.Format("Lobby could not be created! {0}", result), __instance);
            }
            __instance.lobbyHostSettings.lobbyName = "[MODDED]" + __instance.lobbyHostSettings.lobbyName.ToString();
            //lobby.SetData("vers", "This server requires mods. " + GameNetworkManager.Instance.gameVersionNum.ToString());
            Plugin.Log.LogMessage("server pre-setup success");
            return (true);
        }

        private static void LobbyIsJoinable()
        {

        }

        private static bool Vers(MenuManager __instance)
        {
            SVAPI.MenuManager = __instance;
            return true;
        }
    }
}
