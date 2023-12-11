using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace LC_API.GameInterfaceAPI.Features
{
    /// <summary>
    /// Encapsulates a <see cref="PlayerControllerB"/> for earier interacting.
    /// </summary>
    public class Player : NetworkBehaviour
    {
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
        public PlayerControllerB PlayerController { get; private set; }

        /// <summary>
        /// Gets the <see cref="Player"/>'s client id.
        /// </summary>
        public ulong ClientId => PlayerController.actualClientId;

        /// <summary>
        /// Gets whether or not this <see cref="Player"/> is the host.
        /// </summary>
        public new bool IsHost => PlayerController.isHostPlayerObject;

        /// <summary>
        /// Gets whether or not this <see cref="Player"/> is currently being controlled.
        /// Lethal Company creates PlayerControllers ahead of time, so all of them always exist.
        /// </summary>
        public bool IsPlayerControlled => PlayerController.isPlayerControlled;

        private NetworkVariable<int> _healthSync { get; } = new NetworkVariable<int>();

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
                if (!PlayerController.IsServer) throw new Exception("Tried to set health on client.");

                _healthSync.Value = value;
            }
        }

        private void Awake()
        {
            PlayerController = GetComponent<PlayerControllerB>();

            Dictionary.Add(PlayerController, this);

            _healthSync.OnValueChanged += healthSyncChanged;
        }

#pragma warning disable CS1591
        public override void OnDestroy()
        {
            _healthSync.OnValueChanged -= healthSyncChanged;
        }
#pragma warning restore

        private void healthSyncChanged(int oldHealth, int newHealth)
        {
            int health = newHealth;

            PlayerController.health = health;

            if (PlayerController.IsOwner) HUDManager.Instance.UpdateHealthUI(health, health < oldHealth);

            if (health <= 0 && !PlayerController.isPlayerDead && PlayerController.AllowPlayerDeath())
            {
                PlayerController.KillPlayer(default, true, CauseOfDeath.Unknown, 0);
            }
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
    }
}
