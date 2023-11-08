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
    /// Netoworking solution to broadcast and receive data over the server. Use the delegates GetString, GetInt, GetFloat, and GetVector3 for receiving data.
    /// </summary>
    public class Networking
    {
        public delegate void GotStringEventDelegate(string data, string signature);
        public delegate void GotIntEventDelegate(int data, string signature);
        public delegate void GotFloatEventDelegate(float data, string signature);
        public delegate void GotVector3EventDelegate(UnityEngine.Vector3 data, string signature);

        public static GotStringEventDelegate GetString = GotString;
        public static GotIntEventDelegate GetInt = GotInt;
        public static GotFloatEventDelegate GetFloat = GotFloat;
        public static GotVector3EventDelegate GetVector3 = GotVector3;

        /// <summary>
        /// Send data across the network. The signature is an identifier for use when receiving data.
        /// </summary>
        public void Broadcast(string data, string signature)
        {
            if (data.Contains("/"))
            {
                Plugin.Log.LogError("Invalid character in broadcasted string event! ( / )");
                return;
            }
            HUDManager.Instance.AddTextToChatOnServer("<size=0>NWE/" + data + "/" + signature + "/" + NetworkBroadcastDataType.BDstring + "/</size>");
        }

        /// <summary>
        /// Send data across the network. The signature is an identifier for use when receiving data.
        /// </summary>
        public static void Broadcast(int data, string signature)
        {
            HUDManager.Instance.AddTextToChatOnServer("<size=0>NWE/" + data + "/" + signature + "/" + NetworkBroadcastDataType.BDint + "/</size>");
        }

        /// <summary>
        /// Send data across the network. The signature is an identifier for use when receiving data.
        /// </summary>
        public static void Broadcast(float data, string signature)
        {
            HUDManager.Instance.AddTextToChatOnServer("<size=0>NWE/" + data + "/" + signature + "/" + NetworkBroadcastDataType.BDfloat + "/</size>");
        }

        /// <summary>
        /// Send data across the network. The signature is an identifier for use when receiving data.
        /// </summary>
        public static void Broadcast(Vector3 data, string signature)
        {
            HUDManager.Instance.AddTextToChatOnServer("<size=0>NWE/" + data + "/" + signature + "/" + NetworkBroadcastDataType.BDvector3 + "/</size>");
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
