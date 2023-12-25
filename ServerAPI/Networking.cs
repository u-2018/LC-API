using HarmonyLib;
using LC_API.Data;
using LC_API.GameInterfaceAPI.Events;
using LC_API.GameInterfaceAPI.Events.EventArgs.Player;
using LC_API.GameInterfaceAPI.Events.Patches.Player;
using System;
using System.Collections.Generic;
using System.IO;
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
        public class NetworkMessage<T> where T : class
        {
            public ulong SenderId { get; }

            public T Message { get; }

            public NetworkMessage(ulong sender, T message)
            {
                SenderId = sender;
                Message = message;
            }
        }


        public abstract class NetworkMessageHandler { }

        public class NetworkMessageHandler<T> : NetworkMessageHandler where T : class
        {
            internal string UniqueName { get; }

            internal Action<NetworkMessage<T>> OnReceived { get; }

            public NetworkMessageHandler(string uniqueName, Action<NetworkMessage<T>> onReceived)
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

            public void Read(ulong sender, FastBufferReader reader)
            {
                byte[] data;

                reader.ReadValueSafe(out data);

                OnReceived.Invoke(new NetworkMessage<T>(sender, data.ToObject<T>()));
            }
        }

        internal static Dictionary<string, NetworkMessageHandler> NetworkMessageHandlers { get; } = new Dictionary<string, NetworkMessageHandler>();

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

        public static event CustomEventHandler RegisterNetworkMessages;

        internal static void OnRegisterNetworkMessages() => RegisterNetworkMessages.InvokeSafely();

        public static void RegisterMessage<T>(string uniqueName, Action<NetworkMessage<T>> onReceived) where T : class
        {
            if (typeof(T).GetCustomAttribute(typeof(System.SerializableAttribute), true) == null)
                throw new Exception("T must be serializable.");

            if (NetworkMessageHandlers.ContainsKey(uniqueName))
                throw new Exception($"{uniqueName} already registered");

            NetworkMessageHandler<T> networkMessageHandler = new NetworkMessageHandler<T>(uniqueName, onReceived);

            NetworkMessageHandlers.Add(uniqueName, networkMessageHandler);

            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(uniqueName, networkMessageHandler.Read);
        }

        public static void UnregisterMessage<T>(string uniqueName) where T : class
        {
            if (NetworkMessageHandlers.Remove(uniqueName))
            {
                NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler(uniqueName);
            }
        }

        public static void Broadcast<T>(string uniqueName, T @object) where T : class
        {
            if (NetworkMessageHandlers.TryGetValue(uniqueName, out NetworkMessageHandler handler))
            {
                if (handler is NetworkMessageHandler<T> genericHandler)
                {
                    genericHandler.Send(@object);
                }
            }
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
}
