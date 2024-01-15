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
        internal static DroppingItemEventArgs CallEvent(PlayerControllerB playerController, bool placeObject, Vector3 targetPosition, 
            int floorYRotation, NetworkObject parentObjectTo, bool matchRotationOfParent, bool droppedInShip)
        {
            if (Plugin.configVanillaSupport.Value) return null;

            Features.Player player = Features.Player.GetOrAdd(playerController);

            Features.Item item = Features.Item.GetOrAdd(playerController.currentlyHeldObjectServer);

            DroppingItemEventArgs ev = new DroppingItemEventArgs(player, item, placeObject, targetPosition, floorYRotation, parentObjectTo, matchRotationOfParent, droppedInShip);

            Handlers.Player.OnDroppingItem(ev);

            player.CallDroppingItemOnOtherClients(item, placeObject, targetPosition, floorYRotation, parentObjectTo, matchRotationOfParent, droppedInShip);

            return ev;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = new List<CodeInstruction>(instructions);

            int animIndex = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldarg_1);

            CodeInstruction[] animatorStuff = newInstructions.GetRange(0, animIndex).ToArray();

            newInstructions.RemoveRange(0, animIndex);

            LocalBuilder isInShipLocal = generator.DeclareLocal(typeof(bool));

            {
                const int offset = 1;

                int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Stloc_0) + offset;

                Label nullLabel = generator.DefineLabel();
                Label notAllowedLabel = generator.DefineLabel();
                Label skipLabel = generator.DefineLabel();

                CodeInstruction[] inst = new CodeInstruction[]
                {
                    // DroppingItemEventArgs ev = DroppingItem.CallEvent(PlayerControllerB, bool, Vector3, 
                    //  int, NetworkObject, bool, bool)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldarg_3),
                    new CodeInstruction(OpCodes.Ldloc, 4),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Ldarg, 4),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PlayerControllerB), nameof(PlayerControllerB.isInHangarShipRoom))),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DroppingItem), nameof(DroppingItem.CallEvent))),

                    // if (ev is null) -> base game code
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Brfalse_S, nullLabel),

                    new CodeInstruction(OpCodes.Dup),

                    // if (!ev.IsAllowed) return
                    new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(DroppingItemEventArgs), nameof(DroppingItemEventArgs.IsAllowed))),
                    new CodeInstruction(OpCodes.Brfalse_S, notAllowedLabel),

                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Dup),

                    // placePosition = ev.TargetPosition
                    new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(DroppingItemEventArgs), nameof(DroppingItemEventArgs.TargetPosition))),
                    new CodeInstruction(OpCodes.Starg, 3),

                    // floorYRot2 = ev.FloorYRotation
                    new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(DroppingItemEventArgs), nameof(DroppingItemEventArgs.FloorYRotation))),
                    new CodeInstruction(OpCodes.Stloc, 4),

                    // parentObjectTo = ev.ParentObjectTo
                    new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(DroppingItemEventArgs), nameof(DroppingItemEventArgs.ParentObjectTo))),
                    new CodeInstruction(OpCodes.Starg, 2),

                    // matchRotationOfParent = ev.MatchRotationOfParent
                    new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(DroppingItemEventArgs), nameof(DroppingItemEventArgs.MatchRotationOfParent))),
                    new CodeInstruction(OpCodes.Starg, 4),

                    // droppedInShip = ev.DroppedInShip
                    new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(DroppingItemEventArgs), nameof(DroppingItemEventArgs.DroppedInShip))),
                    new CodeInstruction(OpCodes.Stloc, isInShipLocal.LocalIndex),
                };

                inst = inst.AddRangeToArray(animatorStuff);

                inst = inst.AddRangeToArray(new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Br, skipLabel),
                    new CodeInstruction(OpCodes.Pop).WithLabels(nullLabel),
                    new CodeInstruction(OpCodes.Br, skipLabel),
                    new CodeInstruction(OpCodes.Pop).WithLabels(notAllowedLabel),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(PlayerControllerB), nameof(PlayerControllerB.throwingObject))),
                    new CodeInstruction(OpCodes.Ret)
                });

                newInstructions.InsertRange(index, inst);

                newInstructions[index + inst.Length].labels.Add(skipLabel);

                newInstructions.RemoveRange(index + inst.Length + 1, 4);

                newInstructions.InsertRange(index + inst.Length + 1, new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Ldloc, isInShipLocal.LocalIndex),
                    new CodeInstruction(OpCodes.Ldloc, isInShipLocal.LocalIndex)
                });
            }

            {
                const int offset = 1;

                int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Stloc_S) + offset;

                Label nullLabel = generator.DefineLabel();
                Label notAllowedLabel = generator.DefineLabel();
                Label skipLabel = generator.DefineLabel();

                CodeInstruction[] inst = new CodeInstruction[]
                {
                    // DroppingItemEventArgs ev = DroppingItem.CallEvent(PlayerControllerB, bool, Vector3, 
                    //  int, NetworkObject, bool, bool)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Ldarg, 4),
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DroppingItem), nameof(DroppingItem.CallEvent))),

                    // if (ev is null) -> base game code
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Brfalse_S, nullLabel),

                    new CodeInstruction(OpCodes.Dup),

                    // if (!ev.IsAllowed) return
                    new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(DroppingItemEventArgs), nameof(DroppingItemEventArgs.IsAllowed))),
                    new CodeInstruction(OpCodes.Brfalse_S, notAllowedLabel),

                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(OpCodes.Dup),

                    // targetFloorPosition = ev.TargetPosition
                    new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(DroppingItemEventArgs), nameof(DroppingItemEventArgs.TargetPosition))),
                    new CodeInstruction(OpCodes.Stloc_1),

                    // floorYRot = ev.FloorYRotation
                    new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(DroppingItemEventArgs), nameof(DroppingItemEventArgs.FloorYRotation))),
                    new CodeInstruction(OpCodes.Stloc_0),

                    // parentObjectTo = ev.ParentObjectTo
                    new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(DroppingItemEventArgs), nameof(DroppingItemEventArgs.ParentObjectTo))),
                    new CodeInstruction(OpCodes.Starg, 2),

                    // matchRotationOfParent = ev.MatchRotationOfParent
                    new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(DroppingItemEventArgs), nameof(DroppingItemEventArgs.MatchRotationOfParent))),
                    new CodeInstruction(OpCodes.Starg, 4),

                    // droppedInShip = ev.DroppedInShip
                    new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(DroppingItemEventArgs), nameof(DroppingItemEventArgs.DroppedInShip))),
                    new CodeInstruction(OpCodes.Stloc, isInShipLocal.LocalIndex),
                };

                inst = inst.AddRangeToArray(animatorStuff);

                inst = inst.AddRangeToArray(new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Br, skipLabel),
                    new CodeInstruction(OpCodes.Pop).WithLabels(nullLabel),
                    new CodeInstruction(OpCodes.Br, skipLabel),
                    new CodeInstruction(OpCodes.Pop).WithLabels(notAllowedLabel),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(PlayerControllerB), nameof(PlayerControllerB.throwingObject))),
                    new CodeInstruction(OpCodes.Ret)
                });

                newInstructions.InsertRange(index, inst);

                newInstructions[index + inst.Length].labels.Add(skipLabel);

                newInstructions.RemoveRange(index + inst.Length + 1, 3);

                newInstructions.InsertRange(index + inst.Length + 1, new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Ldloc, isInShipLocal.LocalIndex),
                    new CodeInstruction(OpCodes.Ldloc, isInShipLocal.LocalIndex)
                });
            }

            for (int i = 0; i < newInstructions.Count; i++) yield return newInstructions[i];
        }
    }
}
