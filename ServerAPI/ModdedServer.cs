using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

#pragma warning disable CS0618 // Member is obsolete
namespace LC_API.ServerAPI
{
    /// <summary>
    /// You're probably here for <see cref="SetServerModdedOnly"/>
    /// </summary>
    public static class ModdedServer
    {
        private static bool moddedOnly;
        [Obsolete("Use SetServerModdedOnly() instead. This will be removed/private in a future update.")]
        public static bool setModdedOnly; // obsolete for the purposes of getting peoples' IDE's to bitch at them.

        /// <summary>
        /// Has the user been placed in modded only servers?
        /// </summary>
        public static bool ModdedOnly
        {
            get { return moddedOnly; }
        }

        /// <summary>
        /// Call this method to make your plugin place the user in modded only servers.
        /// </summary>
        public static void SetServerModdedOnly()
        {
            moddedOnly = true;
            Plugin.Log.LogMessage("A plugin has set your game to only allow you to play with other people who have mods!");
        }

        /// <summary>
        /// For internal use. Do not call this method.
        /// </summary>
        public static void OnSceneLoaded()
        {
            if (GameNetworkManager.Instance && ModdedOnly)
            {
                GameNetworkManager.Instance.gameVersionNum += 16440;
                setModdedOnly = true;
            }
        }
    }
}
#pragma warning restore CS0618 // Member is obsolete
