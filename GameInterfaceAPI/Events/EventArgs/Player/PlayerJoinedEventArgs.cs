using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC_API.GameInterfaceAPI.Events.EventArgs.Player
{
    /// <summary>
    /// Contains all the information after a player joined the server. Including the host.
    /// </summary>
    public class PlayerJoinedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Gets the joined player.
        /// </summary>
        public PlayerControllerB Player { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerJoinedEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player" /></param>
        public PlayerJoinedEventArgs(PlayerControllerB player)
        {
            Player = player;
        }
    }
}
