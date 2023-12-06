using BepInEx.Configuration;
using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace LC_API.ClientAPI
{
    /// <summary>
    /// Provides an easy way for developers to add chat-based commands.
    /// </summary>
    public static class CommandHandler
    {
        internal static ConfigEntry<string> commandPrefix;

        internal static Dictionary<string, Action<string[]>> CommandHandlers = new Dictionary<string, Action<string[]>>();

        internal static Dictionary<string, List<string>> CommandAliases = new Dictionary<string, List<string>>();

        /// <summary>
        /// Registers a command with no aliases.
        /// </summary>
        /// <param name="command">The command string. No spaces.</param>
        /// <param name="handler">The handler itself. Passes a string[] of arguments.</param>
        /// <returns>Whether or not the command handler was added.</returns>
        public static bool RegisterCommand(string command, Action<string[]> handler)
        {
            // The handler is not capable of handling commands with spaces.
            if (command.Contains(" ") || CommandHandlers.ContainsKey(command)) return false;

            CommandHandlers.Add(command, handler);

            return true;
        }

        /// <summary>
        /// Registers a command with aliases.
        /// </summary>
        /// <param name="command">The command string. No spaces.</param>
        /// <param name="aliases">A list of aliases. None of them can have spaces, and a handler cannot exist with that string.</param>
        /// <param name="handler">The handler itself. Passes a string[] of arguments.</param>
        /// <returns></returns>
        public static bool RegisterCommand(string command, List<string> aliases, Action<string[]> handler)
        {
            // The handler is not capable of handling commands with spaces.
            if (command.Contains(" ") || GetCommandHandler(command) != null) return false;

            foreach (string alias in aliases)
            {
                if (alias.Contains(" ") || GetCommandHandler(alias) != null) return false;
            }

            CommandHandlers.Add(command, handler);

            CommandAliases.Add(command, aliases);

            return true;
        }

        /// <summary>
        /// Unregisters a command.
        /// </summary>
        /// <param name="command">The command string to unregister.</param>
        /// <returns>true if the command existed and was unregistered, false otherwise.</returns>
        public static bool UnregisterCommand(string command)
        {
            CommandAliases.Remove(command);
            return CommandHandlers.Remove(command);
        }

        internal static Action<string[]> GetCommandHandler(string command)
        {
            if (CommandHandlers.TryGetValue(command, out var handler)) return handler;

            foreach (var alias in CommandAliases)
            {
                if (alias.Value.Contains(command)) return CommandHandlers[alias.Key];
            }

            return null;
        }

        internal static bool TryGetCommandHandler(string command, out Action<string[]> handler)
        {
            handler = GetCommandHandler(command);
            return handler != null;
        }

        internal static class SubmitChatPatch
        {
            private static bool HandleMessage(HUDManager manager)
            {
                string message = manager.chatTextField.text;

                if (!message.IsNullOrWhiteSpace() && message.StartsWith(commandPrefix.Value))
                {
                    string[] split = message.Split(' ');

                    string command = split[0].Substring(commandPrefix.Value.Length);

                    if (TryGetCommandHandler(command, out var handler))
                    {
                        string[] arguments = split.Skip(1).ToArray();
                        try
                        {
                            handler.Invoke(arguments);
                        }
                        catch (Exception ex)
                        {
                            Plugin.Log.LogError($"Error handling command: {command}");
                            Plugin.Log.LogError(ex);
                        }
                    }

                    manager.localPlayer.isTypingChat = false;
                    manager.chatTextField.text = "";
                    EventSystem.current.SetSelectedGameObject(null);
                    manager.typingIndicator.enabled = false;

                    return true;
                }

                return false;
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                List<CodeInstruction> newInstructions = new List<CodeInstruction>(instructions);

                Label returnLabel = generator.DefineLabel();

                newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

                int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldfld &&
                    (FieldInfo)i.operand == AccessTools.Field(typeof(PlayerControllerB), nameof(PlayerControllerB.isPlayerDead))) - 2;

                newInstructions.InsertRange(index, new CodeInstruction[]
                {
                    // if (SubmitChatPatch.HandleMessage(this)) return;
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SubmitChatPatch), nameof(SubmitChatPatch.HandleMessage))),
                    new CodeInstruction(OpCodes.Brtrue, returnLabel)
                });

                for (int z = 0; z < newInstructions.Count; z++) yield return newInstructions[z];
            }
        }
    }
}
