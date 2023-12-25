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

namespace LC_API.ServerAPI
{
    public static partial class Networking
    {

        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
        public class NetworkMessage : Attribute
        {
            public string UniqueName { get; }

            public NetworkMessage(string uniqueName)
            {
                UniqueName = uniqueName;
            }
        }

        public abstract class NetworkMessageHandler<T> where T : class
        {
            public abstract void Handler(ulong sender, T message);
        }

        internal abstract class NetworkMessageFinalizer
        {
            internal abstract string UniqueName { get; }

            public abstract void Read(ulong sender, FastBufferReader reader);
        }

        internal class NetworkMessageFinalizer<T> : NetworkMessageFinalizer where T : class
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

        internal static Dictionary<string, NetworkMessageFinalizer> NetworkMessageFinalizers { get; } = new Dictionary<string, NetworkMessageFinalizer>();

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
            foreach (NetworkMessageFinalizer handler in NetworkMessageFinalizers.Values)
            {
                NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(handler.UniqueName, handler.Read);
            }

            MethodInfo registerInfo = null;

            foreach (MethodInfo methodInfo in typeof(Networking).GetMethods())
            {
                if (methodInfo.Name == nameof(RegisterMessage))
                {
                    registerInfo = methodInfo;
                    break;
                }
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
                                registerInfo
                                    .MakeGenericMethod(messageType)
                                    .Invoke(null, new object[]
                                    {
                                        networkMessage.UniqueName,
                                        type.GetMethod("Handler").CreateDelegate(typeof(Action<,>)
                                            .MakeGenericType(typeof(ulong), messageType), Activator.CreateInstance(type))
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

                                    Type messageType = method.GetParameters()[1].ParameterType;
                                    registerInfo
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

        internal static void UnregisterAllMessages()
        {

            foreach (string name in NetworkMessageFinalizers.Keys.ToArray())
            {
                UnregisterMessage(name);
            }
        }

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

        public static void UnregisterMessage(string uniqueName)
        {
            if (NetworkMessageFinalizers.Remove(uniqueName))
            {
                NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler(uniqueName);
            }
        }

        public static void Broadcast<T>(string uniqueName, T @object) where T : class
        {
            if (NetworkMessageFinalizers.TryGetValue(uniqueName, out NetworkMessageFinalizer handler))
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
