using GameNetcodeStuff;
using HarmonyLib;
using LC_API.GameInterfaceAPI.Events.EventArgs.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC_API.GameInterfaceAPI.Events.Patches.Player
{
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.SendNewPlayerValuesClientRpc))]
    internal static class Joined
    {
        private static void Postfix(PlayerControllerB __instance)
        {
            if (!Cache.Player.ConnectedPlayers.Contains(__instance.playerSteamId))
            {
                Cache.Player.ConnectedPlayers.Add(__instance.playerSteamId);
                Handlers.Player.OnJoined(new PlayerJoinedEventArgs(__instance));
            }
        }
    }
}
