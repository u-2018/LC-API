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
    [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Start))]
    class GameNetworkManagerStartPatch
    {
        private static readonly string BUNDLE_PATH = Path.Combine(Paths.PluginPath, "2018-LC_API", "Bundles", "networking");

        private const string PLAYER_NETWORKING_ASSET_LOCATION = "assets/lc_api/playernetworkingprefab.prefab";

        private static void Postfix(GameNetworkManager __instance)
        {
            LoadedAssetBundle assets = BundleLoader.LoadAssetBundle(BUNDLE_PATH);

            GameObject obj = assets.GetAsset<GameObject>(PLAYER_NETWORKING_ASSET_LOCATION);
            obj.AddComponent<Features.Player>();
            __instance.GetComponent<NetworkManager>().AddNetworkPrefab(obj);

            Features.Player.PlayerNetworkPrefab = obj;
        }
    }
}
