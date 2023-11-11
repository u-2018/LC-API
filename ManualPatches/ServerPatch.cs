// -----------------------------------------------------------------------
// <copyright file="ServerPatch.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LC_API.ManualPatches;

using System;

// ReSharper disable  RedundantNameQualifier
// ReSharper disable UnusedMember.Local
using LC_API.Comp;
using LC_API.Data;
using LC_API.ServerAPI;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

#pragma warning disable SA1313

/// <summary>
/// Contains info for the server patch.
/// </summary>
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedParameter.Local
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
internal sealed class ServerPatch
{
    private static bool OnLobbyCreate(GameNetworkManager __instance, Result result, Lobby lobby)
    {
        if (result != Result.OK)
        {
            Debug.LogError($"Lobby could not be created! {result}", __instance);
        }

        __instance.lobbyHostSettings.lobbyName = "[MODDED]" + __instance.lobbyHostSettings.lobbyName;

        // lobby.SetData("vers", "This server requires mods. " + GameNetworkManager.Instance.gameVersionNum.ToString());
        Plugin.Log.LogMessage("server pre-setup success");
        return true;
    }

    private static bool Vers(MenuManager __instance)
    {
        SVAPI.MenuManager = __instance;
        return true;
    }

    private static bool ChatInterpreter(HUDManager __instance, string chatMessage, string nameOfUserWhoTyped)
    {
        if (chatMessage.Contains("NWE") & chatMessage.Contains("<size=0>"))
        {
            string[] dataFragments = chatMessage.Split('/');

            // Make sure there are at least two '/' characters
            if (dataFragments.Length >= 3)
            {
                Enum.TryParse(dataFragments[2], out NetworkBroadcastDataType type);

                switch (type)
                {
                    case NetworkBroadcastDataType.BDstring:
                        Networking.GetString(dataFragments[0], dataFragments[1]);
                        break;

                    case NetworkBroadcastDataType.BDint:
                        Networking.GetInt(int.Parse(dataFragments[0]), dataFragments[1]);
                        break;

                    case NetworkBroadcastDataType.BDfloat:
                        Networking.GetFloat(float.Parse(dataFragments[0]), dataFragments[1]);
                        break;

                    case NetworkBroadcastDataType.BDvector3:
                        string[] components = dataFragments[0].Replace("(", string.Empty).Replace(")", string.Empty).Split(',');
                        Vector3 convertedString = default(Vector3);
                        if (components.Length == 3)
                        {
                            if (float.TryParse(components[0], out float x) && float.TryParse(components[1], out float y) && float.TryParse(components[2], out float z))
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

                        Networking.GetVector3(convertedString, dataFragments[1]);
                        break;
                }
            }
            else
            {
                Plugin.Log.LogError("Generic Network receive fail. This is a failure of the API, and it should be reported as a bug.");
            }
        }

        return true;
    }
}