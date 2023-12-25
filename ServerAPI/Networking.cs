using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using LC_API.Data;
using LC_API.Extensions;
using LC_API.GameInterfaceAPI.Events;
using LC_API.GameInterfaceAPI.Events.EventArgs.Player;
using LC_API.GameInterfaceAPI.Events.Patches.Player;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Netcode;
using UnityEngine;
using static LC_API.GameInterfaceAPI.Events.Events;
using static LC_API.ServerAPI.Networking;
using static UnityEngine.InputSystem.InputRemoting;

namespace LC_API.ServerAPI
{
    public static partial class Networking
    {
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

            public NetworkMessage(string uniqueName)
            {
                UniqueName = uniqueName;
            }
        }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
        public class SyncVar : Attribute
        {
            public SyncVar(NetworkVariableReadPermission readPermission = NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission writePermission = NetworkVariableWritePermission.Owner)
            {
                
            }
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

            public abstract void Read(ulong sender, FastBufferReader reader);
        }

        internal class NetworkMessageFinalizer : NetworkMessageFinalizerBase
        {
            internal override string UniqueName { get; }

            internal Action<ulong> OnReceived { get; }

            public NetworkMessageFinalizer(string uniqueName, Action<ulong> onReceived)
            {
                UniqueName = uniqueName;
                OnReceived = onReceived;
            }

            public void Send()
            {
                using (FastBufferWriter writer = new FastBufferWriter(0, Unity.Collections.Allocator.Temp))
                {
                    NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll(UniqueName, writer);
                }
            }

            public override void Read(ulong sender, FastBufferReader reader)
            {
                OnReceived.Invoke(sender);
            }
        }


        internal class NetworkMessageFinalizer<T> : NetworkMessageFinalizerBase where T : class
        {
            internal override string UniqueName { get; }

            internal Action<ulong, T> OnReceived { get; }

            public NetworkMessageFinalizer(string uniqueName, Action<ulong, T> onReceived)
            {
                if (typeof(T).GetCustomAttribute(typeof(System.SerializableAttribute), true) == null)
                    throw new Exception("T must be serializable.");

                UniqueName = uniqueName;
                OnReceived = onReceived;
            }

            public void Send(T obj)
            {
                byte[] serialized = obj.ToBytes();

                using (FastBufferWriter writer = new FastBufferWriter(FastBufferWriter.GetWriteSize(serialized), Unity.Collections.Allocator.Temp))
                {
                    writer.WriteValueSafe(serialized);
                    NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll(UniqueName, writer);
                }
            }

            public override void Read(ulong sender, FastBufferReader reader)
            {
                byte[] data;

                reader.ReadValueSafe(out data);

                T obj = data.ToObject<T>();

                OnReceived.Invoke(sender, obj);
            }
        }

        internal static Dictionary<string, NetworkMessageFinalizerBase> NetworkMessageFinalizers { get; } = new Dictionary<string, NetworkMessageFinalizerBase>();

        internal static byte[] ToBytes(this object @object)
        {
            if (@object == null) return null;

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();

            binaryFormatter.Serialize(memoryStream, @object);

            return memoryStream.ToArray();
        }

        internal static T ToObject<T>(this byte[] bytes) where T : class
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();

            memoryStream.Write(bytes, 0, bytes.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return binaryFormatter.Deserialize(memoryStream) as T;
        }

        internal static bool StartedNetworking { get; set; } = false;

        /// <summary>
        /// Provides an event in which registering network messages should happen.
        /// </summary>
        public static event CustomEventHandler RegisterNetworkMessages;

        internal static event CustomEventHandler UnregisterNetworkMessages;

        internal static void OnRegisterNetworkMessages() => RegisterNetworkMessages.InvokeSafely();

        internal static void OnUnregisterNetworkMessages() => UnregisterNetworkMessages.InvokeSafely();

