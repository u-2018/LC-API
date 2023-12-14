using BepInEx.Configuration;
using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.UIElements.StylePropertyAnimationSystem;

namespace LC_API.GameInterfaceAPI.Features
{
    /// <summary>
    /// Encapsulates a <see cref="PlayerControllerB"/> for earier interacting.
    /// </summary>
    public class Player : NetworkBehaviour
    {
        internal static GameObject PlayerNetworkPrefab { get; set; }

        /// <summary>
        /// Gets a dictionary containing all <see cref="Player"/>'s. Even inactive ones.
        /// </summary>
        public static Dictionary<PlayerControllerB, Player> Dictionary { get; } = new Dictionary<PlayerControllerB, Player>();

        /// <summary>
        /// Gets a list containing all <see cref="Player"/>'s. Even inactive ones.
        /// </summary>
        public static IReadOnlyCollection<Player> List => Dictionary.Values;

        /// <summary>
        /// Gets a list containing only the currently controlled <see cref="Player"/>'s.
        /// </summary>
        /// TODO: `.Where` is bad. Potentially add and remove from this list as needed with a patch.
        public static IReadOnlyCollection<Player> ActiveList => Dictionary.Values.Where(p => p.IsPlayerControlled).ToList();

        /// <summary>
        /// Gets the encapsulated <see cref="PlayerControllerB"/>.
        /// </summary>
        public PlayerControllerB PlayerController { get; internal set; }

        internal NetworkVariable<ulong> NetworkClientId { get; } = new NetworkVariable<ulong>();

        /// <summary>
        /// Gets the <see cref="Player"/>'s client id.
        /// </summary>
        public ulong ClientId => PlayerController.actualClientId;

        /// <summary>
        /// Gets the <see cref="Player"/>'s steam id.
        /// </summary>
        public ulong SteamId => PlayerController.playerSteamId;

        /// <summary>
        /// Gets whether or not the <see cref="Player"/> is the host.
        /// </summary>
        public new bool IsHost => PlayerController.isHostPlayerObject;

        /// <summary>
        /// Gets whether or not the <see cref="Player"/> is currently being controlled.
        /// Lethal Company creates PlayerControllers ahead of time, so all of them always exist.
        /// </summary>
        public bool IsPlayerControlled => PlayerController.isPlayerControlled;

        /// <summary>
        /// Gets or sets the <see cref="Player"/>'s sprint meter.
        /// </summary>
        /// <exception cref="Exception">Thrown when attempting to set position from the client.</exception>
        public float SprintMeter
        {
            get
            {
                return PlayerController.sprintMeter;
            }
            set
            {
                if (!(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)) throw new Exception("Tried to sprint meter on client.");

                PlayerController.sprintMeter = value;
                SetSprintMeterClientRpc(value);
            }
        }

