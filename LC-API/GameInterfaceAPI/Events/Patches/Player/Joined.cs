using GameNetcodeStuff;
using HarmonyLib;
using LC_API.GameInterfaceAPI.Events.EventArgs.Player;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace LC_API.GameInterfaceAPI.Events.Patches.Player
{
    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnPlayerConnectedClientRpc))]
    internal static class Joined
    {
        private static void Prefix(ulong clientId, int assignedPlayerObjectId)
        {
            if (Plugin.configVanillaSupport.Value) return;

            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
            {
                PlayerControllerB playerController = StartOfRound.Instance.allPlayerScripts[assignedPlayerObjectId];
                Features.Player.GetOrAdd(playerController).NetworkClientId.Value = clientId;
            }
        }

        private static void Postfix(ulong clientId, int assignedPlayerObjectId)
        {
            if (Plugin.configVanillaSupport.Value) return;

            PlayerControllerB playerController = StartOfRound.Instance.allPlayerScripts[assignedPlayerObjectId];
            if (!Cache.Player.ConnectedPlayers.Contains(clientId))
            {
                Cache.Player.ConnectedPlayers.Add(clientId);

                playerController.StartCoroutine(JoinedCoroutine(playerController));
            }
        }

        // Since we have to wait for players' client id to sync to the player instance, we have to constantly check
        // if the player and its controller were linked yet. Very annoying.
        internal static IEnumerator JoinedCoroutine(PlayerControllerB controller)
        {
            yield return new WaitUntil(() => StartOfRound.Instance.localPlayerController != null);

            Features.Player player = Features.Player.GetOrAdd(controller);

            while (player == null)
            {
                yield return new WaitForSeconds(0.1f);

                player = Features.Player.GetOrAdd(controller);
            }

            if (player.IsLocalPlayer)
            {
                Features.Player.LocalPlayer = player;
            }

            Handlers.Player.OnJoined(new JoinedEventArgs(player));
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.ConnectClientToPlayerObject))]
    internal static class Joined2
    {
        private static void Postfix(PlayerControllerB __instance)
        {
            if (Plugin.configVanillaSupport.Value) return;

            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
            {
                Features.Player.GetOrAdd(__instance).NetworkClientId.Value = __instance.actualClientId;
            }

            if (!Cache.Player.ConnectedPlayers.Contains(__instance.actualClientId))
            {
                Cache.Player.ConnectedPlayers.Add(__instance.actualClientId);

                __instance.StartCoroutine(Joined.JoinedCoroutine(__instance));
            }
        }
    }
}
