using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC_API.GameInterfaceAPI.Events.EventArgs.Player
{
    /// <summary>
    /// Contains all the information right before a player leaves the server. Including the local client.
    /// </summary>
    public class PlayerLeftEventArgs : System.EventArgs
    {
        /// <summary>
        /// Gets the player that is leaving.
        /// </summary>
        public PlayerControllerB Player { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerLeftEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player" /></param>
        public PlayerLeftEventArgs(PlayerControllerB player)
        {
            Player = player;
        }
    }
}
