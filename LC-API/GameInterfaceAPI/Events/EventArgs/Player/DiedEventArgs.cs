using UnityEngine;

namespace LC_API.GameInterfaceAPI.Events.EventArgs.Player
{
    /// <summary>
    /// Contains all the information after a <see cref="Features.Player"/> dies.
    /// </summary>
    public class DiedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Gets the player that died.
        /// </summary>
        public Features.Player Player { get; }

        /// <summary>
        /// Gets the force that was added to the ragdoll.
        /// </summary>
        public Vector3 Force { get; }

        /// <summary>
        /// Gets whether or not a ragdoll was spawned.
        /// </summary>
        public bool SpawnBody { get; }

        /// <summary>
        /// Gets the cause of death.
        /// </summary>
        public CauseOfDeath CauseOfDeath { get; }

        /// <summary>
        /// Gets the death animation index.
        /// </summary>
        public int DeathAnimation { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiedEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player" /></param>
        /// <param name="force"><inheritdoc cref="Force" /></param>
        /// <param name="spawnBody"><inheritdoc cref="SpawnBody" /></param>
        /// <param name="causeOfDeath"><inheritdoc cref="CauseOfDeath" /></param>
        /// <param name="deathAnimation"><inheritdoc cref="DeathAnimation" /></param>
        public DiedEventArgs(Features.Player player, Vector3 force, bool spawnBody, CauseOfDeath causeOfDeath, int deathAnimation)
        {
            Player = player;
            Force = force;
            SpawnBody = spawnBody;
            CauseOfDeath = causeOfDeath;
            DeathAnimation = deathAnimation;
        }
    }
}