        [ClientRpc]
        private void SetSprintMeterClientRpc(float value)
        {
            if (!NetworkManager.Singleton.IsClient) return;

            PlayerController.sprintMeter = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Player"/>'s position. 
        /// If you set a player's position out of bounds, they will be teleported back to a safe location next to the ship or entrance/exit to a dungeon.
        /// </summary>
        /// <exception cref="Exception">Thrown when attempting to set position from the client.</exception>
        public Vector3 Position
        {
            get
            {
                return PlayerController.transform.position;
            }
            set
            {
                Plugin.Log.LogInfo("SETTING POSITION");
                if (!(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)) throw new Exception("Tried to set position on client.");

                PlayerController.transform.position = value;
                PlayerController.serverPlayerPosition = value;

                TeleportPlayerClientRpc(value);
            }
        }

        // UpdatePlayerPositionClientRpc doesn't actually set the player's position, so we need a custom rpc to do so.
        [ClientRpc]
        private void TeleportPlayerClientRpc(Vector3 position)
        {
            if (!NetworkManager.Singleton.IsClient) return;

            PlayerController.TeleportPlayer(position);

            PlayerController.UpdatePlayerPositionServerRpc(position, PlayerController.isInElevator, PlayerController.isExhausted, PlayerController.thisController.isGrounded);
        }

        /// <summary>
        /// Gets or sets the <see cref="Player"/>'s health.
        /// </summary>
        /// <exception cref="Exception">Thrown when attempting to set health from the client.</exception>
        public int Health
        {
            get
            {
                return PlayerController.health;
            }
            set
            {
                if (!(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)) throw new Exception("Tried to set health on client.");

                PlayerController.health = value;

                SetHealthClientRpc(value);
            }
        }

        [ClientRpc]
        private void SetHealthClientRpc(int health)
        {
            if (!NetworkManager.Singleton.IsClient) return;

            int oldHealth = PlayerController.health;

            PlayerController.health = health;

            if (PlayerController.IsOwner) HUDManager.Instance.UpdateHealthUI(health, health < oldHealth);

            if (health <= 0 && !PlayerController.isPlayerDead && PlayerController.AllowPlayerDeath())
            {
                PlayerController.KillPlayer(default, true, CauseOfDeath.Unknown, 0);
            }
        }

        private void Start()
        {
            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
            {
                Dictionary.Add(PlayerController, this);
            }
            else
            {
                PlayerController = StartOfRound.Instance.allPlayerScripts.FirstOrDefault(c => c.actualClientId == NetworkClientId.Value);
                NetworkClientId.OnValueChanged += clientIdChanged;
            }
        }

        /// <summary>
        /// For internal use only. Do not use.
        /// </summary>
        public override void OnDestroy()
        {
            if (NetworkManager.Singleton.IsClient)
            {
                NetworkClientId.OnValueChanged -= clientIdChanged;
            }

            base.OnDestroy();
        }

        #region Network variable handlers
        private void clientIdChanged(ulong oldId, ulong newId)
        {
            PlayerController = StartOfRound.Instance.allPlayerScripts.FirstOrDefault(c => c.actualClientId == newId);
        }
        #endregion

        #region Player getters
        /// <summary>
        /// Gets or adds a player from a <see cref="PlayerControllerB"/>.
        /// </summary>
        /// <param name="playerController">The player's <see cref="PlayerControllerB"/>.</param>
        /// <returns>A <see cref="Player"/>.</returns>
        public static Player GetOrAdd(PlayerControllerB playerController)
        {
            if (Dictionary.TryGetValue(playerController, out Player player)) return player;

            foreach (Player p in FindObjectsOfType<Player>())
            {
                if (p.ClientId == playerController.actualClientId)
                {
                    Dictionary.Add(playerController, p);
                    return p;
                }
            }

            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                GameObject go = Instantiate(PlayerNetworkPrefab, Vector3.zero, default);
                go.SetActive(true);
                Player p = go.GetComponent<Player>();
                p.PlayerController = playerController;
                go.GetComponent<NetworkObject>().Spawn(false);

                return p;
            }

            return null;
        }

        /// <summary>
        /// Gets a player from a <see cref="PlayerControllerB"/>.
        /// </summary>
        /// <param name="playerController">The player's <see cref="PlayerControllerB"/>.</param>
        /// <returns>A <see cref="Player"/> or <see langword="null"/> if not found.</returns>
        public static Player Get(PlayerControllerB playerController)
        {
            if (Dictionary.TryGetValue(playerController, out Player player)) return player;

            return null;
        }

        /// <summary>
        /// Tries to get a player from a <see cref="PlayerControllerB"/>.
        /// </summary>
        /// <param name="playerController">The player's <see cref="PlayerControllerB"/>.</param>
        /// <param name="player">Outputs a <see cref="Player"/>, or <see langword="null"/> if not found.</param>
        /// <returns><see langword="true"/> if a player is found, <see langword="false"/> otherwise.</returns>
        public static bool TryGet(PlayerControllerB playerController, out Player player)
        {
            return Dictionary.TryGetValue(playerController, out player);
        }

        /// <summary>
        /// Gets a player from a player's client id.
        /// </summary>
        /// <param name="clientId">The player's client id.</param>
        /// <returns>A <see cref="Player"/> or <see langword="null"/> if not found.</returns>
        public static Player Get(ulong clientId)
        {
            foreach (Player player in List)
            {
                if (player.ClientId == clientId) return player;
            }

            return null;
        }

        /// <summary>
        /// Tries to get a player from a player's client id.
        /// </summary>
        /// <param name="clientId">The player's client id.</param>
        /// <param name="player">Outputs a <see cref="Player"/>, or <see langword="null"/> if not found.</param>
        /// <returns><see langword="true"/> if a player is found, <see langword="false"/> otherwise.</returns>
        public static bool TryGet(ulong clientId, out Player player)
        {
            return (player = Get(clientId)) != null;
        }
        #endregion
    }
}
