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
using System.Runtime.Remoting.Contexts;
using UnityEngine.InputSystem;

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
                    int parsedplayer;
                    bool success = int.TryParse(dataFragments[4], out parsedplayer);
                    if (!success)
                    {
                        Plugin.Log.LogWarning("Failed to parse player ID!!");
                        return false;
                    }
                    if (parsedplayer == (int)GameNetworkManager.Instance.localPlayerController.playerClientId & !SVAPI.netTester)
                    {
                        return false;
                    }
                    NetworkBroadcastDataType type = new NetworkBroadcastDataType();
                    Enum.TryParse<NetworkBroadcastDataType>(dataFragments[3], out type);
                    switch (type)
                    {
                        case NetworkBroadcastDataType.BDstring:
                            Networking.GetString(dataFragments[1], dataFragments[2]);
                            break;

                        case NetworkBroadcastDataType.BDint:
                            Networking.GetInt(int.Parse(dataFragments[1]), dataFragments[2]);
                            break;

                        case NetworkBroadcastDataType.BDfloat:
                            Networking.GetFloat(float.Parse(dataFragments[1]), dataFragments[2]);
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
                            Networking.GetVector3(convertedString, dataFragments[2]);
                            break;
                    }
                    if (SVAPI.netTester)
                    {
                        //Plugin.Log.LogWarning("Success! Received data with no errors.");
                    }
                    return false;
                }
                else
                {
                    Plugin.Log.LogError("Generic Network receive fail. This is a failure of the API, and it should be reported as a bug.");
                }
            }
            return true;
        }

        private static bool ChatCommands(HUDManager __instance, InputAction.CallbackContext context)
        {
            if (__instance.chatTextField.text.ToLower().Contains("/modcheck"))
            {
                CheatDatabase.OtherPlayerCheatDetector();
                return false;
            }
            return true;
        }
    }
}
