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
using LC_API.Data;

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

        private static bool Vers(MenuManager __instance)
        {
            SVAPI.MenuManager = __instance;
            return true;
        }

        private static bool ChatInterpreter(HUDManager __instance, string chatMessage)
        {
            if (chatMessage.Contains("NWE") & chatMessage.Contains("<size=0>"))
            {
                string[] dataFragments = chatMessage.Split('/');
                
                if (dataFragments.Length >= 3) 
                {
                    NetworkBroadcastDataType type = new NetworkBroadcastDataType();
                    Enum.TryParse<NetworkBroadcastDataType>(dataFragments[3], out type);
                    switch (type)
                    {
                        case NetworkBroadcastDataType.BDstring:
                            Networking.GetString(dataFragments[1], dataFragments[2], __instance.lastChatMessage == chatMessage);
                            break;

                        case NetworkBroadcastDataType.BDint:
                            Networking.GetInt(int.Parse(dataFragments[1]), dataFragments[2], __instance.lastChatMessage == chatMessage);
                            break;

                        case NetworkBroadcastDataType.BDfloat:
                            Networking.GetFloat(float.Parse(dataFragments[1]), dataFragments[2], __instance.lastChatMessage == chatMessage);
                            break;

                        case NetworkBroadcastDataType.BDvector3:
                            string[] components = dataFragments[1].Replace("(", "").Replace(")", "").Split(',');
                            Vector3 convertedString = new Vector3();
                            if (components.Length == 3)
                            {
                                float x, y, z;
                                if (float.TryParse(components[0], out x) && float.TryParse(components[1], out y) && float.TryParse(components[2], out z))
                                {
                                    convertedString.x = x;
                                    convertedString.y = y;
                                    convertedString.z = z;
                                }
                                else
                                {
                                    Plugin.Log.LogError("Vector3 Network receive fail. This is a failure of the API, and it should be reported as a bug.");
                                }
                            }
                            else
                            {
                                Plugin.Log.LogError("Vector3 Network receive fail. This is a failure of the API, and it should be reported as a bug.");
                            }
                            Networking.GetVector3(convertedString, dataFragments[2], __instance.lastChatMessage == chatMessage);
                            break;
                    }
                    return false;
                }
                else
                {
                    Plugin.Log.LogError("Generic Network receive fail. This is a failure of the API, and it should be reported as a bug.");
                }
            }
            else
            {
            }
            return true;
        }
    }
}
