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
        private static void Postfix(GameNetworkManager __instance)
        {
            GameObject obj = BundleAPI.BundleLoader.GetLoadedAsset<GameObject>("assets/lc_api/playernetworkingprefab.prefab");
            obj.AddComponent<Features.Player>();
            __instance.GetComponent<NetworkManager>().AddNetworkPrefab(obj);

            Features.Player.PlayerNetworkPrefab = obj;
        }
    }
}
