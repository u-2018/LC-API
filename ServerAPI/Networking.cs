using LC_API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static LC_API.BundleAPI.BundleLoader;

namespace LC_API.ServerAPI
{
    /// <summary>
    /// Networking solution to broadcast and receive data over the server. Use the delegates GetString, GetInt, GetFloat, and GetVector3 for receiving data. Note that the local player will not receive data that they broadcast.
    /// </summary>
    public class Networking
    {
        public delegate void GotStringEventDelegate(string data, string signature);
        public delegate void GotIntEventDelegate(int data, string signature);
        public delegate void GotFloatEventDelegate(float data, string signature);
        public delegate void GotVector3EventDelegate(UnityEngine.Vector3 data, string signature);

        /// <summary>
        /// Delegate for receiving a string value.
        /// </summary>
        public static GotStringEventDelegate GetString = GotString;
        /// <summary>
        /// Delegate for receiving a int value. 
        /// </summary>
        public static GotIntEventDelegate GetInt = GotInt;
        /// <summary>
        /// Delegate for receiving a float value. 
        /// </summary>
        public static GotFloatEventDelegate GetFloat = GotFloat;
        /// <summary>
        /// Delegate for receiving a Vector3 value. 
        /// </summary>
        public static GotVector3EventDelegate GetVector3 = GotVector3;

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
        public static void Broadcast(Vector3 data, string signature)
        {
            HUDManager.Instance.AddTextToChatOnServer("<size=0>NWE/" + data + "/" + signature + "/" + NetworkBroadcastDataType.BDvector3.ToString() + "/" + GameNetworkManager.Instance.localPlayerController.playerClientId + "/" + "</size>");
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
