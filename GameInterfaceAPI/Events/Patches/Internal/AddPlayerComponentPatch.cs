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
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.Awake) )]
    class AddPlayerComponentPatch
    {
        private static void Prefix(PlayerControllerB __instance)
        {
            if (!__instance.gameObject.TryGetComponent(out Features.Player _))
                __instance.gameObject.AddComponent<Features.Player>();
        }
    }
}
