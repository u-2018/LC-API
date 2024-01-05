using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using LC_API.Data;
using LC_API.Extensions;
using LC_API.GameInterfaceAPI.Events;
using LC_API.GameInterfaceAPI.Events.EventArgs.Player;
using LC_API.GameInterfaceAPI.Events.Patches.Player;
using LC_API.GameInterfaceAPI.Features;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using static LC_API.GameInterfaceAPI.Events.Events;

namespace LC_API.Networking
{
    /// <summary>
    /// Provies an easy to use interface for UnityNetcode custom network messages.
    /// </summary>
    public static class Network
    {
        internal const string MESSAGE_RELAY_UNIQUE_NAME = "LC_API_RELAY_MESSAGE";
        internal static Dictionary<string, NetworkMessageFinalizerBase> NetworkMessageFinalizers { get; } = new Dictionary<string, NetworkMessageFinalizerBase>();

        internal static byte[] ToBytes(this object @object)
        {
            if (@object == null) return null;

            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@object));
        }

        internal static T ToObject<T>(this byte[] bytes) where T : class
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
        }

        internal static bool StartedNetworking { get; set; } = false;

        /// <summary>
        /// Provides an event in which registering network messages should happen.
        /// </summary>
        public static event CustomEventHandler RegisterNetworkMessages;

        internal static event CustomEventHandler UnregisterNetworkMessages;

        internal static void OnRegisterNetworkMessages() => RegisterNetworkMessages.InvokeSafely();

        internal static void OnUnregisterNetworkMessages() => UnregisterNetworkMessages.InvokeSafely();

        private static MethodInfo _registerInfo = null;
        private static MethodInfo _registerInfoGeneric = null;

        internal static MethodInfo RegisterInfo
        {
            get
            {
                if (_registerInfo == null)
                {
                    foreach (MethodInfo methodInfo in typeof(Network).GetMethods())
                    {
                        if (methodInfo.Name == nameof(RegisterMessage) && !methodInfo.IsGenericMethod)
                        {
                            _registerInfo = methodInfo;
                            break;
                        }
                    }
                }

                return _registerInfo;
            }
        }

        internal static MethodInfo RegisterInfoGeneric
        {
            get
            {
                if (_registerInfo == null)
                {
                    foreach (MethodInfo methodInfo in typeof(Network).GetMethods())
                    {
                        if (methodInfo.Name == nameof(RegisterMessage) && methodInfo.IsGenericMethod)
                        {
                            _registerInfoGeneric = methodInfo;
                            break;
                        }
                    }
                }

                return _registerInfoGeneric;
            }
        }

        internal static void RegisterAllMessages()
        {
            foreach (NetworkMessageFinalizerBase handler in NetworkMessageFinalizers.Values)
            {
                NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(handler.UniqueName, handler.Read);
            }
        }

        internal static void UnregisterAllMessages()
        {

            foreach (string name in NetworkMessageFinalizers.Keys)
            {
                UnregisterMessage(name, false);
            }
        }

        /// <summary>
        /// Registers all network messages contained in your assembly.
        /// </summary>
        public static void RegisterAll()
        {
            // This cursed line of code comes from Harmony's PatchAll method. Thanks, Harmony
            var m = new StackTrace().GetFrame(1).GetMethod();
            var assembly = m.ReflectedType.Assembly;
            foreach (Type type in AccessTools.GetTypesFromAssembly(assembly))
            {
                RegisterAll(type);
            }
        }

        /// <summary>
        /// Registers all network messages contained in the provided <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to register network messages from.</param>
        public static void RegisterAll(Type type)
        {
            if (type.IsClass)
            {
                NetworkMessage networkMessage = type.GetCustomAttribute<NetworkMessage>();

                if (networkMessage != null)
                {
                    if (type.BaseType.Name == "NetworkMessageHandler`1")
                    {
                        Type messageType = type.BaseType.GetGenericArguments()[0];
                        RegisterInfoGeneric
                            .MakeGenericMethod(messageType)
                            .Invoke(null, new object[]
                            {
                                        networkMessage.UniqueName,
                                        networkMessage.RelayToSelf,
                                        type.GetMethod("Handler").CreateDelegate(typeof(Action<,>)
                                            .MakeGenericType(typeof(ulong), messageType), Activator.CreateInstance(type))
                            });
                    }
                    else if (type.BaseType.Name == "NetworkMessageHandler")
                    {
                        RegisterInfo
                            .Invoke(null, new object[]
                            {
                                        networkMessage.UniqueName,
                                        networkMessage.RelayToSelf,
                                        type.GetMethod("Handler").CreateDelegate(typeof(Action<>)
                                            .MakeGenericType(typeof(ulong)), Activator.CreateInstance(type))
                            });
                    }
                }
                else
                {
                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic))
                    {
                        networkMessage = method.GetCustomAttribute<NetworkMessage>();
                        if (networkMessage != null)
                        {
                            if (!method.IsStatic) throw new Exception("Detected NetworkMessage attribute on non-static method. All NetworkMessages on methods must be static.");

                            if (method.GetParameters().Length == 1)
                            {
                                RegisterInfo
                                    .Invoke(null, new object[]
                                    {
                                                networkMessage.UniqueName,
                                                networkMessage.RelayToSelf,
                                                method.CreateDelegate(typeof(Action<>)
                                                    .MakeGenericType(typeof(ulong)))
                                    });
                            }
                            else
                            {
                                Type messageType = method.GetParameters()[1].ParameterType;
                                RegisterInfoGeneric
                                    .MakeGenericMethod(messageType)
                                    .Invoke(null, new object[]
                                    {
                                                networkMessage.UniqueName,
                                                networkMessage.RelayToSelf,
                                                method.CreateDelegate(typeof(Action<,>)
                                                    .MakeGenericType(typeof(ulong), messageType))
                                    });
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Unregisters all network messages contained in your assembly.
        /// </summary>
        /// <param name="andRemoveHandler">Wheter or not to prevent the handler from being re-registered when a new game is joined.</param>
        public static void UnregisterAll(bool andRemoveHandler = true)
        {
            // This cursed line of code comes from Harmony's PatchAll method. Thanks, Harmony
            var m = new StackTrace().GetFrame(1).GetMethod();
            var assembly = m.ReflectedType.Assembly;
            foreach (Type type in AccessTools.GetTypesFromAssembly(assembly))
            {
                UnregisterAll(type, andRemoveHandler);
            }
        }

        /// <summary>
        /// Unregisters all network messages contained in the provided <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to unregister all network messages of.</param>
        /// <param name="andRemoveHandler">Wheter or not to prevent the handler from being re-registered when a new game is joined.</param>
        public static void UnregisterAll(Type type, bool andRemoveHandler = true)
        {
            if (type.IsClass)
            {
                NetworkMessage networkMessage = type.GetCustomAttribute<NetworkMessage>();

                if (networkMessage != null)
                {
                    UnregisterMessage(networkMessage.UniqueName, andRemoveHandler);
                }
                else
                {
                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic))
                    {
                        networkMessage = method.GetCustomAttribute<NetworkMessage>();
                        if (networkMessage != null)
                        {
                            UnregisterMessage(networkMessage.UniqueName, andRemoveHandler);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Registers a network message with a name and handler.
        /// </summary>
        /// <typeparam name="T">The type of the network message.</typeparam>
        /// <param name="uniqueName">The name of the network message.</param>
        /// <param name="relayToSelf">Whether or not this message should be relayed to the sender.</param>
        /// <param name="onReceived">The handler to use for the message.</param>
        /// <exception cref="Exception">Thrown when T is not serializable, or if the name is already taken.</exception>
        public static void RegisterMessage<T>(string uniqueName, bool relayToSelf, Action<ulong, T> onReceived) where T : class
        {
            if (NetworkMessageFinalizers.ContainsKey(uniqueName))
                throw new Exception($"{uniqueName} already registered");

            NetworkMessageFinalizer<T> networkMessageHandler = new NetworkMessageFinalizer<T>(uniqueName, relayToSelf, onReceived);

            NetworkMessageFinalizers.Add(uniqueName, networkMessageHandler);

            if (StartedNetworking)
                NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(uniqueName, networkMessageHandler.Read);
        }

        /// <summary>
        /// Registers a network message with a name and handler.
        /// </summary>
        /// <param name="uniqueName">The name of the network message.</param>
        /// <param name="relayToSelf">Whether or not this message should be relayed to the sender.</param>
        /// <param name="onReceived">The handler to use for the message.</param>
        /// <exception cref="Exception">Thrown when the name is already taken.</exception>
        public static void RegisterMessage(string uniqueName, bool relayToSelf, Action<ulong> onReceived)
        {
            if (NetworkMessageFinalizers.ContainsKey(uniqueName))
                throw new Exception($"{uniqueName} already registered");

            NetworkMessageFinalizer networkMessageHandler = new NetworkMessageFinalizer(uniqueName, relayToSelf, onReceived);

            NetworkMessageFinalizers.Add(uniqueName, networkMessageHandler);

            if (StartedNetworking)
                NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(uniqueName, networkMessageHandler.Read);
        }

        /// <summary>
        /// Unregisters a network message.
        /// </summary>
        /// <param name="uniqueName">The name of the message to unregister.</param>
        /// <param name="andRemoveHandler">Wheter or not to prevent the handler from being re-registered when a new game is joined.</param>
        public static void UnregisterMessage(string uniqueName, bool andRemoveHandler = true)
        {
            if ((!andRemoveHandler && NetworkMessageFinalizers.ContainsKey(uniqueName)) 
                || (andRemoveHandler && NetworkMessageFinalizers.Remove(uniqueName))) 
            {
                NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler(uniqueName);
            }
        }

        /// <summary>
        /// Sends a network message.
        /// </summary>
        /// <typeparam name="T">The type of the network message.</typeparam>
        /// <param name="uniqueName">The name of the network message.</param>
        /// <param name="object">The network message to send.</param>
        /// <exception cref="Exception">Thrown when the registered message with the name is not of the same type as the network message.</exception>
        public static void Broadcast<T>(string uniqueName, T @object) where T : class
        {
            if (NetworkMessageFinalizers.TryGetValue(uniqueName, out NetworkMessageFinalizerBase handler))
            {
                if (handler is NetworkMessageFinalizer<T> genericHandler)
                {
                    genericHandler.Send(@object);
                } 
                else
                {
                    throw new Exception($"Network handler for {uniqueName} was not broadcast with the right type!");
                }
            }
        }

        /// <summary>
        /// Sends a network message that has no body.
        /// </summary>
        /// <param name="uniqueName">The name of the network message.</param>
        /// <exception cref="Exception">Thrown when the registered message with the name is not of the same type as the network message.</exception>
        public static void Broadcast(string uniqueName)
        {
            if (NetworkMessageFinalizers.TryGetValue(uniqueName, out NetworkMessageFinalizerBase handler))
            {
                if (handler is NetworkMessageFinalizer finalizer)
                {
                    finalizer.Send();
                }
                else
                {
                    throw new Exception($"Network handler for {uniqueName} was not broadcast with the right type!");
                }
            }
        }

        internal static void Init()
        {
            RegisterNetworkMessages += () =>
            {
                StartedNetworking = true;

                if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
                {
                    NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(MESSAGE_RELAY_UNIQUE_NAME,
                        (ulong senderClientId, FastBufferReader reader) =>
                    {
                        reader.ReadValueSafe(out byte[] data);

                        NetworkMessageWrapper wrapped = data.ToObject<NetworkMessageWrapper>();

                        wrapped.Sender = senderClientId;

                        byte[] serialized = wrapped.ToBytes();

                        using (FastBufferWriter writer = new FastBufferWriter(FastBufferWriter.GetWriteSize(serialized), Unity.Collections.Allocator.Temp))
                        {
                            writer.WriteValueSafe(serialized);

                            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll(wrapped.UniqueName, writer, NetworkDelivery.ReliableFragmentedSequenced);
                        }
                    });
                }

                RegisterAllMessages();
            };

            UnregisterNetworkMessages += () =>
            {
                StartedNetworking = false;

                if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
                    NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler(MESSAGE_RELAY_UNIQUE_NAME);

                UnregisterAllMessages();
            };

            SetupNetworking();

            RegisterAll();

            ServerAPI.Networking.InitializeLegacyNetworking();
        }

        internal static void SetupNetworking()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Allows a method/class to act as a network message.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NetworkMessage : Attribute
    {
        /// <summary>
        /// The name of the message.
        /// </summary>
        public string UniqueName { get; }

        /// <summary>
        /// Whether or not to relay this message back to the sender.
        /// </summary>
        public bool RelayToSelf { get; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public NetworkMessage(string uniqueName, bool relayToSelf = false)
        {
            UniqueName = uniqueName;
            RelayToSelf = relayToSelf;
        }
#pragma warning restore
    }

    /// <summary>
    /// For use when decorating a class with the <see cref="NetworkMessage"/> attribute.
    /// </summary>
    /// <typeparam name="T">The type of the message. Must be Serializable.</typeparam>
    public abstract class NetworkMessageHandler<T> where T : class
    {
        /// <summary>
        /// The message handler.
        /// </summary>
        /// <param name="sender">The sender's client id.</param>
        /// <param name="message">The network message.</param>
        public abstract void Handler(ulong sender, T message);
    }

    /// <summary>
    /// For use when decorating a class with the <see cref="NetworkMessage"/> attribute.
    /// </summary>
    public abstract class NetworkMessageHandler
    {
        /// <summary>
        /// The message handler.
        /// </summary>
        /// <param name="sender">The sender's client id.</param>
        public abstract void Handler(ulong sender);
    }

    internal abstract class NetworkMessageFinalizerBase
    {
        internal abstract string UniqueName { get; }

        internal abstract bool RelayToSelf { get; }

        public abstract void Read(ulong sender, FastBufferReader reader);
    }

    internal class NetworkMessageFinalizer : NetworkMessageFinalizerBase
    {
        internal override string UniqueName { get; }

        internal override bool RelayToSelf { get; }

        internal Action<ulong> OnReceived { get; }

        public NetworkMessageFinalizer(string uniqueName, bool relayToSelf, Action<ulong> onReceived)
        {
            UniqueName = uniqueName;
            RelayToSelf = relayToSelf;
            OnReceived = onReceived;
        }

        public void Send()
        {
            if (Player.LocalPlayer == null || Player.HostPlayer == null)
            {
                NetworkManager.Singleton.StartCoroutine(SendLater());
                return;
            }

            NetworkMessageWrapper wrapped = new NetworkMessageWrapper(UniqueName, Player.LocalPlayer.ClientId);
            byte[] serialized = wrapped.ToBytes();

            using (FastBufferWriter writer = new FastBufferWriter(FastBufferWriter.GetWriteSize(serialized), Unity.Collections.Allocator.Temp))
            {
                writer.WriteValueSafe(serialized);

                if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
                {
                    NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll(UniqueName, writer, NetworkDelivery.ReliableFragmentedSequenced);
                }
                else
                {
                    NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(Network.MESSAGE_RELAY_UNIQUE_NAME, Player.HostPlayer.ClientId, writer, NetworkDelivery.ReliableFragmentedSequenced);
                }
            }
        }

        public override void Read(ulong fakeSender, FastBufferReader reader)
        {
            if (Player.LocalPlayer == null || Player.HostPlayer == null)
            {
                NetworkManager.Singleton.StartCoroutine(ReadLater(fakeSender, reader));
                return;
            }

            byte[] data;

            reader.ReadValueSafe(out data);

            NetworkMessageWrapper wrapped = data.ToObject<NetworkMessageWrapper>();

            if (!RelayToSelf && Player.LocalPlayer.ClientId == wrapped.Sender) return;
            OnReceived.Invoke(wrapped.Sender);
        }

        private IEnumerator SendLater()
        {
            int timesWaited = 0;
            while (Player.LocalPlayer == null || Player.HostPlayer == null)
            {
                yield return new WaitForSeconds(0.1f);
                timesWaited++;
                
                if (timesWaited % 20 == 0)
                {
                    Plugin.Log.LogWarning($"Waiting to send network message. Waiting on host?: {Player.HostPlayer == null} Waiting on local player?: {Player.LocalPlayer == null}");
                }

                if (timesWaited >= 100)
                {
                    Plugin.Log.LogError("Dropping network message");
                    yield return null;
                }
            }

            Send();
        }

        private IEnumerator ReadLater(ulong fakeSender, FastBufferReader reader)
        {
            int timesWaited = 0;
            while (Player.LocalPlayer == null || Player.HostPlayer == null)
            {
                yield return new WaitForSeconds(0.1f);
                timesWaited++;

                if (timesWaited % 20 == 0)
                {
                    Plugin.Log.LogWarning($"Waiting to read network message. Waiting on host?: {Player.HostPlayer == null} Waiting on local player?: {Player.LocalPlayer == null}");
                }

                if (timesWaited >= 100)
                {
                    Plugin.Log.LogError("Dropping network message");
                    yield return null;
                }
            }

            Read(fakeSender, reader);
        }
    }

    internal class NetworkMessageFinalizer<T> : NetworkMessageFinalizerBase where T : class
    {
        internal override string UniqueName { get; }

        internal override bool RelayToSelf { get; }

        internal Action<ulong, T> OnReceived { get; }

        public NetworkMessageFinalizer(string uniqueName, bool relayToSelf, Action<ulong, T> onReceived)
        {
            UniqueName = uniqueName;
            RelayToSelf = relayToSelf;
            OnReceived = onReceived;
        }

        public void Send(T obj)
        {
            if (Player.LocalPlayer == null || Player.HostPlayer == null)
            {
                NetworkManager.Singleton.StartCoroutine(SendLater(obj));
                return;
            }

            NetworkMessageWrapper wrapped = new NetworkMessageWrapper(UniqueName, Player.LocalPlayer.ClientId, obj.ToBytes());
            byte[] serialized = wrapped.ToBytes();

            using (FastBufferWriter writer = new FastBufferWriter(FastBufferWriter.GetWriteSize(serialized), Unity.Collections.Allocator.Temp))
            {
                writer.WriteValueSafe(serialized);

                if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
                {
                    NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll(UniqueName, writer, NetworkDelivery.ReliableFragmentedSequenced);
                }
                else
                {
                    NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(Network.MESSAGE_RELAY_UNIQUE_NAME, Player.HostPlayer.ClientId, writer, NetworkDelivery.ReliableFragmentedSequenced);
                }
            }
        }

        public override void Read(ulong fakeSender, FastBufferReader reader)
        {
            if (Player.LocalPlayer == null || Player.HostPlayer == null)
            {
                NetworkManager.Singleton.StartCoroutine(ReadLater(fakeSender, reader));
                return;
            }

            reader.ReadValueSafe(out byte[] data);

            NetworkMessageWrapper wrapped = data.ToObject<NetworkMessageWrapper>();

            if (!RelayToSelf && Player.LocalPlayer.ClientId == wrapped.Sender) return;

            OnReceived.Invoke(wrapped.Sender, wrapped.Message.ToObject<T>());
        }

        private IEnumerator SendLater(T obj)
        {
            int timesWaited = 0;

            while (Player.LocalPlayer == null || Player.HostPlayer == null)
            {
                yield return new WaitForSeconds(0.1f);

                timesWaited++;

                if (timesWaited % 20 == 0)
                {
                    Plugin.Log.LogWarning($"Waiting to send network message. Waiting on host?: {Player.HostPlayer == null} Waiting on local player?: {Player.LocalPlayer == null}");
                }

                if (timesWaited >= 100)
                {
                    Plugin.Log.LogError("Dropping network message");
                    yield return null;
                }
            }

            Send(obj);
        }

        private IEnumerator ReadLater(ulong fakeSender, FastBufferReader reader)
        {
            int timesWaited = 0;
            while (Player.LocalPlayer == null || Player.HostPlayer == null)
            {
                yield return new WaitForSeconds(0.1f);
                timesWaited++;

                if (timesWaited % 20 == 0)
                {
                    Plugin.Log.LogWarning($"Waiting to read network message. Waiting on host?: {Player.HostPlayer == null} Waiting on local player?: {Player.LocalPlayer == null}");
                }

                if (timesWaited >= 100)
                {
                    Plugin.Log.LogError("Dropping network message");
                    yield return null;
                }
            }

            Read(fakeSender, reader);
        }
    }

    internal class NetworkMessageWrapper
    {
        public string UniqueName { get; set; }

        public ulong Sender { get; set; }

        public byte[] Message { get; set; }

        internal NetworkMessageWrapper(string uniqueName, ulong sender)
        {
            UniqueName = uniqueName;
            Sender = sender;
        }

        internal NetworkMessageWrapper(string uniqueName, ulong sender, byte[] message)
        {
            UniqueName = uniqueName;
            Sender = sender;
            Message = message;
        }

        internal NetworkMessageWrapper() { }
    }

    internal static class RegisterPatch
    {
        internal static void Postfix()
        {
            Network.OnRegisterNetworkMessages();
        }
    }

    internal static class UnregisterPatch
    {
        internal static void Postfix()
        {
            Network.OnUnregisterNetworkMessages();
        }
    }
}
