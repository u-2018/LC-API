﻿using UnityEngine;

namespace LC_API.GameInterfaceAPI.Events.EventArgs.Player
{
    /// <summary>
    /// Contains all the information after a <see cref="Features.Player"/> is hurt.
    /// </summary>
    public class HurtEventArgs : System.EventArgs
    {
        /// <summary>
        /// Gets the player that is taking damage.
        /// </summary>
        public Features.Player Player { get; }

        /// <summary>
        /// Gets the amount of damage the <see cref="Player"/> took.
        /// </summary>
        public int Damage { get; }

        /// <summary>
        /// Gets or sets whether or not this damage will play sfx, if it has any.
        /// </summary>
        public bool HasSFX { get; set; }

        /// <summary>
        /// Gets or sets the cause of death.
        /// </summary>
        public CauseOfDeath CauseOfDeath { get; set; }

        /// <summary>
        /// Gets or sets the death animation index.
        /// </summary>
        public int DeathAnimation { get; set; }

        /// <summary>
        /// Gets or sets whether or not this damage is considered fall damage.
        /// </summary>
        public bool FallDamage { get; set; }

        /// <summary>
        /// Gets or sets the force to add to the ragdoll if the player is killed.
        /// </summary>
        public Vector3 Force { get; set; }

        /// <summary>
        /// Gets whether or not this damage will kill the player.
        /// </summary>
        public bool Killing => Player.Health <= 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="HurtingEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player" /></param>
        /// <param name="damage"><inheritdoc cref="Damage" /></param>
        /// <param name="hasSFX"><inheritdoc cref="HasSFX" /></param>
        /// <param name="causeOfDeath"><inheritdoc cref="CauseOfDeath" /></param>
        /// <param name="deathAnimation"><inheritdoc cref="DeathAnimation" /></param>
        /// <param name="fallDamage"><inheritdoc cref="FallDamage" /></param>
        /// <param name="force"><inheritdoc cref="Force" /></param>
        public HurtEventArgs(Features.Player player, int damage, bool hasSFX, CauseOfDeath causeOfDeath, int deathAnimation,
            bool fallDamage, Vector3 force)
        {
            Player = player;
            Damage = damage;
            HasSFX = hasSFX;
            CauseOfDeath = causeOfDeath;
            DeathAnimation = deathAnimation;
            FallDamage = fallDamage;
            Force = force;
        }
    }
}
