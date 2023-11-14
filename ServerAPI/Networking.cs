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
    /// Networking solution to broadcast and receive data over the server. Use the delegates GetString, GetInt, GetFloat, and GetVector3 for receiving data.
    /// </summary>
    public class Networking
    {
        public delegate void GotStringEventDelegate(string data, string signature, bool gotEqualData);
        public delegate void GotIntEventDelegate(int data, string signature, bool gotEqualData);
        public delegate void GotFloatEventDelegate(float data, string signature, bool gotEqualData);
        public delegate void GotVector3EventDelegate(UnityEngine.Vector3 data, string signature, bool gotEqualData);

        /// <summary>
        /// Delegate for receiving a string value. The boolean "GotEqualData" is true if the data is the same as the last data received. You should always check for this, because the player broadcasting data will always receive 2 copies of the data.
        /// </summary>
        public static GotStringEventDelegate GetString = GotString;
        /// <summary>
        /// Delegate for receiving a int value. The boolean "GotEqualData" is true if the data is the same as the last data received. You should always check for this, because the player broadcasting data will always receive 2 copies of the data.
        /// </summary>
        public static GotIntEventDelegate GetInt = GotInt;
        /// <summary>
        /// Delegate for receiving a float value. The boolean "GotEqualData" is true if the data is the same as the last data received. You should always check for this, because the player broadcasting data will always receive 2 copies of the data.
        /// </summary>
        public static GotFloatEventDelegate GetFloat = GotFloat;
        /// <summary>
        /// Delegate for receiving a Vector3 value. The boolean "GotEqualData" is true if the data is the same as the last data received. You should always check for this, because the player broadcasting data will always receive 2 copies of the data.
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
            HUDManager.Instance.AddTextToChatOnServer("<size=0>NWE/" + data + "/" + signature + "/" + NetworkBroadcastDataType.BDstring.ToString() + "/</size>");
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

        private static void GotString(string data, string signature, bool gotEqualData)
        {
        }

        private static void GotInt(int data, string signature, bool gotEqualData)
        {
        }

        private static void GotFloat(float data, string signature, bool gotEqualData)
        {
        }

        private static void GotVector3(UnityEngine.Vector3 data, string signature, bool gotEqualData)
        {
        }
    }
}
