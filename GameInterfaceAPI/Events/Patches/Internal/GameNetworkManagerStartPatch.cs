using GameNetcodeStuff;
using HarmonyLib;
using LC_API.GameInterfaceAPI.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace LC_API.GameInterfaceAPI.Events.Patches.Internal
{
    [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Start) )]
    class GameNetworkManagerStartPatch
    {
        private const string PLAYER_NETWORKING_BUNDLE_LOCATION = "assets/lc_api/playernetworkingprefab.prefab";

        private static void Postfix(GameNetworkManager __instance)
        {
            GameObject obj = BundleAPI.BundleLoader.GetLoadedAsset<GameObject>(PLAYER_NETWORKING_BUNDLE_LOCATION);
            obj.AddComponent<Features.Player>();
            __instance.GetComponent<NetworkManager>().AddNetworkPrefab(obj);

            Features.Player.PlayerNetworkPrefab = obj;
        }
    }
}
