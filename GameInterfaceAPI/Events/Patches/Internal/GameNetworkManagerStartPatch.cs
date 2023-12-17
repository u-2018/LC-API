using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using LC_API.BundleAPI;
using LC_API.GameInterfaceAPI.Features;
using System;
using System.Collections.Generic;
using System.IO;
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
        private const string NETWORKING_BUNDLE_LOCATION = "assets/lc_api/networkingprefab.prefab";

        private static readonly string BUNDLE_PATH = Path.Combine(Paths.PluginPath, "2018-LC_API", "Bundles", "networking");

        private static void Postfix(GameNetworkManager __instance)
        {
            LoadedAssetBundle assets = BundleLoader.LoadAssetBundle(BUNDLE_PATH);

            GameObject obj = assets.GetAsset<GameObject>(NETWORKING_BUNDLE_LOCATION);
            obj.AddComponent<Features.Player>();
            __instance.GetComponent<NetworkManager>().AddNetworkPrefab(obj);

            Features.Player.PlayerNetworkPrefab = obj;
        }
    }
}
