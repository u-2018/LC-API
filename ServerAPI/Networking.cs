using LC_API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using static LC_API.BundleAPI.BundleLoader;

namespace LC_API.ServerAPI
{
    /// <summary>
    /// Networking solution to broadcast and receive data over the server. Use the delegates GetString, GetInt, GetFloat, and GetVector3 for receiving data. Note that the local player will not receive data that they broadcast.
    /// <para>The second parameter for each of the events is the signature string.</para>
    /// </summary>
    public static class Networking
    {
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
            HUDManager.Instance.AddTextToChatOnServer("<size=0>NWE/" + data + "/" + signature + "/" + NetworkBroadcastDataType.BDstring.ToString() + "/" + GameNetworkManager.Instance.localPlayerController.playerClientId + "/" + "</size>");
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
            HUDManager.Instance.AddTextToChatOnServer("<size=0>NWE/" + data + "/" + signature + "/" + NetworkBroadcastDataType.BDlistString.ToString() + "/" + GameNetworkManager.Instance.localPlayerController.playerClientId + "/" + "</size>");
        }

        /// <summary>
        /// Send data across the network. The signature is an identifier for use when receiving data.
        /// </summary>
        public static void Broadcast(int data, string signature)
        {
            HUDManager.Instance.AddTextToChatOnServer("<size=0>NWE/" + data + "/" + signature + "/" + NetworkBroadcastDataType.BDint.ToString() + "/" + GameNetworkManager.Instance.localPlayerController.playerClientId + "/" + "</size>");
        }

        /// <summary>
        /// Send data across the network. The signature is an identifier for use when receiving data.
        /// </summary>
        public static void Broadcast(float data, string signature)
        {
            HUDManager.Instance.AddTextToChatOnServer("<size=0>NWE/" + data + "/" + signature + "/" + NetworkBroadcastDataType.BDfloat.ToString() + "/" + GameNetworkManager.Instance.localPlayerController.playerClientId + "/" + "</size>");
        }

        /// <summary>
        /// Send data across the network. The signature is an identifier for use when receiving data.
        /// </summary>
        public static void Broadcast(UnityEngine.Vector3 data, string signature)
        {
            HUDManager.Instance.AddTextToChatOnServer("<size=0>NWE/" + data + "/" + signature + "/" + NetworkBroadcastDataType.BDvector3.ToString() + "/" + GameNetworkManager.Instance.localPlayerController.playerClientId + "/" + "</size>");
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
                Broadcast(syncString, "LCAPI_NET_SYNCVAR_SET");
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
            if (arg2 == "LCAPI_NET_SYNCVAR_SET")
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

        private static void GotString(string data, string signature)
        {
        }

        private static void GotInt(int data, string signature)
        {
        }

        private static void GotFloat(float data, string signature)
        {
        }

        private static void GotVector3(UnityEngine.Vector3 data, string signature)
        {
        }

        
    }
}
