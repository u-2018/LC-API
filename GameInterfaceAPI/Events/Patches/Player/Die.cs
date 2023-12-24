﻿using GameNetcodeStuff;
using HarmonyLib;
using LC_API.GameInterfaceAPI.Events.EventArgs.Player;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace LC_API.GameInterfaceAPI.Events.Patches.Player
{
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.KillPlayer))]
    internal class Die
    {
        private static DyingEventArgs CallDyingEvent(PlayerControllerB playerController, Vector3 force, bool spawnBody,
            CauseOfDeath causeOfDeath, int deathAnimation)
        {
            DyingEventArgs ev = new DyingEventArgs(Features.Player.GetOrAdd(playerController), force, spawnBody,
                causeOfDeath, deathAnimation);

            Handlers.Player.OnDying(ev);

            return ev;
        }

        private static void CallDiedEvent(PlayerControllerB playerController, Vector3 force, bool spawnBody,
            CauseOfDeath causeOfDeath, int deathAnimation)
        {
            DiedEventArgs ev = new DiedEventArgs(Features.Player.GetOrAdd(playerController), force, spawnBody,
                causeOfDeath, deathAnimation);

            Handlers.Player.OnDied(ev);
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = new List<CodeInstruction>(instructions);
            const int offset = 3;

            int index = newInstructions.FindLastIndex(i => i.OperandIs(AccessTools.Method(typeof(PlayerControllerB),
                nameof(PlayerControllerB.AllowPlayerDeath)))) + offset;

            Label notAllowedLabel = generator.DefineLabel();
            Label skipLabel = generator.DefineLabel();

            CodeInstruction[] inst = new CodeInstruction[]
            {
                // DyingEventArgs ev = Die.CallDyingEvent(PlayerControllerB, Vector3, bool, CauseOfDeath, int)
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldarg_3),
                new CodeInstruction(OpCodes.Ldarg, 4),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Die), nameof(Die.CallDyingEvent))),
                new CodeInstruction(OpCodes.Dup),
                // if (!ev.IsAllwed) return
                new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(DyingEventArgs), nameof(DyingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, notAllowedLabel),

                // Duplicating the stack is more memory efficient than making a local
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),

                // bodyVelocity = ev.Force
                new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(DyingEventArgs), nameof(DyingEventArgs.Force))),
                new CodeInstruction(OpCodes.Starg_S, 1),

                // spawnBody = ev.SpawnBody
                new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(DyingEventArgs), nameof(DyingEventArgs.SpawnBody))),
                new CodeInstruction(OpCodes.Starg_S, 2),

                // causeOfDeath = ev.CauseOfDeath
                new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(DyingEventArgs), nameof(DyingEventArgs.CauseOfDeath))),
                new CodeInstruction(OpCodes.Starg_S, 3),

                // deathAnimation = ev.DeathAnimation
                new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(DyingEventArgs), nameof(DyingEventArgs.DeathAnimation))),
                new CodeInstruction(OpCodes.Starg_S, 4),

                new CodeInstruction(OpCodes.Br, skipLabel),
                new CodeInstruction(OpCodes.Pop).WithLabels(notAllowedLabel),
                new CodeInstruction(OpCodes.Ret)
            };

            newInstructions.InsertRange(index, inst);

            newInstructions[index + inst.Length].labels.Add(skipLabel);

            newInstructions.InsertRange(newInstructions.Count - 1, new CodeInstruction[]
            {
                // Die.CallDiedEvent(PlayerControllerB, Vector3, bool, CauseOfDeath, int)
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldarg_3),
                new CodeInstruction(OpCodes.Ldarg, 4),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Die), nameof(Die.CallDiedEvent))),
            });

            for (int i = 0; i < newInstructions.Count; i++) yield return newInstructions[i];
        }
    }
}
