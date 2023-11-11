namespace LC_API.Comp;

// ReSharper disable RedundantNameQualifier
using LC_API.ServerAPI;
using UnityEngine;

// Field should be made private.
#pragma warning disable SA1401

/// <summary>
/// The main Server List Api.
/// </summary>
// ReSharper disable once InconsistentNaming
// ReSharper disable once IdentifierTypo
internal class SVAPI : MonoBehaviour
{
    /// <summary>
    /// Represents the menu manager object.
    /// </summary>
    public static MenuManager MenuManager;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    public void Update()
    {
        // Does this really need to be called once a frame? It seems expensive and unnecessary. Perhaps we can disable the mono-behaviour after is done?
        if (!ModdedServer.SetModdedOnly)
        {
            ModdedServer.OnSceneLoaded();
        }
        else if (ModdedServer.ModdedOnly)
        {
            if (MenuManager is not null)
            {
                if (MenuManager.versionNumberText)
                {
                    MenuManager.versionNumberText.text = $"v{GameNetworkManager.Instance.gameVersionNum - 16440}\nMOD";
                }
            }
        }
    }
}