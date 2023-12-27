using UnityEngine;

namespace LC_API.GameInterfaceAPI.Events.EventArgs.Player
{
    /// <summary>
    /// Contains all the information before a <see cref="Features.Player"/> is dies.
    /// </summary>
    public class DyingEventArgs : System.EventArgs
    {
        /// <summary>
        /// Gets the player that is dying.
        /// </summary>
        public Features.Player Player { get; }

        /// <summary>
        /// Gets or sets the force to add to the ragdoll.
        /// </summary>
        public Vector3 Force { get; set; }

        /// <summary>
        /// Gets or sets whether or not to spawn a ragdoll.
        /// </summary>
        public bool SpawnBody { get; set; }

        /// <summary>
        /// Gets or sets the cause of death.
        /// </summary>
        public CauseOfDeath CauseOfDeath { get; set; }

        /// <summary>
        /// Gets or sets the death animation index.
        /// </summary>
        public int DeathAnimation { get; set; }

        /// <summary>
        /// Gets or sets whether this damage is allowed to occur.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="DyingEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player" /></param>
        /// <param name="force"><inheritdoc cref="Force" /></param>
        /// <param name="spawnBody"><inheritdoc cref="SpawnBody" /></param>
        /// <param name="causeOfDeath"><inheritdoc cref="CauseOfDeath" /></param>
        /// <param name="deathAnimation"><inheritdoc cref="DeathAnimation" /></param>
        public DyingEventArgs(Features.Player player, Vector3 force, bool spawnBody, CauseOfDeath causeOfDeath, int deathAnimation)
        {
            Player = player;
            Force = force;
            SpawnBody = spawnBody;
            CauseOfDeath = causeOfDeath;
            DeathAnimation = deathAnimation;
        }
    }
}
