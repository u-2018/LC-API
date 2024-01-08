using GameNetcodeStuff;
using HarmonyLib;
using LC_API.GameInterfaceAPI.Events.EventArgs.Player;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace LC_API.GameInterfaceAPI.Events.Patches.Player
{
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DiscardHeldObject))]
    internal class DroppingItem
    {
        internal static DroppingItemEventArgs CallEvent(PlayerControllerB player, bool placeObject)
        {
            if (Plugin.configVanillaSupport.Value) return null;

            DroppingItemEventArgs ev = new DroppingItemEventArgs(Features.Player.GetOrAdd(player),
                Features.Item.Get(player.currentlyHeldObjectServer), placeObject);

            Handlers.Player.OnDroppingItem(ev);

            return ev;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = new List<CodeInstruction>(instructions);

            //const int offset = 3;

            //int index = newInstructions.FindLastIndex(i => i.OperandIs(AccessTools.Method(typeof(PlayerControllerB),
            //    nameof(PlayerControllerB.AllowPlayerDeath)))) + offset;

            //Label nullLabel = generator.DefineLabel();
            //Label notAllowedLabel = generator.DefineLabel();
            //Label skipLabel = generator.DefineLabel();

            //CodeInstruction[] inst = new CodeInstruction[]
            //{
            //    // DroppingItemEventArgs ev = DroppingItem.CallEvent(PlayerControllerB)
            //    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
            //    new CodeInstruction(OpCodes.Ldarg_1),
            //    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DroppingItem), nameof(DroppingItem.CallEvent))),

            //    // if (ev is null) -> base game code
            //    new CodeInstruction(OpCodes.Dup),
            //    new CodeInstruction(OpCodes.Brfalse_S, nullLabel),

            //    new CodeInstruction(OpCodes.Dup),

            //    // if (!ev.IsAllowed) return
            //    new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(DroppingItemEventArgs), nameof(DroppingItemEventArgs.IsAllowed))),
            //    new CodeInstruction(OpCodes.Brfalse_S, notAllowedLabel),

            //    new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(DroppingItemEventArgs), nameof(DroppingItemEventArgs.PlaceObject))),
            //    new CodeInstruction(OpCodes.Starg, 1),

            //    new CodeInstruction(OpCodes.Br, skipLabel),
            //    new CodeInstruction(OpCodes.Pop).WithLabels(nullLabel),
            //    new CodeInstruction(OpCodes.Br, skipLabel),
            //    new CodeInstruction(OpCodes.Pop).WithLabels(notAllowedLabel),
            //    new CodeInstruction(OpCodes.Ret)
            //};

            //newInstructions.InsertRange(index, inst);

            //newInstructions[index + inst.Length].labels.Add(skipLabel);

            for (int i = 0; i < newInstructions.Count; i++) yield return newInstructions[i];
        }
    }
}
