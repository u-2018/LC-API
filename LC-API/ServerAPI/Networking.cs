using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LC_API.ServerAPI
{
    /// <summary>
    /// Networking solution to broadcast and receive data over the server. Use the delegates GetString, GetInt, GetFloat, and GetVector3 for receiving data. Note that the local player will not receive data that they broadcast.
    /// <para>The second parameter for each of the events is the signature string.</para>
    /// </summary>
    [Obsolete("ServerAPI.Networking is obsolete and will be removed in future versions. Use LC_API.Networking.Network.")]
    public static class Networking
    {
        private const string StringMessageRegistrationName = "LCAPI_NET_LEGACY_STRING";
        private const string ListStringMessageRegistrationName = "LCAPI_NET_LEGACY_LISTSTRING";
        private const string IntMessageRegistrationName = "LCAPI_NET_LEGACY_INT";
        private const string FloatMessageRegistrationName = "LCAPI_NET_LEGACY_FLOAT";
        private const string Vector3MessageRegistrationName = "LCAPI_NET_LEGACY_VECTOR3";

        private const string SyncVarMessageRegistrationName = "LCAPI_NET_LEGACY_SYNCVAR_SET";

        /// <summary>
        /// Delegate for receiving a string value. Second parameter is the signature.
        /// <para/> (that signature would have been the signature given to <see cref="Broadcast(string, string)"/>, for example)
        /// </summary>
        public static Action<string, string> GetString = (_, _) => { };
        /// <summary>
        /// Delegate for receiving a list of string values. Second parameter is the signature.
        /// <para/> (that signature would have been the signature given to <see cref="Broadcast(string, string)"/>, for example)
        /// </summary>
        public static Action<List<string>, string> GetListString = (_, _) => { };
        /// <summary>
        /// Delegate for receiving a int value. Second parameter is the signature.
        /// <para/> (that signature would have been the signature given to <see cref="Broadcast(string, string)"/>, for example)
        /// </summary>
        public static Action<int, string> GetInt = (_, _) => { };
        /// <summary>
        /// Delegate for receiving a float value. Second parameter is the signature.
        /// <para/> (that signature would have been the signature given to <see cref="Broadcast(string, string)"/>, for example)
        /// </summary>
        public static Action<float, string> GetFloat = (_, _) => { };
        /// <summary>
        /// Delegate for receiving a Vector3 value. Second parameter is the signature.
        /// <para/> (that signature would have been the signature given to <see cref="Broadcast(string, string)"/>, for example)
        /// </summary>
        public static Action<UnityEngine.Vector3, string> GetVector3 = (_, _) => { };

        private static Dictionary<string, string> syncStringVars = new Dictionary<string, string>();

        /// <summary>
        /// Send data across the network. The signature is an identifier for use when receiving data.
        /// </summary>
        public static void Broadcast(string data, string signature)
        {
            if (data.Contains("/"))
            {
                Plugin.Log.LogError("Invalid character in broadcasted string event! ( / )");
                return;
            }
            LC_API.Networking.Network.Broadcast(StringMessageRegistrationName, new Data<string>(signature: signature, value: data));
        }

        /// <summary>
        /// Send data across the network. The signature is an identifier for use when receiving data.
        /// </summary>
        public static void Broadcast(List<string> data, string signature)
        {
            string dataFormatted = "";
            foreach (var item in data)
            {
                if (item.Contains("/"))
                {
                    Plugin.Log.LogError("Invalid character in broadcasted string event! ( / )");
                    return;
                }
                if (item.Contains("\n"))
                {
                    Plugin.Log.LogError("Invalid character in broadcasted string event! ( NewLine )");
                    return;
                }
                dataFormatted += item + "\n";
            }
            LC_API.Networking.Network.Broadcast(ListStringMessageRegistrationName, new Data<string>(signature: signature, value: dataFormatted));
        }

        /// <summary>
        /// Send data across the network. The signature is an identifier for use when receiving data.
        /// </summary>
        public static void Broadcast(int data, string signature)
        {
            LC_API.Networking.Network.Broadcast(IntMessageRegistrationName, new Data<int>(signature: signature, value: data));
        }

        /// <summary>
        /// Send data across the network. The signature is an identifier for use when receiving data.
        /// </summary>
        public static void Broadcast(float data, string signature)
        {
            LC_API.Networking.Network.Broadcast(FloatMessageRegistrationName, new Data<float>(signature: signature, value: data));
        }

        /// <summary>
        /// Send data across the network. The signature is an identifier for use when receiving data.
        /// </summary>
        public static void Broadcast(Vector3 data, string signature)
        {
            LC_API.Networking.Network.Broadcast(Vector3MessageRegistrationName, new Data<Vector3>(signature: signature, value: data));
        }

        /// <summary>
        /// Register a Sync Variable. Currently Sync Variables can only store string values.
        /// </summary>
        public static void RegisterSyncVariable(string name)
        {
            if (!syncStringVars.ContainsKey(name))
            {
                syncStringVars.Add(name, "");
            }
            else
            {
                Plugin.Log.LogError("Cannot register Sync Variable! A Sync Variable has already been registered with name " + name);
            }
        }

        /// <summary>
        /// Set the value of a Sync Variable.
        /// </summary>
        public static void SetSyncVariable(string name, string value)
        {
            if (syncStringVars.ContainsKey(name))
            {
                syncStringVars[name] = value;
                List<string> syncString = new List<string>();
                syncString.Add(name);
                syncString.Add(value);
                Broadcast(syncString, SyncVarMessageRegistrationName);
            }
            else
            {
                Plugin.Log.LogError("Cannot set the value of Sync Variable " + name + " as it is not registered!");
            }
        }

        private static void SetSyncVariableB(string name, string value)
        {
            if (syncStringVars.ContainsKey(name))
            {
                syncStringVars[name] = value;
            }
            else
            {
                Plugin.Log.LogError("Cannot set the value of Sync Variable " + name + " as it is not registered!");
            }
        }

        internal static void LCAPI_NET_SYNCVAR_SET(List<string> list, string arg2)
        {
            if (arg2 == SyncVarMessageRegistrationName)
            {
                SetSyncVariableB(list[0], list[1]);
            }
        }

        /// <summary>
        /// Get the value of a Sync Variable.
        /// </summary>
        public static string GetSyncVariable(string name)
        {
            if (syncStringVars.ContainsKey(name))
            {
                return syncStringVars[name];
            }
            else
            {
                Plugin.Log.LogError("Cannot get the value of Sync Variable " + name + " as it is not registered!");
                return "";
            }
        }

        private sealed class Data<T>
        {
            public readonly string Signature;
            public readonly T Value;

            public Data(string signature, T value)
            {
                Signature = signature;
                Value = value;
            }
        }

        internal static void InitializeLegacyNetworking()
        {
            GetListString += LCAPI_NET_SYNCVAR_SET;

            LC_API.Networking.Network.RegisterMessage(
                StringMessageRegistrationName,
                false,
                (ulong senderId, Data<string> data) => GetString(data.Value, data.Signature));
            LC_API.Networking.Network.RegisterMessage(
                ListStringMessageRegistrationName,
                false,
                (ulong senderId, Data<string> data) => GetListString(data.Value.Split('\n').ToList(), data.Signature));
            LC_API.Networking.Network.RegisterMessage(
                IntMessageRegistrationName,
                false,
                (ulong senderId, Data<int> data) => GetInt(data.Value, data.Signature));
            LC_API.Networking.Network.RegisterMessage(
                FloatMessageRegistrationName,
                false,
                (ulong senderId, Data<float> data) => GetFloat(data.Value, data.Signature));
            LC_API.Networking.Network.RegisterMessage(
                Vector3MessageRegistrationName,
                false,
                (ulong senderId, Data<Vector3> data) => GetVector3(data.Value, data.Signature));
        }
    }
}