        internal static void RegisterAllMessages()
        {
            foreach (NetworkMessageFinalizerBase handler in NetworkMessageFinalizers.Values)
            {
                NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(handler.UniqueName, handler.Read);
            }

            MethodInfo registerInfo = null;
            MethodInfo registerInfoGeneric = null;

            foreach (MethodInfo methodInfo in typeof(Networking).GetMethods())
            {
                if (methodInfo.Name == nameof(RegisterMessage) && methodInfo.IsGenericMethod)
                {
                    registerInfoGeneric = methodInfo;
                } 
                else if (methodInfo.Name == nameof(RegisterMessage) && !methodInfo.IsGenericMethod)
                {
                    registerInfo = methodInfo;
                }

                if (registerInfo != null && registerInfoGeneric != null) break;
            }

            foreach (PluginInfo pluginInfo in Chainloader.PluginInfos.Values)
            {
                foreach (Type type in pluginInfo.Instance.GetType().Assembly.GetTypes())
                {
                    if (type.IsClass)
                    {
                        NetworkMessage networkMessage = type.GetCustomAttribute<NetworkMessage>();

                        if (networkMessage != null)
                        {
                            if (type.BaseType.Name == "NetworkMessageHandler`1")
                            {
                                Type messageType = type.BaseType.GetGenericArguments()[0];
                                registerInfoGeneric
                                    .MakeGenericMethod(messageType)
                                    .Invoke(null, new object[]
                                    {
                                        networkMessage.UniqueName,
                                        type.GetMethod("Handler").CreateDelegate(typeof(Action<,>)
                                            .MakeGenericType(typeof(ulong), messageType), Activator.CreateInstance(type))
                                    });
                            } 
                            else if (type.BaseType.Name == "NetworkMessageHandler")
                            {
                                registerInfo
                                    .Invoke(null, new object[]
                                    {
                                        networkMessage.UniqueName,
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
                                        registerInfo
                                            .Invoke(null, new object[]
                                            {
                                                networkMessage.UniqueName,
                                                method.CreateDelegate(typeof(Action<>)
                                                    .MakeGenericType(typeof(ulong)))
                                            });
                                    }
                                    else
                                    {
                                        Type messageType = method.GetParameters()[1].ParameterType;
                                        registerInfoGeneric
                                            .MakeGenericMethod(messageType)
                                            .Invoke(null, new object[]
                                            {
                                            networkMessage.UniqueName,
                                            method.CreateDelegate(typeof(Action<,>)
                                                .MakeGenericType(typeof(ulong), messageType))
                                            });
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static void UnregisterAllMessages()
        {

            foreach (string name in NetworkMessageFinalizers.Keys.ToArray())
            {
                UnregisterMessage(name);
            }
        }

        /// <summary>
        /// Registers a network message with a name and handler.
        /// </summary>
        /// <typeparam name="T">The type of the network message.</typeparam>
        /// <param name="uniqueName">The name of the network message.</param>
        /// <param name="onReceived">The handler to use for the message.</param>
        /// <exception cref="Exception">Thrown when T is not serializable, or if the name is already taken.</exception>
        public static void RegisterMessage<T>(string uniqueName, Action<ulong, T> onReceived) where T : class
        {
            if (typeof(T).GetCustomAttribute(typeof(System.SerializableAttribute), true) == null)
                throw new Exception("T must be serializable.");

            if (NetworkMessageFinalizers.ContainsKey(uniqueName))
                throw new Exception($"{uniqueName} already registered");

            NetworkMessageFinalizer<T> networkMessageHandler = new NetworkMessageFinalizer<T>(uniqueName, onReceived);

            NetworkMessageFinalizers.Add(uniqueName, networkMessageHandler);

            if (StartedNetworking)
                NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(uniqueName, networkMessageHandler.Read);
        }

        /// <summary>
        /// Registers a network message with a name and handler.
        /// </summary>
        /// <param name="uniqueName">The name of the network message.</param>
        /// <param name="onReceived">The handler to use for the message.</param>
        /// <exception cref="Exception">Thrown when the name is already taken.</exception>
        public static void RegisterMessage(string uniqueName, Action<ulong> onReceived)
        {
            if (NetworkMessageFinalizers.ContainsKey(uniqueName))
                throw new Exception($"{uniqueName} already registered");

            NetworkMessageFinalizer networkMessageHandler = new NetworkMessageFinalizer(uniqueName, onReceived);

            NetworkMessageFinalizers.Add(uniqueName, networkMessageHandler);

            if (StartedNetworking)
                NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(uniqueName, networkMessageHandler.Read);
        }

        /// <summary>
        /// Unregisters a network message.
        /// </summary>
        /// <param name="uniqueName">The name of the message to unregister.</param>
        public static void UnregisterMessage(string uniqueName)
        {
            if (NetworkMessageFinalizers.Remove(uniqueName))
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

                RegisterAllMessages();
            };

            UnregisterNetworkMessages += () =>
            {
                StartedNetworking = false;

                UnregisterAllMessages();
            };

            SetupNetworking();
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

    internal static class RegisterPatch
    {
        internal static void Postfix()
        {
            OnRegisterNetworkMessages();
        }
    }

    internal static class UnregisterPatch
    {
        internal static void Postfix()
        {
            OnUnregisterNetworkMessages();
        }
    }
}
