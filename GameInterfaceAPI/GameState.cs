using GameNetcodeStuff;
using LC_API.Data;
using LC_API.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace LC_API.GameInterfaceAPI
{
    /// <summary>
    /// Contains callbacks and information about the game's current state.
    /// </summary>
    public static class GameState
    {
        // stops the compiler whining about nullability
        private readonly static Action NothingAction = () => { };

        /// <summary>
        /// Provides the count of living players, as gotten through
        /// </summary>
        public static int AlivePlayerCount { get ; private set; }
        /// <summary>
        /// The state the ship is currently in. See <see cref="Data.ShipState"/>.
        /// </summary>
        public static ShipState ShipState { get ; private set; }

        /// <summary>
        /// Executes the frame after a player dies (determined by a living player count comparison)
        /// </summary>
        public static event Action PlayerDied = NothingAction;
        /// <summary>
        /// Executes the frame after the ship has landed.
        /// </summary>
        public static event Action LandOnMoon = NothingAction;
        /// <summary>
        /// Executes when the ship reaches orbit.
        /// </summary>
        public static event Action WentIntoOrbit = NothingAction;
        /// <summary>
        /// Executes when the ship starts taking off.
        /// </summary>
        public static event Action ShipStartedLeaving = NothingAction;

        internal static void GSUpdate()
        {
            if (StartOfRound.Instance == null)
                return;

            if (StartOfRound.Instance.shipHasLanded && ShipState != ShipState.OnMoon)
            {
                ShipState = ShipState.OnMoon;
                LandOnMoon.InvokeActionSafe();
            }

            if (StartOfRound.Instance.inShipPhase && ShipState != ShipState.InOrbit)
            {
                ShipState = ShipState.InOrbit;
                WentIntoOrbit.InvokeActionSafe();
            }

            if (StartOfRound.Instance.shipIsLeaving && ShipState != ShipState.LeavingMoon)
            {
                ShipState = ShipState.LeavingMoon;
                ShipStartedLeaving.InvokeActionSafe();
            }

            int aliveP = AlivePlayerCount;
            if (aliveP < StartOfRound.Instance.livingPlayers)
            {
                PlayerDied.InvokeActionSafe();
            }
            AlivePlayerCount = StartOfRound.Instance.livingPlayers;
        }
    }
}
