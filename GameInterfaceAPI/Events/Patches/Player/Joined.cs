using GameNetcodeStuff;
using HarmonyLib;
using LC_API.GameInterfaceAPI.Events.EventArgs.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;

namespace LC_API.GameInterfaceAPI.Events.Patches.Player
{
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.SendNewPlayerValuesClientRpc))]
    internal static class Joined
    {
        private static void Prefix(PlayerControllerB __instance)
        {
            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
                Features.Player.GetOrAdd(__instance).NetworkClientId.Value = __instance.actualClientId;
        }

        private static void Postfix(PlayerControllerB __instance)
        {
            if (!Cache.Player.ConnectedPlayers.Contains(__instance.playerSteamId))
            {
                Cache.Player.ConnectedPlayers.Add(__instance.playerSteamId);

                Handlers.Player.OnJoined(new JoinedEventArgs(Features.Player.GetOrAdd(__instance)));
            }
        }
    }
}
