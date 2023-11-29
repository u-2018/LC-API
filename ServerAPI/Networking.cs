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
        public static void Broadcast(UnityEngine.Vector3 data, string signature)
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
