using GameNetcodeStuff;
using HarmonyLib;
using LC_API.GameInterfaceAPI.Events.EventArgs.Player;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace LC_API.GameInterfaceAPI.Events.Patches.Player
{
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.ConnectClientToPlayerObject))]
    internal static class Joined
    {
        private static void Prefix(PlayerControllerB __instance)
        {
            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
            {
                Features.Player.GetOrAdd(__instance).NetworkClientId.Value = __instance.actualClientId;
            }
        }

        private static void Postfix(PlayerControllerB __instance)
        {
            if (!Cache.Player.ConnectedPlayers.Contains(__instance.playerSteamId))
            {
                Cache.Player.ConnectedPlayers.Add(__instance.playerSteamId);

                __instance.StartCoroutine(JoinedCoroutine(__instance));
            }
        }

        // Since we have to wait for players' client id to sync to the player instance, we have to constantly check
        // if the player and its controller were linked yet. Very annoying.
        private static IEnumerator JoinedCoroutine(PlayerControllerB controller)
        {
            yield return new WaitUntil(() => StartOfRound.Instance.localPlayerController != null);

            Features.Player player = Features.Player.GetOrAdd(controller);

            while (player == null)
            {
                yield return new WaitForSeconds(0.1f);

                player = Features.Player.GetOrAdd(controller);
            }

            if (player.IsLocalPlayer) Features.Player.LocalPlayer = player;

            Handlers.Player.OnJoined(new JoinedEventArgs(player));
        }
    }
}
