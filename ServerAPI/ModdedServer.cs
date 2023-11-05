using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace LC_API.ServerAPI
{
    public class ModdedServer
    {
        private static bool moddedOnly;
        public static bool setModdedOnly;

        public static bool ModdedOnly
        {
            get { return moddedOnly; }
        }

        public static void SetServerModdedOnly()
        {
            moddedOnly = true;
            Plugin.Log.LogMessage("A plugin has set your game to only allow you to play with other people who have mods!");
        }

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
