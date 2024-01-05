using BepInEx;
using HarmonyLib;
using LC_API.BundleAPI;
using LC_API.Networking;
using System;
using System.IO;
using Unity.Netcode;
using UnityEngine;

namespace LC_API.GameInterfaceAPI.Events.Patches.Internal
{
    [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Start))]
    class GameNetworkManagerStartPatch
    {
        private static readonly string BUNDLE_PATH = Path.Combine(Plugin.Instance.Info.Location.Substring(0, Plugin.Instance.Info.Location.LastIndexOf(Path.DirectorySeparatorChar)), "Bundles", "networking");

        private const string PLAYER_NETWORKING_ASSET_LOCATION = "assets/lc_api/playernetworkingprefab.prefab";

        private static void Postfix(GameNetworkManager __instance)
        {
            if (Plugin.configVanillaSupport.Value) return;

            if (!File.Exists(BUNDLE_PATH))
            {
                throw new Exception("Networking bundle not found at expected path.");
            }

            NetworkManager networkManager = __instance.GetComponent<NetworkManager>();

            LoadedAssetBundle assets = BundleLoader.LoadAssetBundle(BUNDLE_PATH, false);

            GameObject playerObj = assets.GetAsset<GameObject>(PLAYER_NETWORKING_ASSET_LOCATION);
            playerObj.AddComponent<Features.Player>();
            playerObj.AddComponent<Features.Player.PlayerInventory>();
            networkManager.AddNetworkPrefab(playerObj);
            Features.Player.PlayerNetworkPrefab = playerObj;

            foreach (NetworkPrefab prefab in networkManager.NetworkConfig.Prefabs.Prefabs)
            {
                if (prefab.Prefab.GetComponent<GrabbableObject>() != null)
                {
                    prefab.Prefab.AddComponent<Features.Item>();
                }
            }
        }
    }
}
