namespace LC_API.ServerAPI;

using System.Numerics;

// ReSharper disable RedundantNameQualifier
// ReSharper disable FieldCanBeMadeReadOnly.Global
using LC_API.Data;

// Field can be made readonly.
#pragma warning disable SA1401

/// <summary>
/// Networking solution to broadcast and receive data over the server. Use the delegates GetString, GetInt, GetFloat, and GetVector3 for receiving data.
/// </summary>
// ReSharper disable ClassNeverInstantiated.Global
public sealed class Networking
{
    /// <summary>
    /// Used to invoke <see cref="GotString"/>.
    /// </summary>
    public static GotStringEventDelegate GetString = GotString;

    /// <summary>
    /// Used to invoke <see cref="GotInt"/>.
    /// </summary>
    public static GotIntEventDelegate GetInt = GotInt;

    /// <summary>
    /// Used to invoke <see cref="GotFloat"/>.
    /// </summary>
    public static GotFloatEventDelegate GetFloat = GotFloat;

    /// <summary>
    /// Used to invoke <see cref="GotVector3"/>.
    /// </summary>
    public static GotVector3EventDelegate GetVector3 = GotVector3;

    /// <summary>
    /// Called when data is received from the network in a string form.
    /// </summary>
    /// <param name="data">The data that is received.</param>
    /// <param name="signature">The signature that is received.</param>
    public delegate void GotStringEventDelegate(string data, string signature);

    /// <summary>
    /// Called when data is received from the network in an int form.
    /// </summary>
    /// <param name="data">The data that is received.</param>
    /// <param name="signature">The signature that is received.</param>
    public delegate void GotIntEventDelegate(int data, string signature);

    /// <summary>
    /// Called when data is received from the network in a float form.
    /// </summary>
    /// <param name="data">The data that is received.</param>
    /// <param name="signature">The signature that is received.</param>
    public delegate void GotFloatEventDelegate(float data, string signature);

    /// <summary>
    /// Called when data is received from the network in a vector form.
    /// </summary>
    /// <param name="data">The data that is received.</param>
    /// <param name="signature">The signature that is received.</param>
    public delegate void GotVector3EventDelegate(UnityEngine.Vector3 data, string signature);

    /// <summary>
    /// Send data across the network. The signature is an identifier for use when receiving data.
    /// </summary>
    /// <param name="data">The data that is received.</param>
    /// <param name="signature">The signature that is received.</param>
    public static void Broadcast(int data, string signature)
    {
        HUDManager.Instance.AddTextToChatOnServer("<size=0>NWE/" + data + "/" + signature + "/" + NetworkBroadcastDataType.BDint + "/</size>");
    }

    /// <summary>
    /// Send data across the network. The signature is an identifier for use when receiving data.
    /// </summary>
    /// <param name="data">The data that is received.</param>
    /// <param name="signature">The signature that is received.</param>
    public static void Broadcast(float data, string signature)
    {
        HUDManager.Instance.AddTextToChatOnServer("<size=0>NWE/" + data + "/" + signature + "/" + NetworkBroadcastDataType.BDfloat + "/</size>");
    }

    /// <summary>
    /// Send data across the network. The signature is an identifier for use when receiving data.
    /// </summary>
    /// <param name="data">The data that is received.</param>
    /// <param name="signature">The signature that is received.</param>
    public static void Broadcast(Vector3 data, string signature)
    {
        HUDManager.Instance.AddTextToChatOnServer("<size=0>NWE/" + data + "/" + signature + "/" + NetworkBroadcastDataType.BDvector3 + "/</size>");
    }

    /// <summary>
    /// Used to send data across the network. The signature is an identifier for use when receiving data.
    /// </summary>
    /// <param name="data">The data that is received.</param>
    /// <param name="signature">The signature that is received.</param>
    public void Broadcast(string data, string signature)
    {
        if (data.Contains("/"))
        {
            Plugin.Log.LogError("Invalid character in broadcast string event! ( / )");
            return;
        }

        HUDManager.Instance.AddTextToChatOnServer("<size=0>NWE/" + data + "/" + signature + "/" + NetworkBroadcastDataType.BDstring + "/</size>");
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