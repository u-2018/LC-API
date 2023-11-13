using LC_API.ServerAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LC_API.Comp
{
    internal class SVAPI : MonoBehaviour
    {
        public static MenuManager MenuManager;
        public float netTester;
        public void Update()
        {
            if (!ModdedServer.setModdedOnly)
            {
                ModdedServer.OnSceneLoaded();
            }
            else if (ModdedServer.ModdedOnly)
            {
                if (MenuManager != null)
                {
                    if (MenuManager.versionNumberText)
                    {
                        MenuManager.versionNumberText.text = $"v{GameNetworkManager.Instance.gameVersionNum - 16440}\nMOD";
                    }
                }
            }
        }
    }
}
