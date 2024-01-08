using LC_API.GameInterfaceAPI;
using LC_API.GameInterfaceAPI.Features;
using LC_API.ServerAPI;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LC_API.Comp
{
    internal class LC_APIManager : MonoBehaviour
    {
        public static MenuManager MenuManager;
        private static int playerCount;
        private static bool wanttoCheckMods;
        private static float lobbychecktimer;

        public void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void Update()
        {
            GameState.GSUpdate();
            GameTips.UpdateInternal();
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

            if (GameNetworkManager.Instance != null)
            {
                if (playerCount < GameNetworkManager.Instance.connectedPlayers)
                {
                    lobbychecktimer = -4.5f;
                    wanttoCheckMods = true;
                }
                playerCount = GameNetworkManager.Instance.connectedPlayers;
            }
            if (lobbychecktimer < 0)
            {
                lobbychecktimer += Time.deltaTime;
            }
            else if (wanttoCheckMods && HUDManager.Instance != null && Player.HostPlayer != null && Player.LocalPlayer != null)
            {
                wanttoCheckMods = false;
                CD();
            }
        }

        // For pre-placed items
        internal void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            foreach (GrabbableObject grabbable in FindObjectsOfType<GrabbableObject>())
            {
                if (!grabbable.TryGetComponent(out GameInterfaceAPI.Features.Item _))
                {
                    grabbable.gameObject.AddComponent<GameInterfaceAPI.Features.Item>();
                }
            }
        }

        private void CD()
        {
            CheatDatabase.OtherPlayerCheatDetector();
        }
    }
}
